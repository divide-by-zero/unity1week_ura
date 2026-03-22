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
    [SerializeField] private CardGameScene _cardGameScene;
    [SerializeField] protected TextAsset _talkText;
    [SerializeField] private TextAsset _gameoverText;
    [SerializeField] protected TalkScript _talkScript;
    [SerializeField] private GamePhase _clearedPhase;
    [SerializeField] protected string _nextScene;

    [Inject] protected ChapterProgressManager _chapterProgress;

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
            AudioManager.Instance.Play(AudioEnum.GameClear);
            _chapterProgress.UpdateProgress(_clearedPhase);
            KszSceneManager.Instance.LoadAsync(_nextScene).Forget();
        }
        else
        {
            await OnGameOverAsync(ct);
            KszSceneManager.Instance.LoadAsync("Title").Forget();
        }
    }

    protected virtual async UniTask OnGameOverAsync(CancellationToken ct)
    {
        AudioManager.Instance.Play(AudioEnum.GameOver);
        if (_gameoverText != null)
        {
            await _talkScript.TalkSceneLoadAsync(_gameoverText.text, ct);
        }
    }
}