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

        [field: SerializeField] public Sprite[] CardSprites { private set; get; }
        [field: SerializeField] public Sprite[] CardMarks { private set; get; }
    }
}