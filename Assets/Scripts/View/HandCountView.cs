using TMPro;
using UnityEngine;

namespace InfinitePorker
{
    public class HandCountView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;

        private int _handMaxCount;
        private int _handCount;

        public void SetHandMaxCount(int maxCount)
        {
            _handMaxCount = maxCount;
            UpdateText();
        }

        public void SetHandCount(int count)
        {
            _handCount = count;
            UpdateText();
        }

        private void UpdateText()
        {
            _text.text = $"{_handCount:0} / {_handMaxCount:0}";
            _text.color = _handCount >= _handMaxCount ? Color.red : Color.white;
        }
    }
}