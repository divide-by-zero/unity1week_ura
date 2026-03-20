using System;
using System.Linq;
using InfinitePorker.Model;
using UniRx;
using UnityEngine;

namespace InfinitePorker.Manager
{
    /// <summary>
    /// 汎用的なカード選択管理クラス
    /// 選択枚数上限や特殊な選択ルールに対応
    /// </summary>
    public class GenericSelectCardHolder
    {
        private readonly ReactiveCollection<Card> _selectedCards = new();

        public IReadOnlyReactiveCollection<Card> SelectedCards => _selectedCards;
        public int Count => _selectedCards.Count;
        public int MaxSelectionCount { get; private set; } = 2;
        public Card LastSelectedCard => _selectedCards.LastOrDefault();

        /// <summary>
        /// 選択枚数上限を設定
        /// </summary>
        public GenericSelectCardHolder(int maxSelectionCount)
        {
            MaxSelectionCount = maxSelectionCount;
            Clear();
        }

        /// <summary>
        /// カードを選択/選択解除
        /// </summary>
        public void AddCard(Card card)
        {
            // 既に選択済みの場合は選択解除
            if (_selectedCards.Contains(card))
            {
                RemoveCard(card);
                return;
            }

            // 特殊処理：選択枚数上限が1枚の場合、他を選択すると既選択カードを非選択
            if (MaxSelectionCount == 1 && _selectedCards.Count >= 1)
            {
                var previousCard = _selectedCards[0];
                previousCard.SetCurrentSelected(false);
                _selectedCards.Clear();
                Debug.Log($"前の選択を解除: {previousCard.DisplayText}");
            }

            // 上限チェック
            if (_selectedCards.Count >= MaxSelectionCount)
            {
                Debug.Log($"選択上限に達しています ({_selectedCards.Count}/{MaxSelectionCount})");
                return;
            }

            // カードを追加
            _selectedCards.Add(card);
            card.SetCurrentSelected(true);

            Debug.Log($"カード選択: {card.DisplayText} ({_selectedCards.Count}/{MaxSelectionCount})");

            // 選択完了時のメッセージ
            if (_selectedCards.Count == MaxSelectionCount)
            {
                Debug.Log($"✓ {MaxSelectionCount}枚選択完了！");
            }
        }

        /// <summary>
        /// 指定したカードを選択から除外
        /// </summary>
        public void RemoveCard(Card card)
        {
            if (_selectedCards.Remove(card))
            {
                card.SetCurrentSelected(false);
            }
        }

        /// <summary>
        /// 最後に選択したカードを削除
        /// </summary>
        public void RemoveLastCard()
        {
            if (_selectedCards.Count > 0)
            {
                var lastIndex = _selectedCards.Count - 1;
                var removedCard = _selectedCards[lastIndex];

                removedCard.SetCurrentSelected(false);
                _selectedCards.RemoveAt(lastIndex);
            }
        }

        /// <summary>
        /// 全選択をクリア
        /// </summary>
        public void Clear()
        {
            // 全ての選択状態をクリア
            ClearSelectedCards();
            _selectedCards.Clear();
        }

        /// <summary>
        /// 保持している選択状態で、CardモデルのSetCurrentSelectedを呼び出す
        /// </summary>
        public void SelectedCardsApply()
        {
            foreach (var card in _selectedCards)
            {
                card.SetCurrentSelected(true);
            }

            var cards = _selectedCards.ToList();
            _selectedCards.Clear();
            foreach (var card in cards)
            {
                _selectedCards.Add(card);
            }
        }

        public void ClearSelectedCards()
        {
            // 全ての選択状態をクリア
            foreach (var card in _selectedCards)
            {
                card.SetCurrentSelected(false);
            }
        }

        /// <summary>
        /// 選択が完了しているかチェック
        /// </summary>
        public bool IsSelectionComplete()
        {
            return _selectedCards.Count == MaxSelectionCount;
        }
    }

    public interface ICardSelector
    {
        IObservable<Card> OnCardSelectAsObservable();
    }

    public interface ICardSelectUndo
    {
        IObservable<Unit> OnCardSelectUndoAsObservable();
        void SetInteractable(bool interactable);
    }
}