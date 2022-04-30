using UnityEngine;
using System.Collections;

public enum States {
	Dead, Alive, Static
}

public class Cell : MonoBehaviour {

	public bool wasTouched = false;

	[HideInInspector] public GameOfLife gameOfLife;
	[HideInInspector] public int x, y;
	[HideInInspector] public Cell[] neighbours;

	[HideInInspector] public States state;
	public States nextState;
	public Plant plant;
	public bool isOnEdge;
	public int lifeSpan;
	public int neighboursAlive;
	private  SpriteRenderer spriteRenderer;

	void Awake () {
		plant = null;
		isOnEdge = true;
		spriteRenderer = GetComponent <SpriteRenderer> ();
		state = States.Dead;
	}

	public void Add(Plant p){

			Remove ();

			plant = p;
			GetComponent<SpriteRenderer> ().sprite = plant.sprite;
			lifeSpan = plant.cellLifeSpan;
	}

	public void Kill(){
		Remove ();
		nextState = States.Dead;
		CellApplyUpdate ();
	}

	public void Remove(){
		if(plant != null){
			plant.cells.Remove (this);	
			plant.cellsOnEdge.Remove (this);
			isOnEdge = true;
			plant = null;
//		foreach (Cell c in neighbours) {
//			c.isOnEdge = true;
//		}
		}
	}

	// this method implements cells' behaviour
	public void CellUpdate () {
		nextState = state;
		neighboursAlive = 0;

		neighboursAlive = GetNeighbours ();
		States newState = (States)plant.rule [neighboursAlive];

		if (newState == States.Static) {
			//do nothing
		} else {
			nextState = newState;
		}

		if (lifeSpan <= 0) {
			Kill ();
		}
	}

	// apply new cell's state and update its material
	public void CellApplyUpdate () {
		state = nextState;
		UpdateMaterial ();
	}

	// pass parent object and store x-axis and y-axis coordinates
	public void Init (GameOfLife gol, int[] r, int x, int y) {
		gameOfLife = gol;

		this.x = x;
		this.y = y;
	}

	// use it to set initial, random cell state
	public void SetRandomState () {
		state = (Random.Range (0, 2) == 0) ? States.Dead : States.Alive;
		UpdateMaterial ();
	}

	// change cell appearance based on its state
	private void UpdateMaterial () {
		if (state == States.Alive)
			spriteRenderer.color = plant.color;
		else
			spriteRenderer.color = Color.black;
	}

	// check cell's alive neighbours count
	public int GetNeighbours () {
		int ret = 0;
		if (plant.VonNeumann) {
			for (int i = 0; i < neighbours.Length/2; i++) {
				if (neighbours [i] != null && neighbours [i].state == States.Alive)
					ret++;
			}
		}else{
			for (int i = 0; i < neighbours.Length; i++) {
				if (neighbours[i] != null && neighbours [i].state == States.Alive)
					ret++;
			}
		}

		if (plant.Totalistic) {
			if (state == States.Alive) {
				ret++;
			}
		}
		return ret;
	}
}
