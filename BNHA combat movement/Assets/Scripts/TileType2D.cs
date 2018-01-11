using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TileType2D {

	public string name;
	public GameObject tileVisualPrefab;

	public bool isWalkable = true;
	public float movementCost = 1;


}
