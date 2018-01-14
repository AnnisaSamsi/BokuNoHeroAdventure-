using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Tilemap2D : MonoBehaviour {

	public GameObject selectedUnit;

	public TileType2D[] tileTypes;

	int[,,] tiles;
	Node[,] graph;

	int mapSizex = 10;
	int mapSizey = 10;
	int z = 2 ;

	public Button move;
	public Button attack;
	public Button item;
	public Button wait;

	public int range;

	void Start() {
		//public GameObject selectedUnit = GameObject.Find("Unit1"); //since we dragged Unit into this, Unit is always selected.

		//set up selectedunit's variable
		selectedUnit.GetComponent<Unit2D>().tileX = (int)selectedUnit.transform.position.x;
		selectedUnit.GetComponent<Unit2D>().tileY = (int)selectedUnit.transform.position.y;
		selectedUnit.GetComponent<Unit2D> ().map = this;



		GenerateMapData ();
		GeneratePathfindingGraph ();
		GenerateMapVisual ();

		move = GameObject.Find("Move").GetComponent<Button> ();
		attack = GameObject.Find("Attack").GetComponent<Button> ();
		item = GameObject.Find("Item").GetComponent<Button> ();
		wait = GameObject.Find("Wait").GetComponent<Button> ();

	}

	void Update(){
		if (Input.GetMouseButtonUp (0)){
			RaycastHit raycastHit = new RaycastHit ();

			if (Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out raycastHit)) {
				if(raycastHit.collider.tag == "char"){
					if (selectedUnit == raycastHit.collider.gameObject) {
						Debug.Log ("You already selected this unit");
						return;
					}
					if (selectedUnit != raycastHit.collider.gameObject) {
						selectedUnit = raycastHit.collider.gameObject;

					}
					if (selectedUnit.GetComponent<Unit2D> ().currentPath == null) {
						Debug.Log ("Dont move until a tile is selected");
						selectedUnit.GetComponent<Unit2D> ().currentPath = null;

						selectedUnit.GetComponent<Unit2D>().tileX = Mathf.RoundToInt(selectedUnit.transform.position.x);
						selectedUnit.GetComponent<Unit2D>().tileY = Mathf.RoundToInt(selectedUnit.transform.position.y);
						selectedUnit.GetComponent<Unit2D> ().map = this;
//
//						GeneratePathfindingGraph ();
//						return;
					}

				}

			}
		}
		TileType2D grass = tileTypes [0];
		TileType2D swamp = tileTypes [1];
		TileType2D highlight = tileTypes [3];
		if (selectedUnit.GetComponent<Unit2D> ().hasMoved == true) {
			if (grass.isWalkable == true) {
				Debug.Log ("Unit has moved");

				grass.isWalkable = false;
				swamp.isWalkable = false;
				highlight.isWalkable = false;
			}
			if (move.interactable == true) {
				move.interactable = false;
			}
		}
		if (selectedUnit.GetComponent<Unit2D> ().hasMoved == false) {
			if (move.interactable == false) {
				move.interactable = true;
			}
		}
		if (selectedUnit.GetComponent<Unit2D> ().charTurnEnd == true) {
			if (wait.interactable == true) {
				attack.interactable = false;
				item.interactable = false;
				wait.interactable = false;
			}
		}
		if (selectedUnit.GetComponent<Unit2D> ().charTurnEnd == false) {
			if (wait.interactable == false) {
				attack.interactable = true;
				item.interactable = true;
				wait.interactable = true;
			}
		}
	}

	void GenerateMapData(){

		tiles = new int[mapSizex, mapSizey, z];

		int x, y;
		//initialize map tiles to be grass
		for (x = 0; x < mapSizex; x++) {
			for (y = 0; y < mapSizey; y++) {
					tiles [x, y, 0] = 0;
					tiles [x, y, 1] = 3;	//creates a layer of transparent tiles z value is just in the matrix.
			}
		}


		//make a swamp area
		for(x=3; x <= 5; x++){
			for (y = 0; y < 4; y++) {
				tiles [x, y, 0] = 1;
			}
		}
		//make u shaped mountain range

		tiles [4, 4, 0] = 2;
		tiles [5, 4, 0] = 2;
		tiles [6, 4, 0] = 2;
		tiles [7, 4, 0] = 2;
		tiles [8, 4, 0] = 2;

		tiles [4, 5, 0] = 2;
		tiles [4, 6, 0] = 2;
		tiles [8, 5, 0] = 2;
		tiles [8, 6, 0] = 2;

	}



	public void MoveButton(){
		if (selectedUnit.GetComponent<Unit2D> ().hasMoved == false) {
			bool highlightEn = true;
			range = selectedUnit.GetComponent<Unit2D> ().moveRange;
			TileType2D grass = tileTypes [0];
			TileType2D swamp = tileTypes [1];
			TileType2D highlight = tileTypes [3];
			grass.isWalkable = true;
			swamp.isWalkable = true;
			highlight.isWalkable = true;
			for (int x = 0; x < 10; x++) {
				for (int y = 0; y < 10; y++) {
					GenerateHighlight (x, y, highlightEn);
				}
			}
		}

	}

	public void AttackButton(){
		range = selectedUnit.GetComponent<Unit2D> ().attackRange;
		TileType2D grass = tileTypes [0];
		TileType2D swamp = tileTypes [1];
		TileType2D highlight = tileTypes [3];
		grass.isWalkable = true;
		swamp.isWalkable = true;
		highlight.isWalkable = true;
		//bool highlightEn = true;
		for (int x = 0; x < 10; x++) {
			for (int y = 0; y < 10; y++) {
				GenerateHighlight (x, y, true);
			}
			Debug.Log ("highlight");
		}
		Debug.Log ("Attack");
		selectedUnit.GetComponent<Unit2D> ().hasMoved = true;
	}

	public void WaitButton(){
		selectedUnit.GetComponent<Unit2D> ().charTurnEnd = true;
		Debug.Log ("end turn");
	}


	public float CostToEnterTile(int sourcex, int sourcey, int targetx, int targety){

		TileType2D tt = tileTypes [tiles [targetx, targety, 0]];

		if (tt.isWalkable == false) {
			return Mathf.Infinity;
		}


		return tt.movementCost;

	}


	void GeneratePathfindingGraph (){
		//initialize array
		graph = new Node[mapSizex, mapSizey];

		//initialize a Node for each spot in array
		for (int x = 0; x < mapSizex; x++) {
			for (int y = 0; y < mapSizey; y++) {
				graph [x, y] = new Node ();
				graph [x, y].x = x;
				graph [x, y].y = y;
			}
		}

		//now all nodes exist, calculate their neighbours.
		for (int x = 0; x < mapSizex; x++) {
			for (int y = 0; y < mapSizey; y++) {

				//for a 4 way connected map.
				if (x > 0)
					graph [x, y].neighbours.Add (graph [x - 1, y]);
				if (x < mapSizex - 1)
					graph [x, y].neighbours.Add (graph [x + 1, y]);
				if (y > 0)
					graph [x, y].neighbours.Add (graph [x, y - 1]);
				if (y < mapSizey - 1)
					graph [x, y].neighbours.Add (graph [x, y + 1]);
			}
		}
	}


	void GenerateMapVisual (){
		for (int z = 0; z < 2; z++) {
			for (int x = 0; x < mapSizex; x++) {
				for (int y = 0; y < mapSizey; y++) {

					TileType2D tt = tileTypes [tiles [x, y, z]];
					if (z != 0) {
						GameObject gO = (GameObject)Instantiate (tt.tileVisualPrefab, new Vector3 (x, y, -.25f), Quaternion.identity);
						//this creates a layer of transparent tiles over the map with a z offset of .25 toward the camera.
						string prefabname = "highlight";
						gO.name = prefabname + x + y;
						gO.GetComponent<Renderer> ().enabled = false;
					}
					if (z == 0) {
						GameObject go = (GameObject)Instantiate (tt.tileVisualPrefab, new Vector3 (x, y, z), Quaternion.identity);
						ClickableTile2D ct = go.GetComponent<ClickableTile2D> ();
						ct.tileX = x;
						ct.tileY = y;		//this is only used for the non transparent tiles,
						ct.map = this;		//since the transparent tiles do not have colliders.
					}

				}
			}
		}


	}

	public Vector3 TileCoordToWorldCoord(int x, int y){

		return new Vector3 (x, y, 0);
	}

	public bool UnitCanEnterTile(int x, int y){

		//here would be for testing a unit's ability to enter a tile. if a unit is flying, etc.


		return tileTypes [tiles [x, y, 0]].isWalkable;
	}

	public void GeneratePathTo(int x, int y){
		//clear out unit's old path.
		selectedUnit.GetComponent<Unit2D> ().currentPath = null;
//		bool highlightEnable = selectedUnit.GetComponent<Unit2D> ().highlightEn;
//
//		if (highlightEnable == false) {
//
//			return;
//		}
		if (UnitCanEnterTile (x, y) == false) {
			//user clicked on mountain or out of range
			return;
		}


		Dictionary<Node, float> dist = new Dictionary<Node, float> ();
		Dictionary<Node, Node> prev = new Dictionary<Node, Node> ();

		//list of nodes we haven't checked yet. "Q"
		List<Node> unvisited = new List<Node> ();

		Node source = graph [
			selectedUnit.GetComponent<Unit2D> ().tileX,
			selectedUnit.GetComponent<Unit2D> ().tileY
		];
		Node target = graph [x, y];


		dist [source] = 0;
		prev [source] = null;

		//initialize everything to be infinite distance, since we don't know.
		foreach (Node v in graph) {
			if (v != source) {
				dist [v] = Mathf.Infinity;
				prev [v] = null;
			}

			unvisited.Add (v);
		}

		while (unvisited.Count > 0) {
			//u is going to be the unvisited node with the smallest distance.
			Node u = null;

			foreach (Node possibleU in unvisited) {
				if (u == null || dist [possibleU] < dist [u]) {
					u = possibleU;
				}
			}

			if (u == target) {
				break; // exit while loop.
			}


			unvisited.Remove (u);

			foreach (Node v in u.neighbours) {
				float alt = dist [u] + CostToEnterTile (u.x, u.y, v.x, v.y);
				if (alt < dist [v]) {
					dist [v] = alt;
					prev [v] = u;
				}
			}
		}
		//if we get here, we have found shortest route, or no route exists.

		if (prev [target] == null) {
			//no route exists
			return;
		}

		List<Node> currentPath = new List<Node> ();

		Node curr = target;

		while (curr != null) {
			currentPath.Add (curr);
			curr = prev [curr];
		}
		//currentPath goes from the target to the source, so we reverse it.

		currentPath.Reverse ();

		int unitRange = selectedUnit.GetComponent<Unit2D> ().moveRange;

		if (currentPath.Count > unitRange+1) {	//do nothing if tile clicked is out of range.
			return;
		}
		selectedUnit.GetComponent<Unit2D> ().currentPath = currentPath;
		Debug.Log ("generated path");
		if (selectedUnit.GetComponent<Unit2D> ().currentPath != null) {
			Debug.Log ("current path is generated");
		}
	}


	public void GenerateHighlight(int coordx, int coordy, bool highlightEnable){
		if (UnitCanEnterTile (coordx, coordy) == false) {
			//user clicked on mountain or out of range
			return;
		}

		Dictionary<Node, float> dist = new Dictionary<Node, float> ();
		Dictionary<Node, Node> prev = new Dictionary<Node, Node> ();

		//list of nodes we haven't checked yet. "Q"
		List<Node> unvisited = new List<Node> ();

		Node source = graph [
			selectedUnit.GetComponent<Unit2D> ().tileX,
			selectedUnit.GetComponent<Unit2D> ().tileY
		];
		Node target = graph [coordx, coordy];


		dist [source] = 0;
		prev [source] = null;

		//initialize everything to be infinite distance, since we don't know.
		foreach (Node v in graph) {
			if (v != source) {
				dist [v] = Mathf.Infinity;
				prev [v] = null;
			}

			unvisited.Add (v);
		}

		while (unvisited.Count > 0) {
			//u is going to be the unvisited node with the smallest distance.
			Node u = null;

			foreach (Node possibleU in unvisited) {
				if (u == null || dist [possibleU] < dist [u]) {
					u = possibleU;
				}
			}

			if (u == target) {
				break; // exit while loop.
			}


			unvisited.Remove (u);

			foreach (Node v in u.neighbours) {
				float alt = dist [u] + CostToEnterTile (u.x, u.y, v.x, v.y);
				if (alt < dist [v]) {
					dist [v] = alt;
					prev [v] = u;
				}
			}
		}
		//if we get here, we have found shortest route, or no route exists.

		if (prev [target] == null) {
			//no route exists
			return;
		}

		List<Node> currentPath = new List<Node> ();

		Node curr = target;

		while (curr != null) {
			currentPath.Add (curr);
			curr = prev [curr];
		}
			

		if (currentPath.Count <= range+1) {	//do nothing if tile clicked is out of range.
			GameObject highlightTile = GameObject.Find ("highlight" + coordx + coordy);
			highlightTile.GetComponent<Renderer> ().enabled = highlightEnable;
		}

		int charCoordX = Mathf.RoundToInt ((float)selectedUnit.transform.position.x);
		int charCoordY = Mathf.RoundToInt ((float)selectedUnit.transform.position.y);
		GameObject charTile = GameObject.Find ("highlight" + charCoordX + charCoordY);

		if (charTile.GetComponent<Renderer> ().enabled != highlightEnable) {
			charTile.GetComponent<Renderer> ().enabled = highlightEnable;
		}
	}

}






