using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit2D : MonoBehaviour {

	public int tileX;
	public int tileY;
	public Tilemap2D map;

	public List<Node> currentPath;

	public int moveRange;

	float remainingMovement;
	bool highlightEn = false;

	public void HighlightOn(){
		highlightEn = true;
		for (int x = 0; x < 10; x++) {
			for (int y = 0; y < 10; y++) {
				map.GenerateHighlight (x, y, highlightEn);
			}
		}
	}

	void Update(){
		GameObject currentSelectedUnit = GameObject.Find ("Map").GetComponent<Tilemap2D> ().selectedUnit;
		if (Input.GetMouseButtonUp (0)) {
			RaycastHit raycastHit = new RaycastHit ();

			if (Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out raycastHit)) {
				if (raycastHit.collider.tag != "char") {
					if (raycastHit.collider.tag != "button") {
						highlightEn = false;
						Debug.Log ("cancel highlight");
						for (int x = 0; x < 10; x++) {
							for (int y = 0; y < 10; y++) {
								map.GenerateHighlight (x, y, highlightEn);
							}
						}
					}
				}
//				if (highlightEn == true) {
//					for (int x = 0; x < 10; x++) {
//						for (int y = 0; y < 10; y++) {
//							map.GenerateHighlight (x, y, highlightEn);
//						}
//					}
//				}
					
//				if (raycastHit.collider.tag == "char") {
//					highlightEn = !highlightEn;
//					Debug.Log ("hit a character at");
//					Debug.Log (gameObject.transform.position.x);
//
//					for (int x = 0; x < 10; x++) {
//						for (int y = 0; y < 10; y++) {
//							map.GenerateHighlight (x, y, highlightEn);
//						}
//					}
//					return;
//				} 
			}
		}
		if (currentSelectedUnit == gameObject) {

			//if (highlightEn == true) {
				
			if (currentPath != null) {

				Debug.Log ("current path not null");
				int currNode = 0;

				while (currNode < currentPath.Count - 1) {

					Vector3 start = map.TileCoordToWorldCoord (currentPath [currNode].x, currentPath [currNode].y) +
					                 new Vector3 (0, 0, -1f);
					Vector3 end = map.TileCoordToWorldCoord (currentPath [currNode + 1].x, currentPath [currNode + 1].y) +
					               new Vector3 (0, 0, -1f);

					Debug.DrawLine (start, end, Color.red);

					currNode++;
				}
			}

			remainingMovement = moveRange;

			if (Vector3.Distance (transform.position, map.TileCoordToWorldCoord (tileX, tileY)) < 0.1f) {
				AdvancePathing ();

			}

			transform.position = Vector3.Lerp (transform.position, map.TileCoordToWorldCoord (tileX, tileY), 5f * Time.deltaTime);

		//}


		}

	}

	void AdvancePathing(){

		if (currentPath == null) {
			return;
		}
		if (remainingMovement <= 0) {
			return;
		}
		transform.position = map.TileCoordToWorldCoord (tileX, tileY);

		remainingMovement -= map.CostToEnterTile (currentPath [0].x, currentPath [0].y, currentPath [1].x, currentPath [1].y)-1;

		tileX = currentPath[1].x;
		tileY = currentPath[1].y;

		currentPath.RemoveAt(0);

		if (currentPath.Count == 1) {
			//we are standing on the last tile
			currentPath = null;
			Debug.Log ("currentPath is null");
			//this should be where we bring up attack commands and highlight the attack range
		}
	}




	/*	public void NextTurn(){
		while (currentPath != null && remainingMovement > 0) {
			AdvancePathing ();
		}

		remainingMovement = moveRange;
	}

	public void MoveNextTile(){



		float remainingMovement = moveRange;

		while (remainingMovement > 0) {
		
			if (currentPath == null) {
				return;
			}

			remainingMovement -= map.CostToEnterTile (currentPath [0].x, currentPath [0].y, currentPath [1].x, currentPath [1].y)-1;

			//now grab new first node and move to that position
			tileX = currentPath[1].x;
			tileY = currentPath[1].y;

			transform.position = map.TileCoordToWorldCoord(currentPath[1].x, currentPath[1].y);

			//remove old current/first node from path
			currentPath.RemoveAt(0);

			if (currentPath.Count == 1) {
				//we are standing on the last tile
				currentPath = null;
			}

		}




	}
*/
}
