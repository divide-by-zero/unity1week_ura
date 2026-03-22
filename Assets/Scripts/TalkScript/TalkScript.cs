using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using KszUtil;
using KszUtil.UI;
using KszUtil.Utilities;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TalkScript : MonoBehaviour
{
    private List<string> cmd = new List<string>();
    [SerializeField] private SerializableDictionary<string, Sprite> spriteDict = new SerializableDictionary<string, Sprite>();
    [SerializeField] private LongHoldButton _skipButton;
    [SerializeField] private UIFader _choicePanelFader;
    [SerializeField] private Button _choiceButtonA;
    [SerializeField] private Button _choiceButtonB;

    private int cmdIndex, mainIndex, endIndex;
    private bool isKeyWait;
    private bool isEnd; //会話終了
    private bool isSkip; //早送りモード
    private bool isChoiceWait; //選択肢待ち
    private int currentChoiceId;

    private readonly Subject<int> _onChoiceA = new Subject<int>();
    private readonly Subject<int> _onChoiceB = new Subject<int>();
    public IObservable<int> OnChoiceAAsObservable() => _onChoiceA;
    public IObservable<int> OnChoiceBAsObservable() => _onChoiceB;

    private readonly Dictionary<int, bool> _choiceResults = new();

    /// <summary>
    /// 選択結果を取得（true=A, false=B）。未選択のIDはfalseを返す。
    /// </summary>
    public bool GetChoiceResult(int choiceId) => _choiceResults.GetValueOrDefault(choiceId);

    public CustomYieldInstruction WaitTalkEnd => new WaitUntil(() => isEnd);

    [SerializeField] private UIFader _uiFader;
    public UIFader UIFader => _uiFader;

    public string message;
    public string errorMessage = "";
    private int message_cnt;
    private int totalVisibleCharacters;
    private float font_x, font_y;
    private float font_size;
    private int fontTransition;
    private int fontTransitionSpeed;

    public TMP_Text _screenText;
    public TalkCharactor[] _charactor;

    public void Start()
    {
        _skipButton.OnHoldButtonAsObservable().Subscribe(_ => ToggleSkip()).AddTo(this);

        _choiceButtonA.OnClickAsObservable().Subscribe(_ => OnChoiceSelected(true)).AddTo(this);
        _choiceButtonB.OnClickAsObservable().Subscribe(_ => OnChoiceSelected(false)).AddTo(this);
        _choicePanelFader.Show(false);
    }

    private void OnChoiceSelected(bool isA)
    {
        if (!isChoiceWait) return;
        isChoiceWait = false;
        _choicePanelFader.Show(false);
        _choiceResults[currentChoiceId] = isA;
        if (isA) _onChoiceA.OnNext(currentChoiceId);
        else _onChoiceB.OnNext(currentChoiceId);
        isKeyWait = false;
        CmdProc(cmd[cmdIndex++]);
    }

    public void ToggleSkip()
    {
        isSkip = !isSkip;
    }

    public bool CmdProc(int cmdid)
    {
        if (cmdid < 0 || cmdid >= endIndex) return false;
        CmdProc(cmd[cmdid]);
        return true;
    }

    public void CmdProc(string cm, bool isDryRun = false)
    {
        var lines = cm.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);

        int argCnt = 0;
        switch (lines[0].ToLower())
        {
            case "fontspeed":
                fontTransitionSpeed = int.Parse(lines[++argCnt]);
                return;
            case "set":
            {
                var id = int.Parse(lines[++argCnt]);
//            _charactor[id].x = Float.parseFloat(lines[++argCnt]);
//            _charactor[id].y = Float.parseFloat(lines[++argCnt]);
                return;
            }
            case "sprite":
            {
                var id = int.Parse(lines[++argCnt]);
                _charactor[id].SetSprite(spriteDict[lines[++argCnt]]);
                break;
            }
            case "talk":
            {
                int id = int.Parse(lines[++argCnt]);
                message = lines[++argCnt];
                message_cnt = 0;
                _screenText.text = message;
                _screenText.ForceMeshUpdate();
                totalVisibleCharacters = _screenText.textInfo.characterCount;
                _screenText.maxVisibleCharacters = 0;

                //会話をしているキャラだけアクティブに
                for (var i = 0; i < _charactor.Length; i++)
                {
                    _charactor[i].isTalk = id == i;
                }

                fontTransition = 0;
                isKeyWait = true;
                return;
            }
            case "sound":
                // var audioName = lines[++argCnt];
                // if (isDryRun == false)
                // {
                //     AudioManager.Instance.Play(audioName);
                // }
                //
                return;
            case "visible":
            {
                int id = int.Parse(lines[++argCnt]);
                _charactor[id].isVisible = lines[++argCnt].Equals("1");
                return;
            }
            case "choice":
            {
                currentChoiceId = int.Parse(lines[++argCnt]);
                _choiceButtonA.GetComponentInChildren<TMP_Text>().text = lines[++argCnt];
                _choiceButtonB.GetComponentInChildren<TMP_Text>().text = lines[++argCnt];
                if (isDryRun == false)
                {
                    _choicePanelFader.Show(true);
                    isChoiceWait = true;
                    isKeyWait = true;
                }

                return;
            }

            case "wait":
                isKeyWait = true;
                return;
            //アホっぽいけど、対策しておく
            case "[end]":
            case "[init]":
            case "[main]":
                return;
            default:
                //ここまで来るということは、コマンドが存在しなかった
                errorMessage += "そんなコマンドはありません:" + lines[0] + "\n";
                break;
        }
    }

    public int LoadPage(string talkScript)
    {
        int idx = 0;

        var talks = talkScript.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

        foreach (var talk in talks)
        {
            if (talk == null) break;
            var comment = talk.Split(new[] { "//" }, StringSplitOptions.None);
            var lines = comment[0].Split(new[] { "," }, StringSplitOptions.None);
            if (lines[0].Length <= 1) continue;
            cmd.Add(comment[0].Trim()); //より後ろはコメント扱い
            idx++;

            if (lines[0].Equals("[end]"))
            {
                endIndex = idx;
                break;
            }

            if (lines[0].Equals("[init]"))
            {
                idx = 0;
                continue;
            }

            if (lines[0].Equals("[main]"))
            {
                mainIndex = idx;
                continue;
            }
        }

        return idx;
    }


    public void Create(string talkScript)
    {
        cmd.Clear();
        _choiceResults.Clear();

        //デフォルト値を読み込み・実行
        //画像等を先読み実行
        LoadPage(talkScript);

        errorMessage = "";
        int i = 0;
        for (i = 0; i < endIndex; i++)
        {
            try
            {
                CmdProc(cmd[i], true);
            }
            catch (Exception e)
            {
                errorMessage += "i" + i + " :" + cmd[i] + "\n" + e.Message;
            }
        }

        if (!errorMessage.Equals(""))
        {
            //エラーメッセージが入っているようなら
            cmdIndex = endIndex - 1;
            message = "エラーです。直してください\n" + errorMessage;
            return;
        }

        cmdIndex = 0;
        LoadPage(talkScript);
        //[init]の部分を実行
        while (cmdIndex <= mainIndex)
            CmdProc(cmd[cmdIndex++]);

        isKeyWait = false;
        isEnd = false;
        isSkip = false;
        message = "";
        message_cnt = 0;
        totalVisibleCharacters = 0;
        fontTransition = 0;

        for (i = 0; i < _charactor.Length; i++)
        {
            _charactor[i].Initialize();
        }
    }

