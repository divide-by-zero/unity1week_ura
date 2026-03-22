using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace KszUtil.UI
{
    public class LongHoldButtonEffect : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private LongHoldButton _longHoldButton;

        private void Start()
        {
            _longHoldButton.HoldRatioAsObservable().Subscribe(ratio => _image.fillAmount = ratio).AddTo(this);
        }
    }
}