using System.Threading;
using Cysharp.Threading.Tasks;
using InfinitePorker.Enums;
using KszUtil.SceneManager;
using UniRx;
using UnityEngine;
using VContainer;

public abstract class PhaseSceneBase : MonoBehaviour
{
    [SerializeField] private CardGameScene _cardGameScene;
    [SerializeField] private TextAsset _talkText;
    [SerializeField] private TextAsset _gameoverText;
    [SerializeField] private TalkScript _talkScript;
    [SerializeField] private GamePhase _clearedPhase;
    [SerializeField] private string _nextScene;

    [Inject] private ChapterProgressManager _chapterProgress;

    protected CardGameScene CardGameScene => _cardGameScene;
    protected TalkScript TalkScript => _talkScript;
    protected TextAsset GameoverText => _gameoverText;

    private void Start()
    {
        SubscribeHologram();
        SequenceTask(destroyCancellationToken).Forget();
    }

    private void SubscribeHologram()
    {
        _cardGameScene.OnHoverEnterAsObservable()
            .Subscribe(view =>
            {
                var text = GetHologramText(view);
                if (text == null) return;
                view.SetHologramText(text);
                view.HologramOn();
            })
            .AddTo(this);

        _cardGameScene.OnHoverExitAsObservable()
            .Subscribe(view =>
            {
                view.HologramOff();
            })
            .AddTo(this);
    }

    protected abstract string GetHologramText(CardView view);

    protected virtual async UniTask SequenceTask(CancellationToken ct)
    {
        await UniTask.Yield();

        await _talkScript.TalkSceneLoadAsync(_talkText.text, ct);

        await _cardGameScene.GameLoopAsync(ct);

        _chapterProgress.UpdateProgress(_clearedPhase);

        KszSceneManager.Instance.LoadAsync(_nextScene).Forget();
    }
}