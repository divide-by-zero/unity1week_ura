using System.Threading;
using Cysharp.Threading.Tasks;
using InfinitePorker.Enums;
using KszUtil.SceneManager;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class IntroScene : MonoBehaviour
{
    [SerializeField] private TalkScript _talkScript;
    [SerializeField] private TextAsset _talkText;
    [SerializeField] private string _nextScene;
    [SerializeField] private Button _skipButton;

    [Inject] private ChapterProgressManager _chapterProgress;

    private void Start() => SequenceTask(destroyCancellationToken).Forget();

    private async UniTask SequenceTask(CancellationToken ct)
    {
        await UniTask.Yield(ct);

        await _talkScript.TalkSceneLoadAsync(_talkText.text, ct);

        _chapterProgress.UpdateProgress(GamePhase.Intro);

        KszSceneManager.Instance.LoadAsync(_nextScene).Forget();
    }
}