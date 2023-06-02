using UnityEngine;
using System.Collections;
using System;
using UnityEngine.SceneManagement;
using System.Collections.Generic;


public class  GameOfLife : MonoBehaviour {

	public TextAsset names;

	[HideInInspector]
	public string[] plantNames;

	public enum GameStates {
		Idle, Running
	}
	public GameObject[] plantUI;
	public GameObject ScoreGUI;
	public Texture2D palette;
	public GameObject player;
	public List<Plant> plants;
	public float plantInitAmount;
	public Cell cellPrefab;
	public Plant plantPrefab;
	private float updateInterval; // delay between cell updates
	public GameObject[] scoreBoard;
	[HideInInspector] public Cell[,] cells; // matrix of cells
	[HideInInspector] public GameStates state = GameStates.Idle;
	public static int sizeX = 100; // game size in x-axis
	public static int sizeY = 50; // game size in y-axis
	bool spawnPlant = false;
	public float interval;
	public float refreshRate = 0.2f;
	public int curPlantType = 1;

	public Action cellUpdates; // action which calls cells' update methods
	public Action cellApplyUpdates; // action which calls cells' apply update methods
	public Action plantUpdates; // action which calls cells' update methods

	private IEnumerator coroutine; // reference to main coroutine

	void Awake () {
		plants = new List<Plant> ();
		plantNames = names.text.Split (new char[] { '\n' });

		scoreBoard = new GameObject[9];
		for (int i = 0; i < 9; i++) {
			GameObject newGui = (GameObject)Instantiate (ScoreGUI, Vector3.up * 48 - new Vector3 (8, 6 * i, 0), Quaternion.identity);
			scoreBoard [i] = newGui;
		}
		Init (sizeX, sizeY); // init game with 50x50 cells

//		Run (); // start update coroutine
	}

	void Update(){

		if (Input.GetKeyDown (KeyCode.R)) {
			SceneManager.LoadScene(1);
		}

		if (Input.GetKey (KeyCode.Alpha1)) {
			curPlantType = 1;
			plantUI [1].SetActive (true);
			plantUI [2].SetActive (false);
			plantUI [0].SetActive (false);
		}else if (Input.GetKey (KeyCode.Alpha2)) {
			curPlantType = 0;
			plantUI [0].SetActive (true);
			plantUI [1].SetActive (false);
			plantUI [2].SetActive (false);
		}else if (Input.GetKey (KeyCode.Alpha3)) {
			curPlantType = 2;
			plantUI [2].SetActive (true);
			plantUI [1].SetActive (false);
			plantUI [0].SetActive (false);
		}
			
		if (Input.GetKey (KeyCode.Mouse0)) {
			spawnPlant = true;
		}
		if (interval < 0) {

			UpdateCells ();

//			if (Input.GetKey(KeyCode.Mouse0)) {
////				Ray2D ray = new Ray2D (player.transform.position + (player.transform.up * 5), player.transform.up);
//				RaycastHit2D hit = Physics2D.Raycast (player.transform.position + (player.transform.up * 5), player.transform.up);
//			}
			interval = refreshRate;

			if(spawnPlant){
				SpawnPlant (player.transform.position + player.transform.up * 5, curPlantType);
				spawnPlant = false;
			}
				
			List<Vector2> rankings = new List<Vector2>();

			for (int i = 0; i < plants.Count; i++) {
				rankings.Add(new Vector2(plants [i].AliveCells, i));
			}
			rankings.Sort((b, a) => a.x.CompareTo(b.x));

			for(int p = 0; p < 9; p++){
				if (p < rankings.Count && rankings [p].x > 5) {
					scoreBoard [p].GetComponentInChildren<SpriteRenderer> ().color = plants[(int)rankings[p].y].color;
					scoreBoard [p].GetComponentInChildren<SpriteRenderer> ().sprite = plants [(int)rankings [p].y].sprite;
//					scoreBoard [p].GetComponentInChildren<TextMesh> ().text = plants[(int)rankings[p].y].AliveCells.ToString ();
					scoreBoard [p].GetComponentInChildren<TextMesh> ().text = plants[(int)rankings[p].y].gameObject.name;
				} else {
					scoreBoard [p].GetComponentInChildren<SpriteRenderer> ().color = Color.black;
					scoreBoard [p].GetComponentInChildren<TextMesh> ().text = "";
				}
			}

		} else {
			interval -= Time.deltaTime;
		}


	}

