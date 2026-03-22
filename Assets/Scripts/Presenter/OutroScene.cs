using System.Threading;
using Cysharp.Threading.Tasks;
using InfinitePorker.Enums;
using KszUtil.SceneManager;
using UniRx;
using UnityEngine;
using VContainer;

public class OutroScene : MonoBehaviour
{
    [SerializeField] private TalkScript _talkScript;
    [SerializeField] private TextAsset _talkText;
    [SerializeField] private TextAsset _tweetText;
    [SerializeField] private string _nextScene;

    [Inject] private ChapterProgressManager _chapterProgress;

    private void Start() => SequenceTask(destroyCancellationToken).Forget();

    private async UniTask SequenceTask(CancellationToken ct)
    {
        _talkScript.OnChoiceAAsObservable().Subscribe(id => SendTweet()).AddTo(this);
        _talkScript.OnChoiceBAsObservable().Subscribe(id => LoadNextScene()).AddTo(this);

        await UniTask.Yield(ct);
        await _talkScript.TalkSceneLoadAsyncWithoutDismiss(_talkText.text, ct);

        while (ct.IsCancellationRequested == false)
        {
            await _talkScript.TalkSceneLoadAsyncWithoutDismiss(_tweetText.text, ct);
            if (_talkScript.GetChoiceResult(0) == false) break;
        }
    }

    private void LoadNextScene()
    {
        KszSceneManager.Instance.LoadAsync(_nextScene).Forget();
    }

    private void SendTweet()
    {
        var tweetText = "BackDoorクリアおめでとうございます！ネタバレは控えていただけると嬉しいです！";
        naichilab.UnityRoomTweet.Tweet("backdoor", tweetText, "unity1week");
    }
}