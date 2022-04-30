using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliderControl : MonoBehaviour {

	public TextMesh amount;
	public Transform slider;
	// Use this for initialization
	void Start () {
		amount.text = Services.gameOfLife.refreshRate.ToString("0.00");
	}

	public void OnMouseDrag(){
		slider.transform.position = new Vector3(slider.transform.position.x, Mathf.Clamp(Camera.main.ScreenToWorldPoint(Input.mousePosition).y, 0, 50), 0);
		Services.gameOfLife.refreshRate = Mathf.Pow(Mathf.Pow((1f - (slider.transform.position.y/50)), 3) * 10, 1);
		if (Services.gameOfLife.refreshRate == 10) {
			Services.gameOfLife.refreshRate = Mathf.Infinity;
			Services.gameOfLife.interval = Services.gameOfLife.refreshRate;
		}
		if (Services.gameOfLife.refreshRate <= 0.01) {
			amount.text = Services.gameOfLife.refreshRate.ToString ("0.000");
		} else {
			amount.text = Services.gameOfLife.refreshRate.ToString ("0.00");
		}
		if (Services.gameOfLife.interval > Services.gameOfLife.refreshRate) {
			Services.gameOfLife.interval = Services.gameOfLife.refreshRate;
		}
	}
}
