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

    [Flags]
    public enum GamePhase
    {
        None = 0,
        Start = 1 << 1,
        Intro = 1 << 2,
        Phase1 = 1 << 3,
        Phase2 = 1 << 4,
        Phase3 = 1 << 5,
        BackDoor = 1 << 6,
    }
}