using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using KszUtil;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class GenericDialog : MonoBehaviour
{
    [Serializable]
    public class BackgroundColorAndText
    {
        public Graphic Background;
        public TMP_Text Text;
        public Button Button;

        public void Apply(DialogText dt)
        {
            if (dt == null)
            {
                Background.gameObject.SetActive(false);
                return;
            }
            else
            {
                Background.gameObject.SetActive(true);
            }

            if (dt.BackGroundColor.HasValue)
            {
                Background.color = dt.BackGroundColor.Value;
            }

            if (dt.ForeGroundColor.HasValue)
            {
                Text.color = dt.ForeGroundColor.Value;
            }

            Text.text = dt.Text;
        }
    }

    [SerializeField] private BackgroundColorAndText _titleText;
    [SerializeField] private BackgroundColorAndText _messageText;
    [SerializeField] private BackgroundColorAndText[] _buttonsText;
    [SerializeField] private UIFader _uiFader;
    [SerializeField] private LayoutGroup _layoutGroup;

    public UIFader UIFader => _uiFader;

    public UniTask<(string name, int index)> ShowDialogAsync(DialogText title, DialogText message, params DialogText[] buttons)
    {
        var tcs = new UniTaskCompletionSource<(string name, int index)>();
        ShowDialog(title, message, tuple => tcs.TrySetResult(tuple), buttons);
        return tcs.Task;
    }

    public UniTask<(string name, int index)> ShowDialogAsync(string title, string message, params string[] buttons)
    {
        return ShowDialogAsync(DialogText.Create(title), DialogText.Create(message), buttons.Select(s => DialogText.Create(s)).ToArray());
    }

    private CompositeDisposable _disposable = new CompositeDisposable();

    public void ShowDialog(DialogText title, DialogText message, Action<(string name, int index)> callbackAction, params DialogText[] buttons)
    {
        _disposable?.Dispose();
        _disposable = new CompositeDisposable();
        _titleText.Apply(title);
        _messageText.Apply(message);
        for (var index = 0; index < _buttonsText.Length; index++)
        {
            var button = buttons.ElementAtOrDefault(index);
            _buttonsText[index].Apply(button);
            if (button != null)
            {
                var index1 = index;
                _buttonsText[index].Button.OnClickAsObservable().Subscribe(_ => callbackAction?.Invoke((button.Text, index1))).AddTo(_disposable);
            }
        }

        UIFader.Show();
        LayoutRebuilder.ForceRebuildLayoutImmediate(_layoutGroup.transform.ToRectTransform());
    }

    public void ShowDialog(DialogText message, Action<(string name, int index)> callbackAction, params DialogText[] buttons)
    {
        ShowDialog(null, message, callbackAction, buttons);
    }

    public void ShowDialog(string title, string message, Action<(string name, int index)> callbackAction, params string[] buttons)
    {
        ShowDialog(DialogText.Create(title), DialogText.Create(message), callbackAction, buttons.Select(s => DialogText.Create(s)).ToArray());
    }

    public void ShowDialog(string message, Action<(string name, int index)> callbackAction, params string[] buttons)
    {
        ShowDialog(null, message, callbackAction, buttons);
    }

    public async UniTask DismissDialogAsync(CancellationToken ct = default)
    {
        UIFader.Show(false);
        await UniTask.WaitWhile(() => UIFader.IsShow, cancellationToken: ct);
    }

    public async UniTask ShowDialogAwaitClose(string title, string messsage)
    {
        await ShowDialogAsync(title, messsage, "OK");
        await DismissDialogAsync();
    }

    public void DismissDialog()
    {
        UIFader.Show(false);
    }
}