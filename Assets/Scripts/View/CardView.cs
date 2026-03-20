using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using InfinitePorker.Enums;
using InfinitePorker.Logic;
using KszUtil;
using KszUtil.AudioManager;
using KszUtil.Timeline;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utilities;

namespace InfinitePorker.View
{
    public class CardView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
    {
        [SerializeField] private TMP_Text[] _label;
        [SerializeField] private Image[] _mark;
        [SerializeField] private Image[] _cardImages;
        [SerializeField] private Image _cardImage;

        [SerializeField] private Transform[] _fromPathPositions;
        [SerializeField] private Transform[] _toPathPositions;
        [SerializeField] private Color[] _colors;
        [SerializeField] private Image _cardFrame;
        [SerializeField] private MarkPatternView[] _markParents;
        [SerializeField] private Transform _cardBase;
        [SerializeField] private PorkerSetting _porkerSetting;

        [SerializeField] private PlayableDirectorPlayer _toBackTimeline;
        [SerializeField] private PlayableDirectorPlayer _toFrontTimeline;

        private readonly Subject<Unit> _onCardSelectSubject = new();
        private readonly Subject<Unit> _onCardMouseOverSubject = new();

        public IObservable<Unit> OnCardSelectAsObservable() => _onCardSelectSubject.AsObservable();
        public IObservable<Unit> OnCardMouseOverAsObservable() => _onCardMouseOverSubject.AsObservable();

        private CancellationTokenSource _animationCts;

        /// <summary>
        /// Sprite差し替えVer
        /// </summary>
        /// <param name="cardDataSuit"></param>
        /// <param name="cardDataRank"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void UpdateCard(Suit cardDataSuit, int cardDataRank)
        {
            var index = (int)cardDataSuit * 14 + cardDataRank;
            _cardImage.sprite = _porkerSetting.CardSprites[index];
        }

        public void UpdateCard(string rankText, int suit, bool isRedSuit, int rank)
        {
            var color = isRedSuit ? _colors[1] : _colors[0];
            var markSprite = _porkerSetting.CardMarks[suit];
            foreach (var label in _label)
            {
                label.text = rankText;
                label.color = color;
            }

            foreach (var mark in _mark)
            {
                mark.sprite = markSprite;
                mark.color = color;
            }

            var ranks = VisibleRankMark(rank);
            if (ranks != null)
            {
                ranks.SetMark(markSprite);
                ranks.SetColor(color);

                foreach (var cardImage in _cardImages)
                {
                    cardImage.color = color;
                }
            }
        }

        private MarkPatternView VisibleRankMark(int rank)
        {
            // 0-12の範囲でマークを表示
            for (var i = 0; i < _markParents.Length; i++)
            {
                if (_markParents[i] == null) continue;
                _markParents[i].gameObject.SetActive(i == rank);
            }

            return _markParents.ElementAtOrDefault(rank);
        }

        public Vector3 GetFromPathViewPos(Direction direction) => direction switch
        {
            Direction.Up => _fromPathPositions[0].transform.position,
            Direction.Down => _fromPathPositions[1].transform.position,
            Direction.Left => _fromPathPositions[2].transform.position,
            Direction.Right => _fromPathPositions[3].transform.position,
        };

        public Vector3 GetToPathViewPos(Direction direction) => direction switch
        {
            Direction.Up => _toPathPositions[0].transform.position,
            Direction.Down => _toPathPositions[1].transform.position,
            Direction.Left => _toPathPositions[2].transform.position,
            Direction.Right => _toPathPositions[3].transform.position,
        };

        public async UniTask CurrentSelectAsync(CancellationToken ct)
        {
            _animationCts?.Cancel();
            _animationCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            await (
                _cardBase.DOScale(1.2f, 0.3f).ToUniTask(cancellationToken: _animationCts.Token).SuppressCancellationThrow(),
                _cardBase.ToRectTransform().DOAnchorPosY(15, 0.3f).ToUniTask(cancellationToken: _animationCts.Token).SuppressCancellationThrow(),
                _cardFrame.DOColor(Color.red, 0.2f).ToUniTask(cancellationToken: _animationCts.Token).SuppressCancellationThrow()
            );
            ct.ThrowIfCancellationRequested();
        }

