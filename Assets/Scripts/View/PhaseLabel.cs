using TMPro;
using UnityEngine;
using InfinitePorker.Enums;

namespace InfinitePorker.View
{
    public class PhaseLabel : MonoBehaviour
    {
        [SerializeField] private TMP_Text _label;

        public void SetText(string englishText, string japaneseText)
        {
            _label.text = $"<size=70><u>{englishText}</u></size>\n{japaneseText}";
        }
    }
}