using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;

public class TalkTest : MonoBehaviour
{
    [SerializeField] private TalkScript _talkScript;
    [SerializeField] private List<TextAsset> _talkTexts;

    [SerializeField] private Button _pingButton;
    [SerializeField] private Button _pongButton;
    [SerializeField] private MultiWindowDetector _multiWindowDetector;

    private CancellationTokenSource _currentTalkCts;

    private void Start()
    {
        _pingButton.OnClickAsObservable().Subscribe(_ =>
        {
            Debug.Log("SEND PING");
            _multiWindowDetector.SendPing("Ping!");
        }).AddTo(this);
        _pongButton.OnClickAsObservable().Subscribe(_ =>
        {
            Debug.Log("SEND PONG");
            _multiWindowDetector.SendPong("Pong!");
        }).AddTo(this);
        _multiWindowDetector.PingReceived.Subscribe(s => Debug.Log("Received Ping: " + s)).AddTo(this);
        _multiWindowDetector.PongReceived.Subscribe(s => Debug.Log("Received Pong: " + s)).AddTo(this);

        CreateButtons();
    }

    private void CreateButtons()
    {
        var canvas = new GameObject("TalkTestCanvas").AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        canvas.gameObject.AddComponent<CanvasScaler>();
        canvas.gameObject.AddComponent<GraphicRaycaster>();

        var scrollObj = new GameObject("Scroll");
        scrollObj.transform.SetParent(canvas.transform, false);
        var scrollRect = scrollObj.AddComponent<ScrollRect>();
        var scrollRt = scrollObj.GetComponent<RectTransform>();
        scrollRt.anchorMin = new Vector2(0, 0);
        scrollRt.anchorMax = new Vector2(0, 1);
        scrollRt.pivot = new Vector2(0, 1);
        scrollRt.anchoredPosition = Vector2.zero;
        scrollRt.sizeDelta = new Vector2(220, 0);

        var viewport = new GameObject("Viewport");
        viewport.transform.SetParent(scrollObj.transform, false);
        var viewportRt = viewport.AddComponent<RectTransform>();
        viewport.AddComponent<RectMask2D>();
        viewportRt.anchorMin = Vector2.zero;
        viewportRt.anchorMax = Vector2.one;
        viewportRt.sizeDelta = Vector2.zero;
        viewportRt.offsetMin = Vector2.zero;
        viewportRt.offsetMax = Vector2.zero;

        var content = new GameObject("Content");
        content.transform.SetParent(viewport.transform, false);
        var contentRt = content.AddComponent<RectTransform>();
        contentRt.anchorMin = new Vector2(0, 1);
        contentRt.anchorMax = new Vector2(1, 1);
        contentRt.pivot = new Vector2(0, 1);
        contentRt.anchoredPosition = Vector2.zero;

        var vlg = content.AddComponent<VerticalLayoutGroup>();
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;
        vlg.spacing = 4;
        vlg.padding = new RectOffset(4, 4, 4, 4);

        var csf = content.AddComponent<ContentSizeFitter>();
        csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        scrollRect.content = contentRt;
        scrollRect.viewport = viewportRt;
        scrollRect.horizontal = false;

        for (int i = 0; i < _talkTexts.Count; i++)
        {
            var textAsset = _talkTexts[i];
            if (textAsset == null) continue;

            var btnObj = new GameObject(textAsset.name);
            btnObj.transform.SetParent(content.transform, false);

            var btnImage = btnObj.AddComponent<Image>();
            btnImage.color = new Color(0.15f, 0.15f, 0.15f, 0.85f);

            var le = btnObj.AddComponent<LayoutElement>();
            le.preferredHeight = 36;

            var textObj = new GameObject("Text");
            textObj.transform.SetParent(btnObj.transform, false);
            var tmp = textObj.AddComponent<TextMeshProUGUI>();
            tmp.text = textAsset.name;
            tmp.fontSize = 14;
            tmp.alignment = TextAlignmentOptions.MidlineLeft;
            tmp.color = Color.white;
            var textRt = textObj.GetComponent<RectTransform>();
            textRt.anchorMin = Vector2.zero;
            textRt.anchorMax = Vector2.one;
            textRt.offsetMin = new Vector2(8, 0);
            textRt.offsetMax = new Vector2(-4, 0);

            var btn = btnObj.AddComponent<Button>();
            var captured = textAsset;
            btn.onClick.AddListener(() => PlayTalk(captured));
        }
    }

    private void PlayTalk(TextAsset textAsset)
    {
        _currentTalkCts?.Cancel();
        _currentTalkCts?.Dispose();
        _currentTalkCts = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);
        PlayTalkAsync(textAsset, _currentTalkCts.Token).Forget();
    }

    private async UniTaskVoid PlayTalkAsync(TextAsset textAsset, CancellationToken ct)
    {
        await _talkScript.TalkSceneLoadAsync(textAsset.text, ct);
    }
}