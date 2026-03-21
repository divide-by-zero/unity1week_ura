using InfinitePorker.Enums;
using KszUtil;
using UnityEngine;

public class ChapterProgressManager
{
    private const string Key = "ClearedPhase";

    public GamePhase ClearedPhases
    {
        get => (GamePhase)PlayerPrefs.GetInt(Key, 0);
        set
        {
            PlayerPrefs.SetInt(Key, (int)value);
            PlayerPrefs.Save();
        }
    }

    public ChapterProgressManager()
    {
        if (ClearedPhases == 0)
        {
            ClearedPhases = GamePhase.Start;
        }
    }

    public void Reset()
    {
        PlayerPrefs.DeleteKey(Key);
        PlayerPrefs.Save();
    }

    public void UpdateProgress(GamePhase intro)
    {
        if (ClearedPhases < intro)
        {
            ClearedPhases = intro;
        }
    }
}