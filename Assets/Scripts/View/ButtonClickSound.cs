using KszUtil.Utilities;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace InfinitePorker
{
    [RequireComponent(typeof(Button))]
    public class ButtonClickSound : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private string _audioName;

        private void Start()
        {
            _button.OnClickAsObservable().Subscribe(_ => AudioManager.Instance.Play(_audioName)).AddTo(this);
        }

        private void Reset()
        {
            _button = GetComponent<Button>();
        }
    }
}