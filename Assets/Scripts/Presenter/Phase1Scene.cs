using InfinitePorker.Enums;

public class Phase1Scene : PhaseSceneBase
{
    // ホログラム表示: 数字 × スート値
    protected override string GetHologramText(CardView view)
    {
        var data = view.CardData;

        return data.Suit switch
        {
            Suit.Spades => $"{data.Num}",
            Suit.Hearts => $"{20 - data.Num}",
            _ => "-"
        };
    }
}