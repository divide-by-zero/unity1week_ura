using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using InfinitePorker.Enums;
using KszUtil;
using KszUtil.SceneManager;
using TMPro;
using UniRx;
using UnityEngine;

public class Phase3Scene : PhaseSceneBase
{
    [SerializeField] private MultiWindowDetector _multiWindowDetector;
    [SerializeField] private TextAsset _backdoorSuccessText;
    [SerializeField] private TextAsset _backdoorSuccessButGameOverText;
    [SerializeField] private TextAsset _backdoorNotSuccessButGameClearText;
    [SerializeField] private TMP_Text _hologramText;
    [SerializeField] private TMP_Text _hintText;

    private bool _isHologramOn = false;

    protected override async UniTask SequenceTask(CancellationToken ct)
    {
        _hintText.gameObject.SetActive(false);

        await UniTask.Yield();

        var isBackdoorSuccess = false;

        //特殊コンティニューの場合
        if (_chapterProgress.IsSpecialContinue)
        {
            isBackdoorSuccess = true;
            //ホログラムは見えるようにする
            VisibleHologram();
            //バックドア成功から開始
            await _talkScript.TalkSceneLoadAsync(_backdoorSuccessText.text, ct);
        }
        else
        {
            await _talkScript.TalkSceneLoadAsync(_talkText.text, ct);
            var choiceResult = _talkScript.GetChoiceResult(0);
            //信じるを選んだ場合
            if (choiceResult == false)
            {
                //待機開始
                _multiWindowDetector.PingReceived.Subscribe(s =>
                {
                    Debug.Log(s);
                    _multiWindowDetector.SendPing("信じるわよアキラ！");
                }).AddTo(this);

                //BackDoor が終わるのを待つ BackDoor側は Pong でメッセージを送る
                var pongMessage = await _multiWindowDetector.PongReceived.ToUniTask(true, ct);
                Debug.Log(pongMessage);

                isBackdoorSuccess = true;

                //ホログラムが見えるようになる
                VisibleHologram();

                //再度BackDoorに行かなくても良いように進行を保存しておいて、タイトルから直接ここに来れるようにする
                _chapterProgress.UpdateProgress(GamePhase.Phase3Continue);

                await _talkScript.TalkSceneLoadAsync(_backdoorSuccessText.text, ct);
            }
        }

        var cleared = await CardGameScene.GameLoopAsync(ct);

        if (cleared)
        {
            if (isBackdoorSuccess)
            {
                KszSceneManager.Instance.LoadAsync(_nextScene).Forget();
            }
            else
            {
                //クリアしてるけど、アキラが不幸なバッドエンド
                await _talkScript.TalkSceneLoadAsync(_backdoorNotSuccessButGameClearText.text, ct);
            }
        }
        else
        {
            if (isBackdoorSuccess)
            {
                //バックドア成功してるけど、ゲームオーバー
                await _talkScript.TalkSceneLoadAsync(_backdoorSuccessButGameOverText.text, ct);
            }
            else
            {
                await OnGameOverAsync(ct);
            }

            KszSceneManager.Instance.LoadAsync("Title").Forget();
        }
    }

    private void VisibleHologram()
    {
        _isHologramOn = true;
        _hintText.gameObject.SetActive(true);
    }


    // Phase3: 自身 + 上下の隣接カードの数値合計（イカサマ封じ＝暗号が弱い）
    protected override string GetHologramText(CardView view)
    {
        return null;
    }

    protected override void Hologram(CardView view, bool isOn)
    {
        if (_isHologramOn == false) return;
        if (view == null)
        {
            _hologramText.text = "";
            return;
        }


        var sum = view.CardData.Num;

        // 上, 下のみ
        int[] dz = { -1, 1 };

        for (var i = 0; i < dz.Length; i++)
        {
            var neighbor = CardGameScene.GetNeighbor(view, 0, dz[i]);
            if (neighbor != null)
            {
                sum += neighbor.CardData.Num;
            }
        }

        _hologramText.text = $"{sum}";
    }
}