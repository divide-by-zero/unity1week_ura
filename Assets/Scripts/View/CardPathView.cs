using UnityEngine;
using UnityEngine.UI;

namespace InfinitePorker.View
{
    public class CardPathView : MonoBehaviour
    {
        [SerializeField] private Image _pathBase;
        [SerializeField] private Image _pathImage;

        private Color _originalColor;

        private void Awake()
        {
            _originalColor = _pathImage.color;
        }

        public void SetPathUsed()
        {
            _pathImage.color = Color.darkGray;
        }

        public void SetPathNormal()
        {
            _pathImage.color = _originalColor;
        }

        public void EnablePath(bool isEnable)
        {
            _pathImage.enabled = isEnable;
            _pathBase.enabled = isEnable;
        }
    }
}