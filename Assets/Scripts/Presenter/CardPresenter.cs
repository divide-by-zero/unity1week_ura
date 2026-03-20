using System;
using Cysharp.Threading.Tasks;
using InfinitePorker.Model;
using KszUtil.AudioManager;
using UniRx;
using Utilities;

namespace InfinitePorker.Presenter
{
    /// <summary>
    /// カードModelとViewを結びつけるPresenterクラス
    /// </summary>
    public class CardPresenter : IDisposable
    {
        private readonly Card _card;
        private readonly CardView _cardView;
        private readonly CompositeDisposable _disposables = new();

        public Card Card => _card;
        public CardView CardView => _cardView;

        public CardPresenter(Card card, CardView cardView)
        {
            _card = card;
            _cardView = cardView;

            Initialize();
        }

        private void Initialize()
        {
            // CardのプロパティをViewに反映
            Observable.CombineLatest(_card.Suit, _card.Rank, (suit, rank) => (suit, rank))
                .Subscribe(cardData => _cardView.UpdateCard(_card.RankText, (int)cardData.suit, _card.IsRedSuit, cardData.rank))
                .AddTo(_disposables);
            
            // 現在選択状態をViewに反映
            _card.IsCurrentSelected
                .Subscribe(isSelected =>
                {
                    if (isSelected)
                    {
                        AudioManager.Instance.Play(AudioEnum.Select);
                        _cardView.Select();
                    }
                    else
                    {
                        _cardView.Deselect();
                    }
                })
                .AddTo(_disposables);
        }

        public void UpdateCard(Card newCard)
        {
            _card.CopyFrom(newCard);
        }

        public void UpdateCardWithoutPath(Card newCard)
        {
            _card.SetSuit(newCard.Suit.Value);
            _card.SetRank(newCard.Rank.Value);
        }

        public void Dispose()
        {
            _disposables?.Dispose();
        }
    }
}