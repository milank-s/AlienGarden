using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlantType {deadly, friendly, infectious};

public class Plant: MonoBehaviour {

	public int[] rule;
	public int lifeSpan;
	public int cellLifeSpan;
	public bool VonNeumann;
	public bool Totalistic;
	public GameOfLife main;
	public bool hasOffSpring = false;
	public Color color;
	public Sprite sprite;
	private Texture2D spriteTexture;
	public int AliveCells;

	[HideInInspector]
	public List<Cell> cellsOnEdge;
	public List<Cell> cells;
	private int generations; 
	public PlantType plantType;
//	private Dictionary<Vector2, Cell> cells;

	public static float remapRange(float oldVal, float oldMin, float oldMax, float newMin, float newMax){
		float newValue = 0;
		float oldRange = (oldMax - oldMin);
		float newRange = (newMax - newMin);
		newValue = (((oldVal - oldMin) * newRange) / oldRange) + newMin;
		return newValue;

	}

	public void Init(Cell c, int p){
		cellsOnEdge = new List<Cell> ();

		generations = 0;
//		int rand = Random.Range (0, 100);
//		if (rand < 10) {
//			plantType = PlantType.friendly;
//			lifeSpan = Random.Range (20, 25);
//		} else if (rand < 80) {
//			plantType = PlantType.deadly;
//			lifeSpan = Random.Range (4, 10);
//		} else {
//			lifeSpan = Random.Range (2, 4);
//			plantType = PlantType.infectious;
//		}

		plantType = (PlantType) p;

		if (plantType == PlantType.deadly) {
			lifeSpan = Random.Range (3, 6);
			cellLifeSpan = Random.Range (3, 6);
			float red = (Random.Range(70f, 120f) % 100f)/100f;
			color = Random.ColorHSV (red, red, 1, 1, 1, 1);
		} else if (plantType == PlantType.friendly) {
			lifeSpan = Random.Range (3, 7);
			cellLifeSpan = int.MaxValue;
			color = Random.ColorHSV (0.20f, 0.55f, 1, 1, 1 ,1);
		} else {
			lifeSpan = Random.Range (5, 15);
			cellLifeSpan = int.MaxValue;
			color = Random.ColorHSV (0.5f, 0.8f, 1, 1, 1,1);
		}


		cells = new List<Cell> ();
		rule = new int[10];
		rule [0] = 0;

		spriteTexture = new Texture2D (2, 2);

		for(int i = 1; i< rule.Length; i++){
			if (Random.Range (0, 100) < 50) {
				rule [i] = 0;
			} else {
				rule [i] = 1;
			}
//			Color pixel = rule [i] == 1 ? Color.white : Color.black;
//			spriteTexture.SetPixel (i/3, i%3 , pixel);
//			rule [i] = Random.Range (0, 2);
		}

		int Count = 0;
		for (int k = 0; k < 4; k++) {
			Color pixel = Random.Range(0, 100) < 50 ? Color.white : Color.black;
			if (pixel == Color.white) {
				Count++;
			}
			spriteTexture.SetPixel (k/2, k%2 , pixel);
		}

		if (Count == 0) {
			spriteTexture.SetPixels (new Color[]{Color.white, Color.white, Color.white, Color.white });
		}
//		gameObject.name = "Alien Baby " + rule[0] + rule[1] + rule[2] + rule[3] + rule[4] + rule[5] + rule[6] + rule[7] + rule[8];

		spriteTexture.filterMode = FilterMode.Point;
		spriteTexture.Apply ();
		sprite = Sprite.Create (spriteTexture, new Rect (0, 0, spriteTexture.width, spriteTexture.height), new Vector2(0.5f, 0.5f), 2f, 0, SpriteMeshType.Tight ,new Vector4(1,1,1,1)) ;

		sprite.name = gameObject.name;

		Totalistic = Random.Range (0, 100) < 50 ? true : false;
		VonNeumann = Random.Range (0, 100) < 50 ? true : false;

//		if (VonNeumann) {
//			rule [4] = 0;
//			rule [5] = 0;
//		} else {
//			rule [8] = 0;
//			rule [9] = 0;
//		}

		c.Add (this);
		c.state = States.Alive;

		int xLength = Random.Range (1, 1);
		int yLength = Random.Range (1, 1);

		for (int n = -xLength; n < xLength; n++) {
			for (int m = -yLength; m < yLength; m++) {

				int xPos = c.x + n;
				xPos = Mathf.Abs (xPos % GameOfLife.sizeX);
				int yPos = c.y + m;
				yPos = Mathf.Abs (yPos % GameOfLife.sizeY);

				Add (main.cells[xPos, yPos]);
				main.cells[xPos, yPos].state = Random.Range(0,100) < 20 ? States.Alive : States.Dead;
			}
		}
			
	}
		

