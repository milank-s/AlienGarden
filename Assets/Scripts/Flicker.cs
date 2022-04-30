using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flicker : MonoBehaviour {

	float interval = 0;
	float time = 0.1f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
		interval -= Time.deltaTime;
		if (interval < 0) {
			float c = Random.Range (0.2f, 1f);
			GetComponent<TextMesh> ().color = new Color (c, c, c);
			interval = time;
//			transform.position = Random.insideUnitSphere/4;
		}
	}
}