        public async UniTask CurrentDeSelectAsync(CancellationToken ct)
        {
            _animationCts?.Cancel();
            _animationCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            await (
                _cardBase.DOScale(1.0f, 0.3f).ToUniTask(cancellationToken: _animationCts.Token),
                _cardBase.ToRectTransform().DOAnchorPosY(0, 0.3f).ToUniTask(cancellationToken: _animationCts.Token),
                _cardFrame.DOColor(Color.black, 0.2f).ToUniTask(cancellationToken: _animationCts.Token)
            );
            ct.ThrowIfCancellationRequested();
        }

        public async UniTask MoveFromPositionAsync(Vector3 fromPosition, CancellationToken ct)
        {
            Debug.Log($"PlaySwapFromPositionAsync 開始: {gameObject.name}");
            // 一瞬で相手の位置に移動
            transform.position = fromPosition;

            // 相手の位置から本来の位置にスライドしながら戻る
            _animationCts?.Cancel();
            _animationCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            await (
                transform.ToRectTransform().DOAnchorPos(Vector2.zero, 0.5f).SetEase(Ease.OutQuart).ToUniTask(cancellationToken: _animationCts.Token),
                transform.DOScale(1.1f, 0.25f).SetLoops(2, LoopType.Yoyo).ToUniTask(cancellationToken: _animationCts.Token),
                _cardFrame.DOColor(Color.yellow, 0.25f).SetLoops(2, LoopType.Yoyo).ToUniTask(cancellationToken: _animationCts.Token)
            );
            ct.ThrowIfCancellationRequested();

            Debug.Log($"PlaySwapFromPositionAsync 完了: {gameObject.name}");
        }

        public void MoveGravePosition(Vector3 gravePos)
        {
            // 一瞬で墓地の位置に移動
            transform.position = gravePos;
        }

        public async UniTask PlayMoveGraveAnimationAsync(Vector3 gravePos, float duration, float delay, CancellationToken ct)
        {
            _animationCts?.Cancel();
            _animationCts = CancellationTokenSource.CreateLinkedTokenSource(ct);

            await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: _animationCts.Token).SuppressCancellationThrow();
            AudioManager.Instance.Play(AudioEnum.CardSound);
            await transform.DOMove(gravePos, duration).ToUniTask(cancellationToken: _animationCts.Token).SuppressCancellationThrow();
            ct.ThrowIfCancellationRequested();
        }

        public async UniTask PlayBackAnimationAsync(float duration, float delay, CancellationToken ct)
        {
            _animationCts?.Cancel();
            _animationCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: _animationCts.Token).SuppressCancellationThrow();
            AudioManager.Instance.Play(AudioEnum.CardSound);
            await transform.ToRectTransform().DOAnchorPos(Vector2.zero, duration).ToUniTask(cancellationToken: _animationCts.Token).SuppressCancellationThrow();
            ct.ThrowIfCancellationRequested();
        }

        public async UniTask ToFrontAsync(CancellationToken ct)
        {
            _toFrontTimeline.Play();
            await _toFrontTimeline.OnCompleteAsObservable().ToUniTask(true, ct);
        }

        public async UniTask ToBackAsync(CancellationToken ct)
        {
            _toBackTimeline.Play();
            await _toBackTimeline.OnCompleteAsObservable().ToUniTask(true, ct);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (Input.GetMouseButton(0))
            {
                _onCardSelectSubject.OnNext(Unit.Default);
            }
            else
            {
                _onCardMouseOverSubject.OnNext(Unit.Default);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            // DeSelectAsync(destroyCancellationToken).Forget();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            // 左クリックのみ選択
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                _onCardSelectSubject.OnNext(Unit.Default);
            }
        }
    }
}