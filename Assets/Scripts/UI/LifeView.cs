using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class LifeView : MonoBehaviour
{
    [SerializeField] private TMP_Text _lifeText;
    [SerializeField] private Camera _camera;
    [SerializeField] private float _shakeDuration = 0.3f;
    [SerializeField] private float _shakeStrength = 0.3f;

    private int _maxLife;

    public void Initialize(int maxLife)
    {
        _maxLife = maxLife;
        SetLife(maxLife);
    }

    public void SetLife(int life)
    {
        var filled = new string('♥', life);
        var empty = new string('♡', _maxLife - life);
        _lifeText.text = filled + empty;
    }

    public async UniTask PlayLoseLifeAsync(CancellationToken ct)
    {
        await _camera.transform
            .DOShakePosition(_shakeDuration, _shakeStrength)
            .SetLink(gameObject)
            .ToUniTask(cancellationToken: ct);
    }
}
