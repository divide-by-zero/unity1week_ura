using System;
using System.Linq;

namespace InfinitePorker.Enums
{
    public enum Suit
    {
        Clubs, // クローバー
        Diamonds, // ダイヤ
        Hearts, // ハート
        Spades // スペード
    }

    [System.Flags]
    public enum Direction
    {
        None = 0,
        Up = 1,
        Down = 2,
        Left = 4,
        Right = 8,
        All = Up | Down | Left | Right
    }

    public enum PathConnectionRule
    {
        BothRequired, // 両方のカードにPathが必要
        FromCardOnly, // lastSelectedCardの方向のみ必要
        EitherCard // どちらか片方にPathがあればOK
    }

    public enum CardRare
    {
        Common, // 一般的なカード
        Uncommon, // アンコモンカード
        Rare, // レアカード
        Epic, // エピックカード
        Legendary // レジェンダリーカード
    }

    public enum SkillTiming
    {
        RoundStart, // ラウンド開始時
        BeforeDragPhase, // ドラッグフェーズ前
        AfterDragPhase, // ドラッグフェーズ後
        OneCardPowerCalculation, // 1枚１枚のカードパワー計算時
        AfterCardPowerCalculation, // カードパワー計算後
        BeforeAttack, // 攻撃前
        AfterAttack, // 攻撃後
    }
}