using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jitter : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

	float interval = 0;
	float time = 0.1f;
	// Update is called once per frame
	void Update () {
		interval -= Time.deltaTime;
		if (interval < 0) {
			interval = Random.Range (0.2f, 0.33f);
			transform.eulerAngles = new Vector3 (Random.Range (0f, 1f), Random.Range (0f, 1f), 0);

		}
	}
}
