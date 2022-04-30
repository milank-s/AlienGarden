using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCam : MonoBehaviour {

	float interval = 0;
	float time = 0.1f;
	// Update is called once per frame
	void Update () {
		interval -= Time.deltaTime;
			interval = Random.Range(0.1f, 0.2f);
			transform.Rotate (Random.Range(0f, 2f), Random.Range(-1f, 0f), Random.Range(-1f, 2f));
			
	}
}
