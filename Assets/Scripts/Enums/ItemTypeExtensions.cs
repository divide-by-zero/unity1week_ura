namespace InfinitePorker.Enums
{
    public static class ItemTypeExtensions
    {
        public static string ToJapaneseName(this ItemType itemType)
        {
            return itemType switch
            {
                ItemType.RankUp => "ランク\nアップ",
                ItemType.MarkChange => "マーク\nチェンジ",
                ItemType.PathReroll => "パス\n再構築",
                ItemType.CardMerge => "カード\n合成",
                ItemType.NewCardAdd => "カード\n追加",
                ItemType.HealPotion => "HP1\n回復",
                ItemType.ItemReroll => "商品\n入れ替え",
                ItemType.CardSwap => "カード\n入れ替え",
                ItemType.AddPath => "パス\n追加",
                _ => itemType.ToString()
            };
        }
    }
}