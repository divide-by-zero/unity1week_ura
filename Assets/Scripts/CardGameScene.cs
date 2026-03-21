using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using InfinitePorker.Logic;
using KszUtil.AudioManager;
using TMPro;
using UniRx;
using UnityEngine;
using VContainer;

public class CardGameScene : MonoBehaviour
{
    [Header("参照")] [SerializeField] private GameObject _cardPrefab;
    [SerializeField] private PorkerSetting _porkerSetting;
    [SerializeField] private TMP_Text _statusText;

    [Header("グリッド設定")] [SerializeField] private int _columns = 4;
    [SerializeField] private int _rows = 4;
    [SerializeField] private float _cardSpacingX = 1.5f;
    [SerializeField] private float _cardSpacingZ = 2.0f;

    [Header("ゲーム設定")] [SerializeField] private float _mismatchDelay = 1.5f;

    [Inject] AudioManager _audioManager;

    private CardView[] _cardViews;
    private int[] _cardPairIds;
    private bool[] _isMatched;
    private int _turnCount;
    private int _matchedPairs;
    private int _totalPairs;

    private int _lastClickedIndex = -1;
    private readonly CompositeDisposable _clickSubscriptions = new();

    private void Start()
    {
        InitializeGrid();
        SubscribeToAllCardClicks();
    }

    private void OnDestroy()
    {
        _clickSubscriptions.Dispose();
    }

    private void InitializeGrid()
    {
        var totalCards = _columns * _rows;
        _totalPairs = totalCards / 2;

        _cardPairIds = new int[totalCards];
        for (var i = 0; i < _totalPairs; i++)
        {
            _cardPairIds[i * 2] = i;
            _cardPairIds[i * 2 + 1] = i;
        }

        Shuffle(_cardPairIds);

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

            var spriteIndex = _cardPairIds[i];
            cardView.SetCardSprite(_porkerSetting.CardSprites[spriteIndex]);

            _cardViews[i] = cardView;
        }

        UpdateStatusText();
    }

    private void SubscribeToAllCardClicks()
    {
        for (var i = 0; i < _cardViews.Length; i++)
        {
            var index = i;
            _cardViews[i].OnClickAsObservable()
                .Subscribe(_ => _lastClickedIndex = index)
                .AddTo(_clickSubscriptions);
        }
    }

    public async UniTask GameLoopAsync(CancellationToken ct)
    {
        // CardView.Start() 完了保証
        await UniTask.Yield(ct);

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

            // マッチ判定
            if (_cardPairIds[firstIndex] == _cardPairIds[secondIndex])
            {
                _isMatched[firstIndex] = true;
                _isMatched[secondIndex] = true;
                _matchedPairs++;

                _cardViews[firstIndex].HologramOn();
                _cardViews[secondIndex].HologramOn();
                UpdateStatusText();
            }
            else
            {
                await UniTask.Delay(TimeSpan.FromSeconds(_mismatchDelay), cancellationToken: ct);

                // 両方同時に閉じる
                await UniTask.WhenAll(
                    _cardViews[firstIndex].PlayCloseAsync(ct),
                    _cardViews[secondIndex].PlayCloseAsync(ct)
                );
            }
        }

        // ゲームクリア
        _statusText.text = $"クリア！ {_turnCount} ターン";
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