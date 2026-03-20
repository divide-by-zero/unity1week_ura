using System;
using System.Linq;
using InfinitePorker.Enums;
using KszUtil;
using UniRx;

namespace InfinitePorker.Model
{
    public class Card
    {
        private readonly ReactiveProperty<Suit> _suit = new();
        private readonly ReactiveProperty<int> _rank = new();
        private readonly ReactiveProperty<Direction> _paths = new();
        private readonly ReactiveProperty<Direction> _usedPaths = new();
        private readonly ReactiveProperty<bool> _isCurrentSelected = new();

        public IReadOnlyReactiveProperty<Suit> Suit => _suit;
        public IReadOnlyReactiveProperty<int> Rank => _rank;
        public IReadOnlyReactiveProperty<Direction> Paths => _paths;
        public IReadOnlyReactiveProperty<Direction> UsedPaths => _usedPaths;
        public IReadOnlyReactiveProperty<bool> IsCurrentSelected => _isCurrentSelected;

        private readonly Subject<Unit> _onSelectErrorSubject = new();
        public IObservable<Unit> OnSelectErrorAsObservable() => _onSelectErrorSubject;


        public int AceLowerRank => _rank.Value == 14 ? 1 : _rank.Value;
        public int AceHighRank => _rank.Value;

        public string SuitText => _suit.Value switch
        {
            Enums.Suit.Spades => "♠",
            Enums.Suit.Hearts => "♥",
            Enums.Suit.Diamonds => "♦",
            Enums.Suit.Clubs => "♣",
            _ => ""
        };

        public bool IsRedSuit => _suit.Value == Enums.Suit.Hearts || _suit.Value == Enums.Suit.Diamonds;

        public string RankText => _rank.Value switch
        {
            11 => "J",
            12 => "Q",
            13 => "K",
            14 => "A",
            _ => _rank.Value.ToString()
        };

        public Card(Suit suit, int rank, Direction paths = Direction.None)
        {
            _suit.Value = suit;
            _rank.Value = rank;
            _paths.Value = paths;
            _usedPaths.Value = Direction.None;
            _isCurrentSelected.Value = false;
        }

        // コピーコンストラクタ
        public Card(Card other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));

            // 基本的な初期化
            _suit = new ReactiveProperty<Suit>();
            _rank = new ReactiveProperty<int>();
            _paths = new ReactiveProperty<Direction>();
            _usedPaths = new ReactiveProperty<Direction>();
            _isCurrentSelected = new ReactiveProperty<bool>();

            // 他のカードから内容をコピー
            CopyFrom(other);
            _isCurrentSelected.Value = other._isCurrentSelected.Value; // 選択状態もコピー
        }

        public Card()
        {
        }

        public void SetSuit(Suit suit)
        {
            _suit.Value = suit;
        }

        public void SetRank(int rank)
        {
            _rank.Value = rank;
        }

        public void SetPaths(Direction paths)
        {
            _paths.Value = paths;
            _usedPaths.Value = Direction.None; // 使用済みパスはリセット
        }

        // 他のCardの内容をこのCardにコピー
        public void CopyFrom(Card other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));

            _suit.Value = other._suit.Value;
            _rank.Value = other._rank.Value;
            _paths.Value = other._paths.Value;
            _usedPaths.Value = other._usedPaths.Value;
            // _isCurrentSelectedは意図的にコピーしない（選択状態は個別管理）
        }

        public bool HasPath(Direction direction)
        {
            return (_paths.Value & direction) != 0;
        }

        public bool IsPathUsed(Direction direction)
        {
            return (_usedPaths.Value & direction) != 0;
        }

        public bool HasAvailablePath(Direction direction)
        {
            return HasPath(direction) && !IsPathUsed(direction);
        }

        public void UsePath(Direction direction)
        {
            if (HasPath(direction))
            {
                _usedPaths.Value |= direction;
            }
        }

        public void ClearUsedPaths()
        {
            _usedPaths.Value = Direction.None;
        }

        public void UnusePath(Direction direction)
        {
            if (IsPathUsed(direction))
            {
                _usedPaths.Value &= ~direction;
            }
        }

        public string DisplayText => $"{SuitText}{RankText}";

        public void SetCurrentSelected(bool isSelected)
        {
            _isCurrentSelected.SetValueAndForceNotify(isSelected);
        }

        public bool CanConnectTo(Card otherCard, Direction direction)
        {
            Direction oppositeDirection = direction switch
            {
                Direction.Up => Direction.Down,
                Direction.Down => Direction.Up,
                Direction.Left => Direction.Right,
                Direction.Right => Direction.Left,
                _ => Direction.None
            };

            return HasAvailablePath(direction) && otherCard.HasAvailablePath(oppositeDirection);
        }

        public override string ToString()
        {
            return $"{_suit.Value.ToString()[0]}{RankText}"; // 例: S10, HA
        }

        public override bool Equals(object obj)
        {
            if (obj is Card other)
            {
                return _suit.Value == other._suit.Value &&
                       _rank.Value == other._rank.Value &&
                       _paths.Value == other._paths.Value;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_suit.Value, _rank.Value, _paths.Value);
        }

        public bool ContentEquals(Card other)
        {
            if (other == null) return false;
            return _suit.Value == other._suit.Value &&
                   _rank.Value == other._rank.Value &&
                   _paths.Value == other._paths.Value;
        }

        public void RankUp(int num)
        {
            _rank.Value += num;
            if (_rank.Value > 14)
            {
                _rank.Value = _rank.Value - 14 + 1; // 14を超えたら2に戻る
                _usedPaths.Value = Direction.None; // 使用済みパスはリセット
            }
        }

        public void PathReroll()
        {
            // ランダムなパスを生成して設定
            // 有効なパスの数を取得
            var pathCount = CardUtil.GetPathCount(_paths.Value);
            var oldPath = _paths.Value;

            if (pathCount != 4 || pathCount != 0)
            {
                do
                {
                    _paths.Value = CardUtil.GenerateRandomPaths(pathCount);
                } while (_paths.Value == oldPath);
            }

            _usedPaths.Value = Direction.None; // 使用済みパスはリセット
        }

        public void MarkChange()
        {
            var newSuit = CardUtil.AllSuits().Except(new[] { _suit.Value }).RandomAt();
            _suit.Value = newSuit;
        }

        public void AddPath()
        {
            // 既存のPathを除いた、追加可能なPathをリストアップ
            var availableDirections = CardUtil.AllDirections().Where(dir => !_paths.Value.HasFlag(dir)).ToArray();

            // 追加可能なPathがない場合は何もしない
            if (availableDirections.Length == 0) return;

            // ランダムに一つのPathを選択して追加
            var newPath = availableDirections.RandomAt();
            _paths.Value |= newPath;

            _usedPaths.Value = Direction.None; // 使用済みパスはリセット
        }

        public void SelectError()
        {
            _onSelectErrorSubject.OnNext(Unit.Default);
        }
    }
}

// --- 共通の定義 ---