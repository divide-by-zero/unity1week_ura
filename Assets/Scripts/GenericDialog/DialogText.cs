using KszUtil;
using UnityEngine;
using UnityEngine.UI;

public class DialogText
{
    public Color? BackGroundColor { private set; get; }
    public Color? ForeGroundColor { private set; get; }
    public string Text { private set; get; }

    public DialogText(Color backGroundColor, Color forGroundColor, string text)
    {
        BackGroundColor = backGroundColor;
        ForeGroundColor = forGroundColor;
        Text = text;
    }

    public DialogText(string text)
    {
        Text = text;
    }

    public static DialogText Create(string text)
    {
        return new DialogText(text);
    }

    public static DialogText Create(Color color, string text)
    {
        return new DialogText(color, color.GetEnableColor(), text);
    }

    public static DialogText Create(string text, Color color)
    {
        return new DialogText(color, color.GetEnableColor(), text);
    }
}