	public void UpdateCells(){

		for(int i = cells.Count -1; i >= 0; i--){
			cells [i].lifeSpan--;
			cells [i].CellUpdate ();
		}

	}

	public void ApplyCellUpdate(){
		AliveCells = 0;

		for(int i = cells.Count - 1; i >= 0; i--){
			 
			Cell c = cells [i];

			c.CellApplyUpdate();
			if (c.state == States.Alive) {
				AliveCells++;
			}
			//Bools: 
			//I should check after 
			// if I'm alive after update, check neighbours. 
			// if generations are < max life 
			// if I'm on the edge of the plant 

			//Keep track of
			// if I set my neighbours rules to mine or destroy them, remove them from that plant's list
			// if my neighbour deletes me, make sure I remove myself from my own plant
			// if 
		}
	}

	public void AddNewCells(Cell c){

		ContactNeighbours (c);
		cellsOnEdge.Remove (c);
		c.isOnEdge = false;
	}
		

	public void AddNeighbours(Cell c){
		foreach (Cell n in c.neighbours) {
			Add (n);
		}
	}


	public void GrowProbability(){

		for (int i = cellsOnEdge.Count - 1; i >= 0; i--) {
			if (cellsOnEdge[i].state == States.Alive) {
				Cell c = cellsOnEdge [i];
				AddNewCells (c);
			}
		}

		generations ++;

		Kill ();
	}

	public void Add(Cell c){

		c.isOnEdge = true;
		cellsOnEdge.Add (c);
		
		cells.Add (c);
		c.Add (this);
	}

	public void Kill(){

		if (AliveCells < generations && plantType == PlantType.friendly && !hasOffSpring) {
			Plant newPlant = main.SpawnPlant (new Vector3 (transform.position.x, transform.position.y, 0), 1);
			hasOffSpring = true;
//			newPlant.hasOffSpring = true;
		}

		if (cells.Count <= 0) {
			main.cellUpdates -= this.UpdateCells;
			main.cellApplyUpdates -= this.ApplyCellUpdate;
			main.plantUpdates -= this.GrowProbability;
			Services.gameOfLife.plants.Remove (this);
			Destroy (gameObject);
		}
//			else if (AliveCells <= 0) {
//			for(int i = cells.Count -1; i >= 0; i--) {
//				cells[i].Kill ();
//			}
//			main.cellUpdates -= this.UpdateCells;
//			main.cellApplyUpdates -= this.ApplyCellUpdate;
//			main.plantUpdates -= this.GrowProbability;
//			Destroy (gameObject);
//		}
	}

	public void ContactNeighbours(Cell c){
		foreach (Cell n in c.neighbours) {

			//don't bother if it's part of plant.

			if (n.plant == this) {
				
				
			}else{

				//n.plant == null && generations < lifeSpan
				if (n.plant == null && generations < lifeSpan) {
					Add (n);
				} else {
					switch (plantType) {

					case PlantType.deadly:


						if(n.plant != null){
//							&& n.plant.plantType != PlantType.friendly
//							n.plant.rule = c.plant.rule;
							Add (n);
						}

						break;

					case PlantType.friendly:
					//plant forms a new rule? 

						if (n.plant != null && n.state == States.Dead) {
							Add (n);
						}
							
						break;

					case PlantType.infectious:

//						&& n.plant.plantType != PlantType.friendly

						if (n.plant != null && n.plant.plantType != PlantType.deadly) {
							
							Add (n);
						} 

					//plant can grow past its size restrictions
					//and continue infecting existing plant matter

						break;

					default:
					// what if a cell doesn't have a plant assigned
						break;

					}
				}
			}

		}
	
	}

}
