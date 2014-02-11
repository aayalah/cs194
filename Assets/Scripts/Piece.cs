﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Piece : MonoBehaviour {
	public string id;
	public GameManager game;
	public int maxHP = 10;
	public int currentHP = 10;
	public int[] attackHistogram = {5};
	public int[] defenseHistogram = {5};
	public int[] specialHistogram = {5};
	public int attackModifier = 0;
	public int defenseModifier = 0;
	public int movementRange = 3;
	public int attackRange = 1;
	public int experience = 0;
	public Color baseColor = Color.green;

	public int x = 0;
	public int z = 0;
	public Player player;
	public GridController board;

	public bool movesHighlighted = false;
	public bool attacksHighlighted = false;

	private float lastMoveTime;

	public void Initialize(Player player, GameManager game) {
		this.player = player;
		this.game = game;
	}

	// Use this for initialization
	void Start () {
		lastMoveTime = Time.timeSinceLevelLoad;
		gameObject.renderer.material.color = Color.green;
		if(Application.loadedLevel == 2){
			board = GameObject.Find("Game").GetComponent<GridController> ();
			GameObject startingCell = board.getCellAt(x, z);
			moveTo(startingCell);
			setHighlight(false);
		}
	}
	
	// Update is called once per frame
	void Update () {

	}

	/************************
	 * STUFF W/O PLAYER INPUT
	 * **********************/

	private IEnumerator actuallyYield() {
		yield return null;
	}
	// Do a raycast to wherever mouse is pointing (?) and return the object 
	// that was hit.
	private GameObject getSelectedObject() {
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, Mathf.Infinity)) {
			return hit.transform.gameObject;
		} else {
			return null;
		}
	}

	public void moveTo(GameObject tile) {
		TileController tileController = tile.GetComponent<TileController>();
		board.movePiece(this, tileController.x, tileController.z);
		x = tileController.x;
		z = tileController.z;
		transform.position = tile.transform.position + new Vector3(0,tile.transform.localScale.y/2,0) + new Vector3(0, transform.localScale.y/2, 0);
	}

	public void moveToCoords(int xCoord, int zCoord) {
		moveTo (board.getCellAt (xCoord, zCoord));
	}

	public List<GameObject> getMoveLocations() {
		// Default movement is, let's say... everything forward, backward, left, and right.
		List<GameObject> locations = new List<GameObject> ();
		for (int i = -movementRange; i <= movementRange; i++) {
			if (board.cellIsFree(x, z+i, this)) {
				locations.Add(board.getCellAt (x, z+i));
			}
			if (i != 0) {	// So we don't add the piece's current square twice
				if (board.cellIsFree(x+i, z, this)) {
					locations.Add(board.getCellAt (x+i, z));
				}
			}
		}
		return locations;
	}
	public void setMoveHighlights(bool onOrOff) {
		movesHighlighted = onOrOff;
		foreach (GameObject tile in getMoveLocations()) {
			tile.GetComponent<TileController>().setFlashing(onOrOff);
		}
	}
	public List<Piece> getAttackablePieces() {
		// Default attack is, oh let's say anypoint <= attackRange spots away...
		List<Piece> pieces = new List<Piece> ();
		for (int i = x - attackRange; i <= x + attackRange; i++) {
			for (int j = z - attackRange; j <= z + attackRange; j++) {
				if (Mathf.Sqrt((x-i)*(x-i) + (z-j)*(z-j)) <= attackRange) {
					Piece attackablePiece = board.getPieceAt (i, j);
					if (attackablePiece /*&& attackablePiece.player != this.player*/) {
						pieces.Add (attackablePiece);
					}
				}
			}
		}
		return pieces;
	}
	public void setAttackHighlights(bool onOrOff) {
		attacksHighlighted = onOrOff;
		foreach (Piece piece in getAttackablePieces()) {
			GameObject tile = board.getCellAt(piece.x, piece.z);
			piece.setHighlight(onOrOff);
		}
	}

	public void setHighlight(bool onOrOff) {
		gameObject.renderer.material.color = onOrOff ? Color.yellow : baseColor;
	}

	/******************************
	 * STUFF REQUIRING PLAYER INPUT
	 * ****************************/

	// Wait for the player to click on a permissible tile. Then, 
	// return that tile GameObject.
	private GameObject getTile(List<GameObject> allowedTiles) {
		GameObject selected = null;
		while (!allowedTiles.Contains(selected)) {
			while (!Input.GetMouseButtonDown(0)) {
				actuallyYield();
			}
			selected = getSelectedObject();
		}
		return selected;
	}

	public IEnumerator makeMove() {
		List<GameObject> moveLocations = getMoveLocations();
		setMoveHighlights(true);
		GameObject selected = null;
		while (!moveLocations.Contains(selected)) {
			yield return null;
			while (!Input.GetMouseButtonDown(0)) {
				yield return null;
			}
			selected = getSelectedObject();
		}
		setMoveHighlights(false);
		moveTo(selected);
	}

	public IEnumerator attack() {
		List<Piece> attackablePieces = getAttackablePieces();
		if (attackablePieces.Count == 0) {
			// If no attacks, just move on
			print ("No pieces :(");
			yield return null;
		} else {
			setAttackHighlights(true);
			GameObject selectedObject = null;
			Piece selectedPiece = null;
			while (!attackablePieces.Contains(selectedPiece)) {
				yield return null;
				while (!Input.GetMouseButtonDown(0)) {
					yield return null;
				}
				selectedObject = getSelectedObject();
				if (selectedObject) {
					selectedPiece = selectedObject.GetComponent<Piece>();
				}
			}
		}
		setAttackHighlights(false);
	}




}
