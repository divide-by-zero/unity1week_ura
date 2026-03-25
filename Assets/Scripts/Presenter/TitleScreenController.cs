using System;
using System.Linq;
using InfinitePorker.Enums;
using KszUtil;
using KszUtil.SceneManager;
using KszUtil.Utilities;
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
        public Button Button;
        public string SceneName;
        public bool isSpecialContinue;
    }

    [SerializeField] private ButtonInfo[] _buttonInfos;
    [SerializeField] private TMP_Text _testText;
    [SerializeField] private GamePhase _debugClearedPhase;
    [SerializeField] private MultiWindowDetector _windowDetector;
    [SerializeField] private AudioClip _muonClip;

    [SerializeField] private Slider _bgmVolumeSlider;
    [SerializeField] private Slider _seVolumeSlider;


    [Inject] private ChapterProgressManager _chapterProgress;

    private void Start()
    {
        InitializeAudioSliders();
#if UNITY_EDITOR
        if (_debugClearedPhase != GamePhase.None)
        {
            _chapterProgress.ClearedPhases = _debugClearedPhase;
        }
#endif
        _windowDetector.PongReceived.Subscribe(s =>
        {
            Debug.Log(s);
            UpdateButtonStates(GamePhase.BackDoor);
            AudioManager.Instance.PlayBGM(_muonClip);
        }).AddTo(this);

        _windowDetector.SendPing("リナまだ待ってる？");

        UpdateButtonStates(_chapterProgress.ClearedPhases);

        _buttonInfos.Select(info => info.Button.OnClickAsObservable().Select(_ => info))
            .Merge()
            .Subscribe(info => LoadChapter(info))
            .AddTo(this);

        _testText.text = "copyright (c) 2026 tsu-ki All rights reserved.";
    }

    private void UpdateButtonStates(GamePhase phase)
    {
        foreach (var info in _buttonInfos)
        {
            info.Button.gameObject.SetActive(info.VisiblePhase.HasAnyFlags(phase));
        }
    }

    private void LoadChapter(ButtonInfo info)
    {
        _chapterProgress.IsSpecialContinue = info.isSpecialContinue;
        KszSceneManager.Instance.LoadAsync(info.SceneName);
    }

    private void InitializeAudioSliders()
    {
        // 初期値設定
        _bgmVolumeSlider.value = AudioManager.Instance.BgmVolume;
        _seVolumeSlider.value = AudioManager.Instance.SeVolume;

        // BGMボリューム変更
        _bgmVolumeSlider.OnValueChangedAsObservable()
            .Subscribe(volume => AudioManager.Instance.BgmVolume = volume)
            .AddTo(this);

        // SEボリューム変更
        _seVolumeSlider.OnValueChangedAsObservable()
            .Throttle(TimeSpan.FromSeconds(0.2f))
            .Subscribe(volume =>
            {
                AudioManager.Instance.SeVolume = volume;
                AudioManager.Instance.Play(AudioEnum.Check);
            })
            .AddTo(this);
    }
}