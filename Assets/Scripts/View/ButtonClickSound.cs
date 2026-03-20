using KszUtil.AudioManager;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace InfinitePorker
{
    [RequireComponent(typeof(Button))]
    public class ButtonClickSound : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private AudioEnum _audioEnum;

        private void Start()
        {
            _button.OnClickAsObservable().Subscribe(_ => AudioManager.Instance.Play(_audioEnum)).AddTo(this);
        }

        private void Reset()
        {
            _button = GetComponent<Button>();
        }
    }
}