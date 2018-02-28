using TMPro;
using UnityEngine;

public class TimeLabel : MonoBehaviour
{
    public TextMeshProUGUI label;
	
	// Update is called once per frame
	void Update ()
	{
	    float timeRemain = GameManager.Instance.timeRemain;
	    if (timeRemain > 60f)
	    {
	        label.text = "ОСТАЛОСЬ: " + (int) (timeRemain/60) + "мин " + ((float) (timeRemain%60f)).ToString("##") + "сек";
	    }
        else label.text = "ОСТАЛОСЬ: " + timeRemain.ToString("##") + "сек";
    }
}
