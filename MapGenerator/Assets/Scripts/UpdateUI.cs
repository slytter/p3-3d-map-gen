using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UpdateUI : MonoBehaviour
{

	[SerializeField]
	private Text timerLabel;

	[SerializeField]
	private Text keysLabel;

	[SerializeField]
	public Image checkmark;



	// Use this for initialization
	void Awake ()
	{
		//helperText.GetComponent<Text>().enabled = false;
		checkmark.enabled = false;
	}
	
	// Update is called once per frame
	void Update ()
	{
        
		timerLabel.text = FormatTime (GameManager.instance.countedTime);
		keysLabel.text = GameManager.instance.NumKeys.ToString () + "/" + GameManager.instance.maxKeys;
		if (GameManager.instance.NumKeys == GameManager.instance.maxKeys) {
			//helperText.GetComponent<Text>().enabled = true;
			checkmark.enabled = true;

		}

	}

	// Converting the time from seconds to minutes and seconds
	private string FormatTime (float timeInSeconds)
	{
		return string.Format ("{0}:{1:00}", Mathf.FloorToInt (timeInSeconds / 60), Mathf.FloorToInt (timeInSeconds % 60));
	}

}
