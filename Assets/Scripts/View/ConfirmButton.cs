using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace InfinitePorker.View
{
    public class ConfirmButton : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private TMP_Text _buttonText;

        public IObservable<Unit> OnClickAsObservable() => _button.OnClickAsObservable();

        private void Reset()
        {
            _button = GetComponent<Button>();
            _buttonText = GetComponentInChildren<TMP_Text>();
        }

        public void SetVisible(bool isVisible)
        {
            _button.gameObject.SetActive(isVisible);
        }

        public async UniTask PresentAsync(CancellationToken ct = default)
        {
            _button.gameObject.SetActive(true);
        }

        public async UniTask DismissAsync(CancellationToken ct = default)
        {
            _button.gameObject.SetActive(false);
        }

        public void SetText(string text)
        {
            _buttonText.text = text;
        }

        public void SetInteractable(bool isInteractable)
        {
            _button.interactable = isInteractable;
        }
    }
}