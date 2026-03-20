using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using KszUtil;
using UnityEngine;

namespace InfinitePorker
{
    public class BattleCharacter : MonoBehaviour
    {
        [SerializeField] private RectTransform _attackPoint;
        [SerializeField] private DissolveEffect _dissolveEffect;

        private Vector3 _basePosition;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _basePosition = transform.position;
        }

        public async UniTask AttackAsync(BattleCharacter targetCharacter, CancellationToken ct)
        {
            await transform.ToRectTransform().DOAnchorPosX(_attackPoint.anchoredPosition.x, 0.5f).ToUniTask(cancellationToken: ct);
        }

        public async UniTask BackAsync(CancellationToken ct)
        {
            await transform.DOMove(_basePosition, 0.5f).ToUniTask(cancellationToken: ct);
        }

        public async UniTask DamageAsync(CancellationToken ct)
        {
            await transform.DOShakePosition(0.5f, new Vector3(20f, 20f, 0), vibrato: 10, randomness: 90).ToUniTask(cancellationToken: ct);
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(2))
            {
                DeadAsync(destroyCancellationToken);
            }
        }

        public async UniTask DeadAsync(CancellationToken ct)
        {
            await (
                _dissolveEffect.DissolveAsync(1.0f, 0, 3.0f, ct),
                transform.DOShakePosition(3.0f, 50).ToUniTask(cancellationToken: ct)
            );
            gameObject.SetActive(false);
        }
    }
}