using System;
using System.Linq;
using InfinitePorker.Enums;

namespace InfinitePorker.Model
{
    public class GameFieldModel
    {
        public const int GridWidth = 3;
        public const int GridHeight = 3;
        public const int TotalCards = GridWidth * GridHeight;

        public Card[] Cards { get; } = new Card[TotalCards];

        public GameFieldModel()
        {
            for (var i = 0; i < Cards.Length; i++)
            {
                Cards[i] = new Card();
            }
        }

        public void Initialize(Card[] cards)
        {
            if (cards.Length != TotalCards)
                throw new ArgumentException($"Expected {TotalCards} cards, got {cards.Length}");

            foreach (var (src, from) in Cards.Zip(cards, (src, from) => (src, from)))
            {
                src.CopyFrom(from);
            }
        }

        public Card GetCard(int index)
        {
            if (index < 0 || index >= TotalCards)
                return null;
            return Cards[index];
        }

        public Card GetCard(GridPosition position)
        {
            return GetCard(position.ToIndex(GridWidth));
        }

        public int GetCardIndex(Card card) => Array.FindIndex(Cards, c => c == card);

        public GridPosition GetCardPosition(Card card)
        {
            var index = GetCardIndex(card);
            return index == -1 ? default : GridPosition.FromIndex(index, GridWidth);
        }

        public bool AreAdjacent(Card card1, Card card2)
        {
            var pos1 = GetCardPosition(card1);
            var pos2 = GetCardPosition(card2);
            return pos1.IsAdjacentTo(pos2);
        }

        public Direction GetDirection(Card fromCard, Card toCard)
        {
            var fromPos = GetCardPosition(fromCard);
            var toPos = GetCardPosition(toCard);
            return fromPos.GetDirectionTo(toPos);
        }

        public void UpdateCards(Card[] newCards)
        {
            Initialize(newCards);
        }

        public void SwapCards(Card card1, Card card2)
        {
            // 一時的にcard1の内容をコピー
            var tempCard = new Card(card1);
            // card1にcard2の内容をコピー
            card1.CopyFrom(card2);
            // card2に一時保存したcard1の内容をコピー
            card2.CopyFrom(tempCard);
        }
    }
}