using System;
using InfinitePorker.Enums;
using KszUtil;
using UnityEngine;
using UnityEngine.Serialization;

namespace InfinitePorker.Logic
{
    [CreateAssetMenu(menuName = "Create PorkerSetting", fileName = "PorkerSetting", order = 0)]
    public class PorkerSetting : ScriptableObject
    {
        [field: SerializeField] public int HandSize { get; set; }
        [field: SerializeField] public PathConnectionRule PathConnectionRule { private set; get; }

        [field: SerializeField] public EnumSerializableDictionary<HandRank, HandInfo> HandInfoDictionary { private set; get; }

        [field: SerializeField] public Sprite[] CardSprites { private set; get; }
        [field: SerializeField] public Sprite[] CardMarks { private set; get; }

        [field: SerializeField] public EnumSerializableDictionary<ItemType, Sprite> ItemSprites { private set; get; }

        [field: SerializeField] public EnumSerializableDictionary<ItemType, int> ItemBasePrices { private set; get; }
        [field: SerializeField] public EnumSerializableDictionary<ItemType, float> ItemPriceMultipliers { private set; get; }
        [field: SerializeField] public EnumSerializableDictionary<ItemType, ShopItemMessage> ShopItemMessageDictionary { get; private set; }

        public HandInfo GetHandInfo(HandRank rank)
        {
            HandInfoDictionary.TryGetValue(rank, out var handInfo);
            return handInfo;
        }
    }

    [Serializable]
    public class ShopItemMessage
    {
        [Multiline(3)] public string Message;
        [Multiline(3)] public string NormalMessage;
    }

    [Serializable]
    public class HandInfo
    {
        public string JapaneseName;
        public string EnglishName;
        public float Magnitude;
    }
}