using System;
using System.Runtime.InteropServices;
using UniRx;
using UnityEngine;

/// <summary>
/// 2窓検出ブリッジ。
/// BroadcastChannel の ping/pong を C# 側で完全に制御する。
/// </summary>
public class MultiWindowDetector : MonoBehaviour
{
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")] private static extern void MWD_Init(string gameObjectName);
    [DllImport("__Internal")] private static extern void MWD_Ping(string message);
    [DllImport("__Internal")] private static extern void MWD_Pong(string message);
#endif

    private readonly Subject<string> _pingReceived = new();
    private readonly Subject<string> _pongReceived = new();

    /// <summary>他タブから ping を受信したときに発火する。</summary>
    public IObservable<string> PingReceived => _pingReceived;

    /// <summary>他タブから pong を受信したときに発火する。</summary>
    public IObservable<string> PongReceived => _pongReceived;

    private void Awake()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        MWD_Init(gameObject.name);
#endif
    }

    /// <summary>他タブに ping を送信する。</summary>
    public void SendPing(string message = "")
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        MWD_Ping(message);
#endif
    }

    /// <summary>他タブに pong を送信する（ping に対する応答）。</summary>
    public void SendPong(string message = "")
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        MWD_Pong(message);
#endif
    }

    // --- jslib から SendMessage で呼ばれるコールバック ---

    public void OnPingReceived(string message)
    {
        _pingReceived.OnNext(message);
    }

    public void OnPongReceived(string message)
    {
        _pongReceived.OnNext(message);
    }

    private void OnDestroy()
    {
        _pingReceived.Dispose();
        _pongReceived.Dispose();
    }
}