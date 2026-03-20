using UnityEngine;
using UnityEngine.UI;

public class TalkCharactor : MonoBehaviour
{
    public Vector2 waitpos;
    public Vector2 activePos;
    public bool isTalk;
    public bool isVisible;


    public Image _charaImage;

    private bool isTalkEnd;
    private float talk_y, talk_target_y;
    private float fadeSpeed = 10;

    private RectTransform _rect;
    private RectTransform RectTransform
    {
        get { return _rect == null ? (_rect = GetComponent<RectTransform>()) : _rect; }
    }

    // Use this for initialization
    void Start () {
		
	}

    public void Initialize()
    {
        isTalk = false;
        isTalkEnd = true;
        RectTransform.anchoredPosition = waitpos;
    }

    public void Talk()
    {
        isTalkEnd = false;
    }

	// Update is called once per frame
	void Update () {
	    if (isVisible == false) return;
	    var pos = RectTransform.anchoredPosition;
	    if (isTalk)
	    {
	        pos += (activePos - pos) / fadeSpeed;
	    }
	    else
	    {
	        pos += (waitpos - pos) / fadeSpeed;
	    }

	    if (isTalk && isTalkEnd == false)
	    {
	        if (talk_target_y > 0)
	        {
	            if (talk_y > 15)
	            {
	                talk_target_y = -20;
	            }
	        }
            else if (talk_target_y < 0)
	        {
	            if (talk_y < -15)
	            {
	                talk_target_y = 20;
	            }
            }
	        else
	        {
	            talk_target_y = 20;
	        }

        }
	    else
	    {
	        talk_target_y = 0;
	    }
	    isTalkEnd = true;
	    talk_y += (talk_target_y - talk_y) / fadeSpeed;

	    RectTransform.anchoredPosition = pos;
        RectTransform.rotation = Quaternion.Euler(0,0,talk_y);

	    _charaImage.color = isTalk ? Color.white : Color.gray;
	}
}
