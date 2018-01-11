using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharSelectandMove : MonoBehaviour {

    
    public int moverange = 3;
	public int mouseOverChar = 0;	//variable to tell if the cursor is hovering over the character.
	//public Transform prefab;
	//public int totaltilesx = 10;
	//public int totaltilesy = 10;
	//public tiles = new Renderer[totaltilesy, totaltilesx];

	//public var tiles = Renderer[,];

	// Use this for initialization
	void Start ()
	{
		/*int row = 0;
		int column = 0;
		float x = -totaltilesx / 2.0F;
		float y = totaltilesy / 2.0F;
		var tiles = new Renderer[totaltilesy, totaltilesx];
		if (row < totaltilesy) 
		{
			if (column < totaltilesx)
			{
				var go = Instantiate (prefab, new Vector3(x, y, 0.0F), Quaternion.identity);
				tiles [row, column] = GetComponent<Renderer>();
				x += 1.0f;
				column = column + 1;
			}
			y -= 1.0f;
			x = -totaltilesx / 2.0f;
			row = row + 1;
		}*/

	}
	void OnMouseOver ()	//if the cursor is over the character, set mouseOverChar to 1.
	{
		mouseOverChar = 1;
	}
	void OnMouseExit ()	//if cursor is not over the character, reset mouseOverChar to 0.
	{
		mouseOverChar = 0;
	}
	// Update is called once per frame
	void Update () 
	{
        //if the cursor is over the character, and the
        //mouse button is pressed, highlight where the 
        //character can move
		//var tiles = new Renderer[totaltilesy, totaltilesx];
		if (Input.GetMouseButtonDown (0) & (mouseOverChar == 1))
        {
			Debug.Log ("Pressed left click on character.");
			transform.GetComponent<Renderer> ().material.color = Color.yellow;
            //changes color of character to yellow when clicked.
			/*int row = 0;
			int column = 0;
            if ((row) < moverange)
            {
                tiles[row, column].material.color = Color.yellow;
            }*/
		}
	}
}



//Figure out how to highlight the background tile without directly clicking it.
//it should register the character is on the tile
//and that the mouse is on the character. so it should be


//if the cursor is over the character, the character is over the tile,
//and the mouse button is pressed, the tile is highlighted.