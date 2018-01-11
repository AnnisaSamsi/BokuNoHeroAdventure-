using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickableTile2D : MonoBehaviour {

	public int tileX;
	public int tileY;
	public Tilemap2D map;


	void OnMouseUp(){

		RaycastHit raycastHit = new RaycastHit ();

		if (Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out raycastHit)) {
			if(raycastHit.collider.tag == "clickabletile"){
				map.GeneratePathTo (tileX, tileY);
				Debug.Log("Click");
				//for (int x = 0; x < 10; x++) {
				//	for (int y = 0; y < 10; y++) {
				//		map.GenerateHighlight (x, y, false);
				//	}
				//}
			}

		}

	}
}
