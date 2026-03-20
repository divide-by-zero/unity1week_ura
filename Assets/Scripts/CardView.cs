using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using InfinitePorker.Logic;
using KszUtil;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Playables;

public class CardView : MonoBehaviour, IMouseHoverable
{
    [SerializeField] private PlayableDirector _turnOpen;
    [SerializeField] private PlayableDirector _turnClose;
    [SerializeField, Range(0f, 1f)] private float _toggleProbability = 0.3f;
    [SerializeField] private float _intervalSeconds = 2f;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private PorkerSetting _porkerSetting;
    [SerializeField] private TextMeshPro _text;

    public BoolReactiveProperty IsOpen { get; } = new(false);

    private bool _isPlaying;

    private void Start()
    {
        _text.text = "" + "0123456789ABCDEF".RandomAt() + "0123456789ABCDEF".RandomAt();
    }

    public async UniTask PlayOpen(CancellationToken ct = default)
    {
        if (_isPlaying || IsOpen.Value) return;
        _isPlaying = true;

        _spriteRenderer.sprite = _porkerSetting.CardSprites.RandomAt();

        await _turnOpen.PlayAsync(ct);

        IsOpen.Value = true;
        _isPlaying = false;
    }

    public async UniTask PlayClose(CancellationToken ct = default)
    {
        if (_isPlaying || !IsOpen.Value) return;
        _isPlaying = true;

        await _turnClose.PlayAsync(ct);

        IsOpen.Value = false;
        _isPlaying = false;
    }

    public void OnHoverEnter()
    {
        PlayOpen(destroyCancellationToken).Forget();
    }

    public void OnHoverExit()
    {
        PlayClose(destroyCancellationToken).Forget();
    }

    public void UpdateCard(string cardRankText, int cardDataSuit, bool cardIsRedSuit, int cardDataRank)
    {
    }

    public async UniTask CurrentSelectAsync(CancellationToken cardViewDestroyCancellationToken)
    {
    }

    public async UniTask CurrentDeSelectAsync(CancellationToken cardViewDestroyCancellationToken)
    {
    }
}