	void Reset(){
		state = GameStates.Idle;

		foreach (Cell c in cells) {
			c.nextState = States.Dead;
			c.isOnEdge = true;
			c.CellApplyUpdate ();
		}
	}

	public void Init (int x, int y) {
		
		if (cells != null) {
			for (int i = 0; i < sizeX; i++) {
				for (int j = 0; j < sizeY; j++) {
					GameObject.Destroy (cells [i, j].gameObject);
				}
			}
		}

		// clear actions
		cellUpdates = null;
		cellApplyUpdates = null;
		plantUpdates = null;
		coroutine = null;

		sizeX = x;
		sizeY = y;
		SpawnCells (sizeX, sizeY);
	}

	// this method invokes actions which call update and apply methods in cells
	public void UpdateCells () {
		if (cellUpdates != null) {
			cellUpdates ();
			cellApplyUpdates ();
			plantUpdates ();
		}
	}

	public void SpawnCells (int x, int y) {
		cells = new Cell[x, y]; // create new cells' matrix

		for (int i = 0; i < x; i++) {
			for (int j = 0; j < y; j++) {
				Cell c = Instantiate (cellPrefab, new Vector3 ((float)i, (float)j, 0f), Quaternion.identity) as Cell; // create new cell in scene
				cells [i, j] = c;
				c.Init (this, Rules.dead, i, j); // init cell by passing this object to it
				// register cell's methods to proper actions
			}
		}

		// get and set references to neighbours for every cell
		for (int i = 0; i < x; i++) {
			for (int j = 0; j < y; j++) {
				cells [i, j].neighbours = GetNeighbours (i, j);
			}
		}

		for (int p = 0; p < plantInitAmount; p++) {
			Vector3 pos = new Vector3 (sizeX/2, sizeY/2, 0);
			SpawnPlant (pos, 1);

		}
	}

	public Plant SpawnPlant(Vector3 pos, int p){
		Plant newPlant = Instantiate (plantPrefab, pos, Quaternion.identity) as Plant;
		newPlant.main = this;
		plants.Add (newPlant);
		newPlant.Init (cells[Mathf.Clamp((int)pos.x, 0, sizeX -1), Mathf.Clamp((int)pos.y, 0, sizeY-1)], p);
		newPlant.gameObject.name = plantNames [UnityEngine.Random.Range (0, plantNames.Length)];
//		newPlant.color = palette.GetPixel(UnityEngine.Random.Range(0, palette.width), 0);
		cellUpdates += newPlant.UpdateCells;
		cellApplyUpdates += newPlant.ApplyCellUpdate;
		plantUpdates += newPlant.GrowProbability;
		return newPlant;
	}

	// create array with adjacent cells to cell with coordinates (x,y)
	public Cell[] GetNeighbours (int x, int y) {
		Cell[] result = new Cell[8];

		//von neumann
		result[0] = cells[x, (y + 1) % sizeY]; // top
		result[1] = cells[(x + 1) % sizeX, y % sizeY]; // right
		result[2] = cells[x % sizeX, (sizeY + y - 1) % sizeY]; // bottom
		result[3] = cells[(sizeX + x - 1) % sizeX, y % sizeY]; // left

		//moore
		result[4] = cells[(x + 1) % sizeX, (y + 1) % sizeY]; // top right
		result[5] = cells[(x + 1) % sizeX, (sizeY + y - 1) % sizeY]; // bottom right
		result[6] = cells[(sizeX + x - 1) % sizeX, (sizeY + y - 1) % sizeY]; // bottom left
		result[7] = cells[(sizeX + x - 1) % sizeX, (y + 1) % sizeY]; // top left

		return result;
	}

	// this method stops current coroutine and starts new its instance
//	public void Run () {
//		state = GameStates.Running;
//		if (coroutine != null)
//			StopCoroutine (coroutine);
//		coroutine = RunCoroutine ();
//		StartCoroutine (coroutine);
//	}
//
//	private IEnumerator RunCoroutine () {
//
//
//		while (state == GameStates.Running) { // while simulation is running
//
//			UpdateCells (); // update all cells in game
//
//			yield return new WaitForSeconds (updateInterval); // just wait...
//		}
//	}
}
