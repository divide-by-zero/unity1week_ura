using UnityEngine;

public class ChapterProgressManager
{
    private const string Key = "ClearedChapter";
    private const int TotalChapters = 3;

    public int ClearedChapter
    {
        get => PlayerPrefs.GetInt(Key, 0);
        set
        {
            PlayerPrefs.SetInt(Key, Mathf.Clamp(value, 0, TotalChapters));
            PlayerPrefs.Save();
        }
    }

    public bool IsChapterUnlocked(int chapter)
    {
        if (chapter <= 1) return true;
        return ClearedChapter >= chapter - 1;
    }

    public void Reset()
    {
        PlayerPrefs.DeleteKey(Key);
        PlayerPrefs.Save();
    }
}
