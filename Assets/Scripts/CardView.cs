using System;
using System.Collections;
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

    public bool IsClickable { get; set; } = true;
    public bool IsFlipping { get; private set; }

    private Sprite _presetSprite;

    private Tweener _selectTween;
    private Tweener _hoverTween;
    private Vector3 _baseLocalPos;

    private void Start()
    {
        _text.text = "" + "0123456789ABCDEF".RandomAt() + "0123456789ABCDEF".RandomAt();
        _baseLocalPos = _cardParent.localPosition;
    }

    public void SetCardSprite(Sprite sprite)
    {
        _presetSprite = sprite;
    }

    // --- Open / Close (コルーチンベース) ---

    public Coroutine PlayOpen()
    {
        if (IsFlipping || IsOpen.Value) return null;
        _spriteRenderer.sprite = _presetSprite != null ? _presetSprite : _porkerSetting.CardSprites.RandomAt();
        return StartCoroutine(FlipCoroutine(new Vector3(0f, 0f, 180f), true));
    }

    public Coroutine PlayClose()
    {
        if (IsFlipping || !IsOpen.Value) return null;
        return StartCoroutine(FlipCoroutine(Vector3.zero, false));
    }

    private IEnumerator FlipCoroutine(Vector3 targetRotation, bool openState)
    {
        IsFlipping = true;

        var startPos = _baseLocalPos;
        var liftPos = _baseLocalPos + Vector3.up * _flipLiftHeight;
        var startRot = _cardParent.localEulerAngles;

        // 持ち上げ
        yield return LerpCoroutine(_flipLiftDuration,
            t => _cardParent.localPosition = Vector3.Lerp(startPos, liftPos, EaseOutQuad(t)));

        // 回転
        yield return LerpCoroutine(_flipDuration,
            t => _cardParent.localEulerAngles = Vector3.Lerp(startRot, targetRotation, t));

        // 下ろす
        yield return LerpCoroutine(_flipLiftDuration,
            t => _cardParent.localPosition = Vector3.Lerp(liftPos, startPos, EaseInQuad(t)));

        IsOpen.Value = openState;
        IsFlipping = false;
    }

    private static IEnumerator LerpCoroutine(float duration, Action<float> onUpdate)
    {
        var elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            onUpdate(Mathf.Clamp01(elapsed / duration));
            yield return null;
        }
        onUpdate(1f);
    }

    private static float EaseOutQuad(float t) => 1f - (1f - t) * (1f - t);
    private static float EaseInQuad(float t) => t * t;

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
        HologramOn();
        _hoverTween?.Kill();
        _hoverTween = transform.DOScale(_hoverScale, _hoverDuration)
            .SetEase(_hoverEase)
            .SetLink(gameObject);
    }

    public void OnHoverExit()
    {
        HologramOff();
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
