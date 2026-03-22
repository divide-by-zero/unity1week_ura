using InfinitePorker.Logic;
using UnityEngine;

public class TitleCardRain : MonoBehaviour
{
    [SerializeField] private GameObject _cardPrefab;
    [SerializeField] private PorkerSetting _porkerSetting;
    [SerializeField] private int _cardCount = 20;

    [Header("スポーン範囲")] [SerializeField] private float _spawnRangeX = 15f;
    [SerializeField] private float _spawnMinY = 15f;
    [SerializeField] private float _spawnMaxY = 25f;
    [SerializeField] private float _spawnZ = 0f;

    [Header("落下設定")] [SerializeField] private float _fallSpeedMin = 3f;
    [SerializeField] private float _fallSpeedMax = 6f;
    [SerializeField] private float _spinSpeedMin = 90f;
    [SerializeField] private float _spinSpeedMax = 360f;
    [SerializeField] private float _despawnY = -10f;

    private Transform[] _cards;
    private Vector3[] _spinAxes;
    private float[] _fallSpeeds;
    private float[] _spinSpeeds;

    private void Start()
    {
        _cards = new Transform[_cardCount];
        _spinAxes = new Vector3[_cardCount];
        _fallSpeeds = new float[_cardCount];
        _spinSpeeds = new float[_cardCount];

        for (var i = 0; i < _cardCount; i++)
        {
            var cardObj = Instantiate(_cardPrefab, transform);
            var cardView = cardObj.GetComponent<CardView>();
            cardView.IsClickable = false;
            AssignRandomSprite(cardView);

            _cards[i] = cardObj.transform;
            ResetCard(i, true);
        }
    }

    private void Update()
    {
        for (var i = 0; i < _cards.Length; i++)
        {
            var t = _cards[i];
            t.position += Vector3.down * (_fallSpeeds[i] * Time.deltaTime);
            t.Rotate(_spinAxes[i], _spinSpeeds[i] * Time.deltaTime, Space.World);

            if (t.position.y < _despawnY)
            {
                var cardView = t.GetComponent<CardView>();
                AssignRandomSprite(cardView);
                ResetCard(i, false);
            }
        }
    }

    private void ResetCard(int index, bool randomY)
    {
        var x = Random.Range(-_spawnRangeX, _spawnRangeX);
        var y = randomY
            ? Random.Range(_despawnY, _spawnMaxY)
            : Random.Range(_spawnMinY, _spawnMaxY);

        _cards[index].position = new Vector3(x, y, _spawnZ);
        _cards[index].rotation = Random.rotation;
        _spinAxes[index] = Random.onUnitSphere;
        _fallSpeeds[index] = Random.Range(_fallSpeedMin, _fallSpeedMax);
        _spinSpeeds[index] = Random.Range(_spinSpeedMin, _spinSpeedMax);
    }

    private void AssignRandomSprite(CardView cardView)
    {
        var suit = Random.Range(0, 2);
        var num = Random.Range(0, 9);
        var spriteIndex = suit * 14 + num;
        cardView.SetCardSprite(_porkerSetting.CardSprites[spriteIndex]);
    }
}