using System.Threading;
using Cysharp.Threading.Tasks;
using InfinitePorker.Enums;
using KszUtil.SceneManager;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

public class BackDoorScene : MonoBehaviour
{
    [SerializeField] private TalkScript _talkScript;
    [SerializeField] private TextAsset _talkAText;
    [SerializeField] private TextAsset _talkBText;
    [SerializeField] private MultiWindowDetector _multiWindowDetector;

    private void Start() => SequenceTask(destroyCancellationToken).Forget();

    private async UniTask SequenceTask(CancellationToken ct)
    {
        await UniTask.Yield(ct);

        await _talkScript.TalkSceneLoadAsyncWithoutDismiss(_talkAText.text, ct);

        // await UniTask.WaitUntil(() => Mouse.current != null
        //                               && Mouse.current.leftButton.wasPressedThisFrame, cancellationToken: ct);

        await _talkScript.TalkSceneLoadAsyncWithoutDismiss(_talkBText.text, ct);
        _multiWindowDetector.SendPong("後は頼んだわよレナ！");
    }
}