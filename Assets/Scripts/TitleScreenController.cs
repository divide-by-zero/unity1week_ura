using KszUtil.SceneManager;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class TitleScreenController : MonoBehaviour
{
    [Header("章ボタン")]
    [SerializeField] private Button _chapter1Button;
    [SerializeField] private Button _chapter2Button;
    [SerializeField] private Button _chapter3Button;

    [Header("シーン名")]
    [SerializeField] private string _chapter1Scene = "Chapter1";
    [SerializeField] private string _chapter2Scene = "Chapter2";
    [SerializeField] private string _chapter3Scene = "Chapter3";

    [Inject] private ChapterProgressManager _chapterProgress;

    private void Start()
    {
        UpdateButtonStates();

        _chapter1Button.OnClickAsObservable()
            .Subscribe(_ => LoadChapter(_chapter1Scene))
            .AddTo(this);
        _chapter2Button.OnClickAsObservable()
            .Subscribe(_ => LoadChapter(_chapter2Scene))
            .AddTo(this);
        _chapter3Button.OnClickAsObservable()
            .Subscribe(_ => LoadChapter(_chapter3Scene))
            .AddTo(this);
    }

    private void UpdateButtonStates()
    {
        SetButtonState(_chapter1Button, 1);
        SetButtonState(_chapter2Button, 2);
        SetButtonState(_chapter3Button, 3);
    }

    private void SetButtonState(Button button, int chapter)
    {
        bool unlocked = _chapterProgress.IsChapterUnlocked(chapter);
        button.gameObject.SetActive(unlocked);
    }

    private void LoadChapter(string sceneName)
    {
        KszSceneManager.Instance.LoadAsync(sceneName);
    }
}