// Update is called once per frame
    void Update()
    {
        if (isSkip)
        {
            // スキップモード：テキスト即表示＋自動進行
            message_cnt = totalVisibleCharacters;

            if (isChoiceWait) return; // 選択肢待ちはスキップしない

            if (isKeyWait)
            {
                isKeyWait = false;
                CmdProc(cmd[cmdIndex++]);
            }
            else
            {
                if (CmdProc(cmdIndex))
                {
                    cmdIndex++;
                }
                else
                {
                    isEnd = true;
                    isSkip = false;
                }
            }

            return;
        }

        //フォントの一文字ずつ描画
        if (fontTransition++ > fontTransitionSpeed)
        {
            fontTransition = 0;
            if (++message_cnt > totalVisibleCharacters)
            {
                message_cnt = totalVisibleCharacters;
            }
        }

        if (message_cnt >= totalVisibleCharacters)
        {
            //トークが全て表示されていたら
            if (isKeyWait)
            {
                if (isChoiceWait == false && Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
                {
                    isKeyWait = false;
                    CmdProc(cmd[cmdIndex++]);
                }
            }
            else
            {
                if (CmdProc(cmdIndex))
                {
                    cmdIndex++;
                }
                else
                {
                    isEnd = true; //最後まで読みきりました。
                }
            }
        }
        else
        {
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                //まだメッセージが出切ってないので、全メッセージ表示
                message_cnt = totalVisibleCharacters;
            }

            //まだ会話中なので、会話しているキャラを上下させる
            foreach (var t in _charactor)
            {
                t.Talk();
            }
        }

        _screenText.maxVisibleCharacters = message_cnt;

        if (isEnd)
        {
        }
    }

    public async UniTask TalkSceneLoadAsync(string text, CancellationToken ct)
    {
        UIFader.Show(true);
        Create(text);
        await WaitTalkEnd.WithCancellation(ct);
        UIFader.Show(false);
    }

    public async UniTask TalkSceneLoadAsyncWithoutDismiss(string text, CancellationToken ct)
    {
        UIFader.Show(true);
        Create(text);
        await WaitTalkEnd.WithCancellation(ct);
    }
}