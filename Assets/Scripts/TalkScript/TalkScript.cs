using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using KszUtil.AudioManager;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TalkScript : MonoBehaviour
{
    private List<string> cmd = new List<string>();
    private int cmdIndex, mainIndex, endIndex;
    private bool isKeyWait;
    private bool isEnd; //会話終了

    public static bool IsDisp => SceneManager.GetSceneByName("Talk").isLoaded;

    public CustomYieldInstruction WaitTalkEnd => new WaitUntil(() => isEnd);

    private int wait_active_gap;

    public string message;
    public string errorMessage = "";
    int message_cnt;
    float font_x, font_y;
    float font_size;
    int fontTransition;
    int fontTransitionSpeed;

//    public Text _screenText;
    public TMP_Text _screenText;
    public string talkScriptStr;
    public TalkCharactor[] _charactor;

    // Use this for initialization
    void Start()
    {
    }

    public bool CmdProc(int cmdid)
    {
        if (cmdid < 0 || cmdid >= endIndex) return false;
        CmdProc(cmd[cmdid]);
        return true;
    }

    public void CmdProc(string cm)
    {
        var lines = cm.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);

        int argCnt = 0;
        if (lines[0].Equals("fontspeed"))
        {
            fontTransitionSpeed = int.Parse(lines[++argCnt]);
            return;
        }

        if (lines[0].Equals("wait_active_gap"))
        {
            wait_active_gap = int.Parse(lines[++argCnt]);
            return;
        }

        if (lines[0].Equals("set"))
        {
            var id = int.Parse(lines[++argCnt]);
//            _charactor[id].x = Float.parseFloat(lines[++argCnt]);
//            _charactor[id].y = Float.parseFloat(lines[++argCnt]);
            return;
        }


        if (lines[0].Equals("talk"))
        {
            int id = int.Parse(lines[++argCnt]);
            message = lines[++argCnt];
            message_cnt = 0;

            //会話をしているキャラだけアクティブに
            for (var i = 0; i < _charactor.Length; i++)
            {
                _charactor[i].isTalk = id == i;
            }

            fontTransition = 0;
            isKeyWait = true;
            return;
        }

        if (lines[0].Equals("sound"))
        {
            AudioManager.Instance.Play(lines[++argCnt]);
            return;
        }

        if (lines[0].Equals("visible"))
        {
            int id = int.Parse(lines[++argCnt]);
            _charactor[id].isVisible = lines[++argCnt].Equals("1");
            return;
        }

        if (lines[0].Equals("wait"))
        {
            isKeyWait = true;
            return;
        }

        //アホっぽいけど、対策しておく
        if (lines[0].Equals("[end]"))
        {
            return;
        }

        if (lines[0].Equals("[init]"))
        {
            return;
        }

        if (lines[0].Equals("[main]"))
        {
            return;
        }

        //ここまで来るということは、コマンドが存在しなかった
        errorMessage += "そんなコマンドはありません:" + lines[0] + "\n";
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
        //デフォルト値を読み込み・実行
        //画像等を先読み実行
        LoadPage(talkScript);

        errorMessage = "";
        int i = 0;
        for (i = 0; i < endIndex; i++)
        {
            try
            {
                CmdProc(cmd[i]);
            }
            catch (Exception e)
            {
                errorMessage += "i" + i + " :" + cmd[i] + "\n";
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
        message = "";
        message_cnt = 0;
        fontTransition = 0;

        for (i = 0; i < _charactor.Length; i++)
        {
            _charactor[i].Initialize();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //フォントの一文字ずつ描画
        if (fontTransition++ > fontTransitionSpeed)
        {
            fontTransition = 0;
            if (++message_cnt > message.Length)
            {
                message_cnt = message.Length;
            }
        }

        if (message_cnt == message.Length)
        {
            //トークが全て表示されていたら
            if (isKeyWait)
            {
                if (Input.GetMouseButtonDown(0))
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
            if (Input.GetMouseButtonDown(0))
            {
                //まだメッセージが出切ってないので、全メッセージ表示
                message_cnt = message.Length;
            }

            //まだ会話中なので、会話しているキャラを上下させる
            foreach (var t in _charactor)
            {
                t.Talk();
            }
        }

        _screenText.text = message.Substring(0, message_cnt);

        if (isEnd)
        {
        }
    }

    public static async UniTask TalkSceneLoadAsync(string text)
    {
        var ts = FindObjectOfType<TalkScript>();
        if (ts)
        {
            ts.talkScriptStr = text;
        }

        await ts.WaitTalkEnd;
    }
}