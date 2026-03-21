using System.Threading;
using Cysharp.Threading.Tasks;
using InfinitePorker.Enums;
using UnityEngine;
using VContainer;

public class Phase3 : MonoBehaviour
{
    [SerializeField] private CardGameScene _cardGameScene;
    [SerializeField] private TextAsset _talkText;
    [SerializeField] private TalkScript _talkScript;

    [Inject] private ChapterProgressManager _chapterProgress;

    private void Start() => SequenceTask(destroyCancellationToken).Forget();

    private async UniTask SequenceTask(CancellationToken ct)
    {
        await _talkScript.TalkSceneLoadAsync(_talkText.text, ct);

        await _cardGameScene.GameLoopAsync(ct);

        _chapterProgress.UpdateProgress(GamePhase.Phase3);
    }
}