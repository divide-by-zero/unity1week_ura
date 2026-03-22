using System.Threading;
using Cysharp.Threading.Tasks;
using InfinitePorker.Enums;
using KszUtil.SceneManager;
using KszUtil.Utilities;
using UniRx;
using UnityEngine;
using VContainer;

public abstract class PhaseSceneBase : MonoBehaviour
{
    [SerializeField] protected CardGameScene _cardGameScene;
    [SerializeField] protected TextAsset _talkText;
    [SerializeField] protected TextAsset _gameoverText;
    [SerializeField] protected TextAsset _gameClearText;

    [SerializeField] protected TalkScript _talkScript;
    [SerializeField] private GamePhase _clearedPhase;
    [SerializeField] protected string _nextScene;

    [Inject] protected ChapterProgressManager _chapterProgress;

    protected CardGameScene CardGameScene => _cardGameScene;
    protected TalkScript TalkScript => _talkScript;

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
                AudioManager.Instance.Play(AudioEnum.Select);
                Hologram(view, true);
                var text = GetHologramText(view);
                if (text == null) return;
                view.SetHologramText(text);
                view.HologramOn();
            })
            .AddTo(this);

        _cardGameScene.OnHoverExitAsObservable()
            .Subscribe(view =>
            {
                Hologram(null, false);
                view.HologramOff();
            })
            .AddTo(this);
    }

    protected abstract string GetHologramText(CardView view);

    protected abstract void Hologram(CardView text, bool isOn);

    protected virtual async UniTask SequenceTask(CancellationToken ct)
    {
        await UniTask.Yield();

        await _talkScript.TalkSceneLoadAsync(_talkText.text, ct);

        var cleared = await _cardGameScene.GameLoopAsync(ct);

        if (cleared)
        {
            await OnGameClearAsync(_gameClearText.text, _nextScene, ct);
        }
        else
        {
            await OnGameOverAsync(_gameoverText.text, "Title", ct);
        }
    }

    protected async UniTask OnGameClearAsync(string talkText, string nextScene, CancellationToken ct)
    {
        AudioManager.Instance.Play(AudioEnum.GameClear);
        _chapterProgress.UpdateProgress(_clearedPhase);
        await _talkScript.TalkSceneLoadAsync(talkText, ct);
        KszSceneManager.Instance.LoadAsync(nextScene).Forget();
    }

    protected async UniTask OnGameOverAsync(string talkText, string nextScene, CancellationToken ct)
    {
        AudioManager.Instance.Play(AudioEnum.GameOver);
        await _talkScript.TalkSceneLoadAsync(talkText, ct);
        KszSceneManager.Instance.LoadAsync(nextScene).Forget();
    }
}