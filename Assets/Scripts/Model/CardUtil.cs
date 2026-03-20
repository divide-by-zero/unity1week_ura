using System;
using System.Linq;
using InfinitePorker.Enums;
using KszUtil;
using UnityEngine;

namespace InfinitePorker.Model
{
    public static class CardUtil
    {
        public static int GetRandomRank(CardRare rare) => rare switch
        {
            CardRare.Common => UnityEngine.Random.Range(2, 5),
            CardRare.Uncommon => UnityEngine.Random.Range(5, 8),
            CardRare.Rare => UnityEngine.Random.Range(8, 12),
            CardRare.Epic => UnityEngine.Random.Range(10, 15),
            CardRare.Legendary => UnityEngine.Random.Range(12, 15),
            _ => 1
        };

        public static Direction GetRandomPath(CardRare rare) => rare switch
        {
            CardRare.Common => Direction.None,
            CardRare.Uncommon => Direction.Right,
            CardRare.Rare => GenerateRandomPaths(1),
            CardRare.Epic => GenerateRandomPaths(2),
            CardRare.Legendary => GenerateRandomPaths(3)
        };

        public static Direction GenerateRandomPaths(int cnt)
        {
            var retDirection = Direction.None;
            foreach (var direction in AllDirections().OrderBy(_ => Guid.NewGuid()).ToArray().Take(cnt))
            {
                retDirection |= direction;
            }

            return retDirection;
        }


        public static int GetPathCount(this Direction direction)
        {
            var count = 0;
            if (direction.HasFlag(Direction.Up)) count++;
            if (direction.HasFlag(Direction.Down)) count++;
            if (direction.HasFlag(Direction.Left)) count++;
            if (direction.HasFlag(Direction.Right)) count++;
            return count;
        }

        public static Direction[] AllDirections() => new[] { Direction.Up, Direction.Right, Direction.Left, Direction.Down };
        public static Suit[] AllSuits() => Enum.GetValues(typeof(Suit)).Cast<Suit>().ToArray();

        public static T Random<T>(params T[] elements) => elements.RandomAt();

        public static T Random<T>() where T : Enum => Enum.GetValues(typeof(T)).Cast<T>().RandomAt();
    }
}