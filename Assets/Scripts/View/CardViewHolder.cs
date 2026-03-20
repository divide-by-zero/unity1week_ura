using InfinitePorker.View;
using UnityEngine;

namespace InfinitePorker
{
    public class CardViewHolder : MonoBehaviour
    {
        public CardView CardView { get; private set; }

        public void Initialize(CardView cardView)
        {
            CardView = cardView;
            CardView.transform.SetParent(transform, false);
            cardView.gameObject.SetActive(false);
        }
    }
}