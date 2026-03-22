using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using InfinitePorker.Logic;
using KszUtil;
using TMPro;
using UniRx;
using UnityEngine;

public class CardView : MonoBehaviour, IMouseHoverable, IMouseClickable
{
    [Header("参照")] [SerializeField] private Transform _cardParent;
    [SerializeField] private Transform _hologramParent;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private PorkerSetting _porkerSetting;
    [SerializeField] private TextMeshPro _text;

    [Header("Open / Close")] [SerializeField]
    private float _flipDuration = 0.5f;

    [SerializeField] private Ease _flipEase = Ease.OutQuart;
    [SerializeField] private float _flipLiftHeight = 0.3f;
    [SerializeField] private float _flipLiftDuration = 0.15f;

    [Header("Select / Deselect")] [SerializeField]
    private float _selectOffsetY = 0.15f;

    [SerializeField] private float _selectDuration = 0.2f;
    [SerializeField] private Ease _selectEase = Ease.OutQuad;

    [Header("MouseOver On / Off")] [SerializeField]
    private float _hoverScale = 1.08f;

    [SerializeField] private float _hoverDuration = 0.15f;
    [SerializeField] private Ease _hoverEase = Ease.OutQuad;

    public BoolReactiveProperty IsOpen { get; } = new(false);

    private readonly Subject<CardView> _onClickSubject = new();
    public IObservable<CardView> OnClickAsObservable() => _onClickSubject;

    private readonly Subject<CardView> _onHoverEnterSubject = new();
    private readonly Subject<CardView> _onHoverExitSubject = new();
    public IObservable<CardView> OnHoverEnterAsObservable() => _onHoverEnterSubject;
    public IObservable<CardView> OnHoverExitAsObservable() => _onHoverExitSubject;

    public bool IsClickable { get; set; } = true;
    public bool IsFlipping { get; private set; }
    public CardGameScene.CardData CardData { get; set; }

    private Sprite _presetSprite;

    private Tweener _selectTween;
    private Tweener _hoverTween;
    private Vector3 _baseLocalPos;

    private void Start()
    {
        _baseLocalPos = _cardParent.localPosition;
    }

    public void SetHologramText(string text)
    {
        _text.text = text;
    }

    public void SetCardSprite(Sprite sprite)
    {
        _presetSprite = sprite;
    }

    // --- Open / Close (UniTask) ---

    public async UniTask PlayOpenAsync(CancellationToken ct = default)
    {
        if (IsFlipping || IsOpen.Value) return;
        _spriteRenderer.sprite = _presetSprite != null ? _presetSprite : _porkerSetting.CardSprites.RandomAt();
        await FlipAsync(new Vector3(0f, 0f, 180f), true, ct);
    }

    public async UniTask PlayCloseAsync(CancellationToken ct = default)
    {
        if (IsFlipping || !IsOpen.Value) return;
        await FlipAsync(Vector3.zero, false, ct);
    }

    private async UniTask FlipAsync(Vector3 targetRotation, bool openState, CancellationToken ct)
    {
        IsFlipping = true;

        var liftPos = _baseLocalPos + Vector3.up * _flipLiftHeight;

        // 持ち上げ
        await _cardParent.DOLocalMove(liftPos, _flipLiftDuration)
            .SetEase(Ease.OutQuad)
            .SetLink(gameObject)
            .ToUniTask(cancellationToken: ct);

        // 回転
        await _cardParent.DOLocalRotate(targetRotation, _flipDuration)
            .SetEase(_flipEase)
            .SetLink(gameObject)
            .ToUniTask(cancellationToken: ct);

        // 下ろす
        await _cardParent.DOLocalMove(_baseLocalPos, _flipLiftDuration)
            .SetEase(Ease.InQuad)
            .SetLink(gameObject)
            .ToUniTask(cancellationToken: ct);

        IsOpen.Value = openState;
        IsFlipping = false;
    }

    // --- Select / Deselect ---

    public void Select()
    {
        _selectTween?.Kill();
        _selectTween = _cardParent.DOLocalMoveY(_baseLocalPos.y + _selectOffsetY, _selectDuration)
            .SetEase(_selectEase)
            .SetLink(gameObject);
    }

    public void Deselect()
    {
        _selectTween?.Kill();
        _selectTween = _cardParent.DOLocalMoveY(_baseLocalPos.y, _selectDuration)
            .SetEase(_selectEase)
            .SetLink(gameObject);
    }

    // --- MouseOver On / Off ---

    public void OnHoverEnter()
    {
        _onHoverEnterSubject.OnNext(this);
        _hoverTween?.Kill();
        _hoverTween = transform.DOScale(_hoverScale, _hoverDuration)
            .SetEase(_hoverEase)
            .SetLink(gameObject);
    }

    public void OnHoverExit()
    {
        _onHoverExitSubject.OnNext(this);
        _hoverTween?.Kill();
        _hoverTween = transform.DOScale(1f, _hoverDuration)
            .SetEase(_hoverEase)
            .SetLink(gameObject);
    }

    // --- Hologram ---

    public void HologramOn()
    {
        _hologramParent.DOScale(Vector3.one, _hoverDuration)
            .SetEase(Ease.OutBack)
            .SetLink(gameObject);
    }

    public void HologramOff()
    {
        _hologramParent.DOScale(Vector3.zero, _hoverDuration)
            .SetEase(Ease.InBack)
            .SetLink(gameObject);
    }

    // --- Click ---

    public void OnClick()
    {
        if (!IsClickable || IsFlipping || IsOpen.Value) return;
        _onClickSubject.OnNext(this);
    }

    public void UpdateCard(string cardRankText, int cardDataSuit, bool cardIsRedSuit, int cardDataRank)
    {
    }
}
