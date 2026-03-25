using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using InfinitePorker.Enums;
using InfinitePorker.Logic;
using KszUtil.Utilities;
using TextFx;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class CardGameScene : MonoBehaviour
{
    // カードの数字は 1〜9、スートは Suit enum（1〜4）
    public const int MinCardNum = 1;
    public const int MaxCardNum = 9;
    public const int SuitCount = 4;

    public struct CardData
    {
        public int Num; // 1〜9
        public Suit Suit; // Spades=1, Hearts=2, Diamonds=3, Clubs=4
    }

    [SerializeField] private GameObject _cardPrefab;
    [SerializeField] private PorkerSetting _porkerSetting;
    [SerializeField] private TMP_Text _statusText;

    [SerializeField] private int _columns = 4;
    [SerializeField] private int _rows = 4;
    [SerializeField] private float _cardSpacingX = 1.5f;
    [SerializeField] private float _cardSpacingZ = 2.0f;

    [SerializeField] private float _mismatchDelay = 1.5f;
    [SerializeField] private LifeView _lifeView;
    [SerializeField] private Image _successImage;
    [SerializeField] private TextFxTextMeshProUGUI _textFxText;

    [Inject] AudioManager _audioManager;

    private CardView[] _cardViews;
    private CardData[] _cards;
    private bool[] _isMatched;
    private int _turnCount;
    private int _matchedPairs;
    private int _totalPairs;
    private int _initialLife = 4;
    public IntReactiveProperty Life { get; } = new();

    private int _lastClickedIndex = -1;

    private readonly Subject<CardView> _onHoverEnter = new();
    private readonly Subject<CardView> _onHoverExit = new();
    public IObservable<CardView> OnHoverEnterAsObservable() => _onHoverEnter;
    public IObservable<CardView> OnHoverExitAsObservable() => _onHoverExit;

    public IDisposable Disposable { private set; get; }

    private void Start()
    {
        InitializeGrid();
        Disposable = SubscribeToAllCardClicks().AddTo(this);

        if (_lifeView != null)
        {
            _lifeView.Initialize(_initialLife);
            Life.Subscribe(life => _lifeView.SetLife(life)).AddTo(this);
        }
    }

    private void InitializeGrid()
    {
        var totalCards = _columns * _rows;
        _totalPairs = totalCards / 2;

        _cards = new CardData[totalCards];

        for (var i = 0; i < _totalPairs; i++)
        {
            var num = MinCardNum + i;
            _cards[i * 2] = new CardData { Num = num, Suit = Suit.Hearts };
            _cards[i * 2 + 1] = new CardData { Num = num, Suit = Suit.Spades };
        }

        Shuffle(_cards);

        _cardViews = new CardView[totalCards];
        _isMatched = new bool[totalCards];

        var offsetX = (_columns - 1) * _cardSpacingX * 0.5f;
        var offsetZ = (_rows - 1) * _cardSpacingZ * 0.5f;

        for (var i = 0; i < totalCards; i++)
        {
            var col = i % _columns;
            var row = i / _columns;

            var pos = new Vector3(
                col * _cardSpacingX - offsetX,
                0f,
                row * _cardSpacingZ - offsetZ
            );

            var cardObj = Instantiate(_cardPrefab, pos, Quaternion.identity, transform);
            var cardView = cardObj.GetComponent<CardView>();

            var card = _cards[i];
            cardView.CardData = card;
            cardView.SetCardSprite(_porkerSetting.CardSprites[((int)card.Suit - 1) * 13 + (card.Num - MinCardNum)]);

            _cardViews[i] = cardView;
        }

        UpdateStatusText();
    }

    public int GetCardIndex(CardView view)
    {
        return Array.IndexOf(_cardViews, view);
    }

    public bool IsMatched(CardView view)
    {
        return _isMatched[GetCardIndex(view)];
    }

    public CardView GetNeighbor(CardView view, int dx, int dz)
    {
        var index = GetCardIndex(view);
        var col = index % _columns;
        var row = index / _columns;
        var nc = col + dx;
        var nr = row + dz;
        if (nc < 0 || nc >= _columns || nr < 0 || nr >= _rows) return null;
        return _cardViews[nr * _columns + nc];
    }

    private IDisposable SubscribeToAllCardClicks()
    {
        var compositeDisposable = new CompositeDisposable();

        for (var i = 0; i < _cardViews.Length; i++)
        {
            var index = i;
            _cardViews[i].OnClickAsObservable()
                .Subscribe(_ =>
                {
                    AudioManager.Instance.Play(AudioEnum.CardSound);
                    _lastClickedIndex = index;
                })
                .AddTo(compositeDisposable);

            _cardViews[i].OnHoverEnterAsObservable()
                .Subscribe(v => _onHoverEnter.OnNext(v))
                .AddTo(compositeDisposable);

            _cardViews[i].OnHoverExitAsObservable()
                .Subscribe(v => _onHoverExit.OnNext(v))
                .AddTo(compositeDisposable);
        }

        return compositeDisposable;
    }

    /// <summary>
    /// ゲームループ。クリアなら true、ゲームオーバーなら false を返す。
    /// </summary>
    public async UniTask<bool> GameLoopAsync(CancellationToken ct)
    {
        //開始演出
        AudioManager.Instance.Play(AudioEnum.GameStart);
        _textFxText.AnimationManager.PlayAnimation();

        // CardView.Start() 完了保証
        await UniTask.Yield(ct);

        Life.Value = _initialLife;

        while (_matchedPairs < _totalPairs)
        {
            // 1枚目を待つ
            SetAllClickable(true);
            _lastClickedIndex = -1;
            await UniTask.WaitUntil(() => _lastClickedIndex >= 0, cancellationToken: ct);
            var firstIndex = _lastClickedIndex;
            _lastClickedIndex = -1;

            _cardViews[firstIndex].IsClickable = false;
            await _cardViews[firstIndex].PlayOpenAsync(ct);

            // 2枚目を待つ
            await UniTask.WaitUntil(() => _lastClickedIndex >= 0, cancellationToken: ct);
            var secondIndex = _lastClickedIndex;
            _lastClickedIndex = -1;

            SetAllClickable(false);
            await _cardViews[secondIndex].PlayOpenAsync(ct);

            _turnCount++;
            UpdateStatusText();

            // マッチ判定（数字が同じならペア成立）
            if (_cards[firstIndex].Num == _cards[secondIndex].Num)
            {
                _isMatched[firstIndex] = true;
                _isMatched[secondIndex] = true;
                _matchedPairs++;

                AudioManager.Instance.Play(AudioEnum.Success);
                _textFxText.text = $"{_matchedPairs} Hit!!";
                _textFxText.AnimationManager.PlayAnimation();
                // if (_successImage != null)
                // {
                //     UniTask.Void(async () =>
                //     {
                //         _successImage.gameObject.SetActive(true);
                //         await _successImage.DOFillAmount(1f, 0.5f).From(0f).ToUniTask(cancellationToken: ct);
                //         await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: ct);
                //         _successImage.gameObject.SetActive(false);
                //     });
                // }

                UpdateStatusText();
            }
            else
            {
                AudioManager.Instance.Play(AudioEnum.Bad);
                Life.Value--;

                if (_lifeView != null)
                {
                    await _lifeView.PlayLoseLifeAsync(ct);
                }

                if (Life.Value <= 0)
                {
                    Disposable.Dispose();
                    return false;
                }

                await UniTask.Delay(TimeSpan.FromSeconds(_mismatchDelay), cancellationToken: ct);

                // 両方同時に閉じる柴
                await UniTask.WhenAll(
                    _cardViews[firstIndex].PlayCloseAsync(ct),
                    _cardViews[secondIndex].PlayCloseAsync(ct)
                );
            }
        }

        //ちょっとDelay
        await UniTask.Delay(TimeSpan.FromSeconds(1f), cancellationToken: ct);

        // ゲームクリア
        Disposable.Dispose();
        _statusText.text = $"クリア！ {_turnCount} ターン";
        return true;
    }

    private void SetAllClickable(bool clickable)
    {
        for (var i = 0; i < _cardViews.Length; i++)
        {
            _cardViews[i].IsClickable = clickable && !_isMatched[i] && !_cardViews[i].IsOpen.Value;
        }
    }

    private void UpdateStatusText()
    {
        if (_statusText != null)
            _statusText.text = $"ターン: {_turnCount}  ペア: {_matchedPairs}/{_totalPairs}";
    }

    private static void Shuffle<T>(T[] array)
    {
        for (var i = array.Length - 1; i > 0; i--)
        {
            var j = UnityEngine.Random.Range(0, i + 1);
            (array[i], array[j]) = (array[j], array[i]);
        }
    }
}