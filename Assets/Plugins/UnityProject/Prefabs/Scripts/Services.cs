using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Services : MonoBehaviour {

	public static GameOfLife gameOfLife;

	// Use this for initialization
	void Start () {
		gameOfLife = GetComponent<GameOfLife> ();
	}

}
