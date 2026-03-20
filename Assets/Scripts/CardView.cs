using System.Threading;
using Cysharp.Threading.Tasks;
using KszUtil;
using UniRx;
using UnityEngine;
using UnityEngine.Playables;

public class CardView : MonoBehaviour
{
    [SerializeField] private PlayableDirector _turnOpen;
    [SerializeField] private PlayableDirector _turnClose;
    [SerializeField, Range(0f, 1f)] private float _toggleProbability = 0.3f;
    [SerializeField] private float _intervalSeconds = 2f;

    public BoolReactiveProperty IsOpen { get; } = new(false);

    private bool _isPlaying;

    private void Awake()
    {
        RandomToggleLoop(destroyCancellationToken).Forget();
    }

    private async UniTaskVoid RandomToggleLoop(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            await UniTask.Delay(
                System.TimeSpan.FromSeconds(_intervalSeconds),
                cancellationToken: ct
            );

            if (_isPlaying) continue;
            if (Random.value > _toggleProbability) continue;

            if (IsOpen.Value)
                await PlayClose(ct);
            else
                await PlayOpen(ct);
        }
    }

    public async UniTask PlayOpen(CancellationToken ct = default)
    {
        if (_isPlaying || IsOpen.Value) return;
        _isPlaying = true;

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