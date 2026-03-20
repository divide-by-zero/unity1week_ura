using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace InfinitePorker
{
    public class DissolveEffect : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Image _image;

        private static readonly int _timeProperty = Shader.PropertyToID("_time");

        private void Reset()
        {
            TryGetComponent(out _spriteRenderer);
            TryGetComponent(out _image);
        }

        public async UniTask DissolveAsync(float from, float to, float speed, CancellationToken ct)
        {
            var mat = new Material(Shader.Find("Shader Graphs/MyDissolveShader"));
            if (_spriteRenderer != null) _spriteRenderer.material = mat;
            if (_image != null) _image.material = mat;

            mat.SetFloat(_timeProperty, from);
            await DOVirtual.Float(from, to, speed, value => mat.SetFloat(_timeProperty, value)).ToUniTask(cancellationToken: ct);
        }
    }
}