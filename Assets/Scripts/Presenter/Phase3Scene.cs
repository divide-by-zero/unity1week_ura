public class Phase3Scene : PhaseSceneBase
{
    // Phase3: 自身 + 上下の隣接カードの数値合計（イカサマ封じ＝暗号が弱い）
    protected override string GetHologramText(CardView view)
    {
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

        return $"{sum}";
    }
}