using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantNames : MonoBehaviour {

	public TextAsset names;

	[HideInInspector]
	public string[] plantNames;
	void Start () {
		plantNames = names.text.Split (new char[] { '\n' });
	}

}
