using System;

namespace InfinitePorker.Enums
{
    public enum HandRank
    {
        HighCard,
        Pair,
        TwoPair,
        ThreePair,
        FourPair,
        ThreeCard,
        DoubleThreeCard,
        FourCard,
        FiveCard,
        SixCard,
        SevenCard,
        EightCard,
        FiveFlush,
        SixFlush,
        SevenFlush,
        EightFlush,
        FiveStraight,
        SixStraight,
        SevenStraight,
        EightStraight
    }

    public static class HandRankExtensions
    {
        public static HandRank GetNOfAKind(int n)
        {
            return n switch
            {
                1 => HandRank.HighCard,
                2 => HandRank.Pair,
                3 => HandRank.ThreeCard,
                4 => HandRank.FourCard,
                5 => HandRank.FiveCard,
                6 => HandRank.SixCard,
                7 => HandRank.SevenCard,
                8 => HandRank.EightCard,
                _ => HandRank.HighCard
            };
        }

        public static HandRank GetNFlash(int n)
        {
            return n switch
            {
                5 => HandRank.FiveFlush,
                6 => HandRank.SixFlush,
                7 => HandRank.SevenFlush,
                8 => HandRank.EightFlush,
                _ => HandRank.HighCard
            };
        }

        public static HandRank GetNStraight(int n)
        {
            return n switch
            {
                5 => HandRank.FiveStraight,
                6 => HandRank.SixStraight,
                7 => HandRank.SevenStraight,
                8 => HandRank.EightStraight,
                _ => HandRank.HighCard
            };
        }

        public static HandRank GetNPair(int n)
        {
            return n switch
            {
                1 => HandRank.Pair,
                2 => HandRank.TwoPair,
                3 => HandRank.ThreePair,
                4 => HandRank.FourPair,
                _ => HandRank.HighCard
            };
        }
    }
}