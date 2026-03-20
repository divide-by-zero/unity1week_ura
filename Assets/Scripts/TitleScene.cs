using UniRx;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TitleScene : MonoBehaviour
{
    [SerializeField] private MultiWindowDetector _multiWindowDetector;
    [SerializeField] private Button _toggleButton;
    [SerializeField] private TMP_Text _statusText;

    private readonly ReactiveProperty<bool> _respondToPing = new(false);

    private void Start()
    {
        // ボタンで応答フラグを Toggle
        _toggleButton.OnClickAsObservable()
            .Subscribe(_ => _respondToPing.Value = !_respondToPing.Value)
            .AddTo(this);

        // フラグ状態を TMP に表示
        _respondToPing
            .Subscribe(on => _statusText.text = on ? "応答: ON" : "応答: OFF")
            .AddTo(this);

        // フラグ ON の時だけ ping に pong を返す
        _multiWindowDetector.PingReceived
            .Where(_ => _respondToPing.Value)
            .Subscribe(msg => _multiWindowDetector.SendPong($"ping {msg} callback Hello World!"))
            .AddTo(this);

        // pong を受信したら TMP に表示
        _multiWindowDetector.PongReceived
            .Subscribe(msg => _statusText.text += $"\n2窓を検出しました！ {msg}")
            .AddTo(this);

        // 起動時に ping を送る
        _multiWindowDetector.SendPing("what's up?");
    }
}




