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

    private bool _isPlaying;
    private Tweener _selectTween;
    private Tweener _hoverTween;
    private Vector3 _baseLocalPos;

    private void Start()
    {
        _text.text = "" + "0123456789ABCDEF".RandomAt() + "0123456789ABCDEF".RandomAt();
        _baseLocalPos = _cardParent.localPosition;
    }

    // --- Open / Close ---

    public async UniTask PlayOpen(CancellationToken ct = default)
    {
        if (_isPlaying || IsOpen.Value) return;
        _isPlaying = true;

        _spriteRenderer.sprite = _porkerSetting.CardSprites.RandomAt();

        var seq = DOTween.Sequence()
            .Append(_cardParent.DOLocalMoveY(_baseLocalPos.y + _flipLiftHeight, _flipLiftDuration).SetEase(Ease.OutQuad))
            .Append(_cardParent.DOLocalRotate(new Vector3(0f, 0f, 180f), _flipDuration).SetEase(_flipEase))
            .Append(_cardParent.DOLocalMoveY(_baseLocalPos.y, _flipLiftDuration).SetEase(Ease.InQuad))
            .SetLink(gameObject);
        await seq.ToUniTask(cancellationToken: ct);

        IsOpen.Value = true;
        _isPlaying = false;
    }

    public async UniTask PlayClose(CancellationToken ct = default)
    {
        if (_isPlaying || !IsOpen.Value) return;
        _isPlaying = true;

        var seq = DOTween.Sequence()
            .Append(_cardParent.DOLocalMoveY(_baseLocalPos.y + _flipLiftHeight, _flipLiftDuration).SetEase(Ease.OutQuad))
            .Append(_cardParent.DOLocalRotate(Vector3.zero, _flipDuration).SetEase(_flipEase))
            .Append(_cardParent.DOLocalMoveY(_baseLocalPos.y, _flipLiftDuration).SetEase(Ease.InQuad))
            .SetLink(gameObject);
        await seq.ToUniTask(cancellationToken: ct);

        IsOpen.Value = false;
        _isPlaying = false;
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
        if (IsOpen.Value)
            PlayClose(destroyCancellationToken).Forget();
        else
            PlayOpen(destroyCancellationToken).Forget();
    }

    public void UpdateCard(string cardRankText, int cardDataSuit, bool cardIsRedSuit, int cardDataRank)
    {
    }
}