using System;
using System.Linq;
using InfinitePorker.Enums;
using KszUtil;
using KszUtil.SceneManager;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class TitleScreenController : MonoBehaviour
{
    [Serializable]
    public class ButtonInfo
    {
        public GamePhase VisiblePhase;
        [Header("章ボタン")] public Button Button;
        [Header("シーン名")] public string SceneName;
    }

    [SerializeField] private ButtonInfo[] _buttonInfos;
    [SerializeField] private TMP_Text _testText;
    [SerializeField] private GamePhase _debugClearedPhase;
    [SerializeField] private MultiWindowDetector _windowDetector;

    [Inject] private ChapterProgressManager _chapterProgress;

    private void Start()
    {
#if UNITY_EDITOR
        if (_debugClearedPhase != GamePhase.None)
        {
            _chapterProgress.ClearedPhases = _debugClearedPhase;
        }
#endif
        _windowDetector.PingReceived.Subscribe(s =>
        {
            UpdateButtonStates(GamePhase.BackDoor);
        }).AddTo(this);

        UpdateButtonStates(_chapterProgress.ClearedPhases);

        _buttonInfos.Select(info => info.Button.OnClickAsObservable().Select(_ => info.SceneName))
            .Merge()
            .Subscribe(sceneName => LoadChapter(sceneName))
            .AddTo(this);

        _testText.text = "version 1.0 copyright (c) 2026 tsu-ki All rights reserved.";
    }

    private void UpdateButtonStates(GamePhase phase)
    {
        foreach (var info in _buttonInfos)
        {
            info.Button.gameObject.SetActive(info.VisiblePhase.HasAnyFlags(phase));
        }
    }

    private void LoadChapter(string sceneName)
    {
        KszSceneManager.Instance.LoadAsync(sceneName);
    }
}