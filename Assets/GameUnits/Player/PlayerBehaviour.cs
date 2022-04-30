using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour {

	SpriteRenderer sr;


	void Start(){
		sr = GetComponent<SpriteRenderer> ();
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetAxis ("Horizontal") != 0) {
			transform.position += new Vector3 (Input.GetAxis ("Horizontal"), 0, 0).normalized * Time.deltaTime * 25;
			transform.up = Vector3.right * Mathf.Sign (Input.GetAxis ("Horizontal"));

		} else if (Input.GetAxis ("Vertical") != 0) {
			transform.position += new Vector3 (0, Input.GetAxis ("Vertical"), 0).normalized * Time.deltaTime * 25;
			transform.up = Vector3.up * Mathf.Sign (Input.GetAxis ("Vertical"));
		}

		if (transform.position.x < 0) {
			transform.position += Vector3.right * GameOfLife.sizeX;
		}

		if (transform.position.y < 0) {
			transform.position += Vector3.up * GameOfLife.sizeY;
		}

		if (transform.position.x > GameOfLife.sizeX) {
			transform.position = new Vector3 (0, transform.position.y, 0);
		}

		if (transform.position.y > GameOfLife.sizeY) {
			transform.position = new Vector3 (transform.position.x, 0, 0);
		}
	}

	public void OnTriggerEnter2D(Collider2D col){
		if (Input.GetKey(KeyCode.Mouse1) && col.GetComponent<Cell> ().plant != null) {
			Cell c = col.GetComponent<Cell> ();
			c.Kill();

//			if (c.plant.plantType == PlantType.friendly) {
//				c.plant.AddNewCells (c);
//			} else if (c.plant.plantType == PlantType.infectious) {
//				c.nextState = States.Dead;
//				c.CellApplyUpdate ();
//				c.Kill();
//			}
		}
	}

}
