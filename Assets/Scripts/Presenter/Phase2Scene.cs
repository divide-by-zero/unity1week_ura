using InfinitePorker.Enums;

public class Phase2Scene : PhaseSceneBase
{
    // ホログラム表示: 数字にスート倍率をかける（♠×2, ♥×3）
    protected override string GetHologramText(CardView view)
    {
        var data = view.CardData;

        return data.Suit switch
        {
            Suit.Spades => $"{data.Num * 2}",
            Suit.Hearts => $"{data.Num * 3}",
            _ => "-"
        };
    }
}