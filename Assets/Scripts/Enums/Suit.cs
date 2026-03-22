using System;
using System.Linq;

namespace InfinitePorker.Enums
{
    public enum Suit
    {
        Spades = 1, // スペード
        Hearts = 2, // ハート
        Diamonds = 3, // ダイヤ
        Clubs = 4, // クローバー
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