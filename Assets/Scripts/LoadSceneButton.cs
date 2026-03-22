using System;
using Cysharp.Threading.Tasks;
using KszUtil.SceneManager;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class LoadSceneButton : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private string _sceneName = "Title";

    private void Reset()
    {
        TryGetComponent(out _button);
    }

    void Start()
    {
        _button.OnClickAsObservable()
            .Subscribe(_ => KszSceneManager.Instance.LoadAsync(_sceneName).Forget())
            .AddTo(this);
    }
}