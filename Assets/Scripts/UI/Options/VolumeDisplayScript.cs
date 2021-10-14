using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VolumeDisplayScript : MonoBehaviour
{
    #region Fields
    
	private Slider volumeSlider;
	private TextMeshProUGUI volumeValueText;

    #endregion

    public void Start()
	{
		volumeSlider = GameObject.Find("VolumeSlider").GetComponent<Slider>();
		volumeValueText = this.GetComponentsInChildren<TextMeshProUGUI>()[1];
		volumeSlider.onValueChanged.AddListener(delegate { ValueChangeCheck(); });
	}

	// Invoked when the value of the slider changes.
	public void ValueChangeCheck()
	{
        volumeValueText.text = volumeSlider.value.ToString();
    }
}
