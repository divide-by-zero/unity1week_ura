using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace KszUtil.UI
{
    public class LongHoldButton : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private float _duration;
        private FloatReactiveProperty _holdTime = new FloatReactiveProperty(0);

        public IObservable<float> HoldRatioAsObservable() => _holdTime.Select(f => Mathf.Clamp01(f / _duration));

        //_duration秒以上ボタンを押し続けたら発火
        public IObservable<Unit> OnHoldButtonAsObservable() => _holdTime.Where(f => f > _duration).AsUnitObservable();

        //ボタン押下している間 _holdTimeを更新, ボタンを離したら0に戻す
        void Start()
        {
            _button.OnPointerDownAsObservable()
                .SelectMany(_ => Observable.EveryUpdate().TakeUntil(_button.OnPointerUpAsObservable()))
                .Subscribe(_ => _holdTime.Value += Time.deltaTime);

            _button.OnPointerUpAsObservable()
                .Subscribe(_ => _holdTime.Value = 0);
        }
    }
}