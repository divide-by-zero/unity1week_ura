using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InfinitePorker
{
    public class MarkPatternView : MonoBehaviour
    {
        [SerializeField] private List<Image> _markImages;
        [SerializeField] private List<Image> _otherImages;

        public void SetMark(Sprite markSprite)
        {
            foreach (var markImage in _markImages)
            {
                markImage.sprite = markSprite;
            }
        }

        public void SetColor(Color color)
        {
            foreach (var markImage in _markImages)
            {
                markImage.color = color;
            }

            foreach (var otherImage in _otherImages)
            {
                otherImage.color = color;
            }
        }
    }
}