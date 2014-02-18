using UnityEngine;
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
	public int minDamage;
	public int attackModifier = 0;
	public int defenseModifier = 0;
	public int movementRange = 3;
	public int attackRange = 1;
	public int experience = 0;
	public Color baseColor = Color.green;
	public bool dead = false;

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
		minDamage = 1;
		lastMoveTime = Time.timeSinceLevelLoad;
		gameObject.renderer.material.color = Color.white;
		if(Application.loadedLevel == 2){
			board = GameObject.Find("Game").GetComponent<GridController> ();
			GameObject startingCell = board.getCellAt(x, z);
			moveTo(startingCell);
			setColor(baseColor);
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
			for(int j = -movementRange; j <= movementRange; j++){
				if(Mathf.Abs(i)+Mathf.Abs(j) <= movementRange){
					if (board.cellIsFree(x+i, z+j, this)) {
						if(!locations.Contains(board.getCellAt(x+i, z+j)))
							locations.Add(board.getCellAt (x+i, z+j));
					}
				}
			}/*
			if (board.cellIsFree(x, z+i, this)) {
				locations.Add(board.getCellAt (x, z+i));
			}
			if (i != 0) {	// So we don't add the piece's current square twice
				if (board.cellIsFree(x+i, z, this)) {
					locations.Add(board.getCellAt (x+i, z));
				}
			}*/

		}
		return locations;
	}
	public void setMoveHighlights(bool onOrOff, List<GameObject> locations) {
		movesHighlighted = onOrOff;
		foreach (GameObject tile in locations) {
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
					if (attackablePiece && attackablePiece.player != this.player) {
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
			piece.setColor(onOrOff ? Color.yellow : piece.baseColor);
		}
	}

	public void setColor(Color color) {
		gameObject.renderer.material.color = color;
	}

	public void die() {
		currentHP = 0;
		dead = true;
		gameObject.transform.position = new Vector3(0,-100000,0);
		board.removePiece(this);
		game.reduceNumPieces (player.getId ());
	}

	public void takeDamage(int damage) {
		int index = Random.Range(0, defenseHistogram.Length);
		int shield = defenseHistogram[index];
		Debug.Log("Shield for "+ shield/2);
		if(shield/2 >= damage){
			damage = 0;
			Debug.Log("Blocked all damage");
		} 
		Debug.Log("Remaining HP: " + (currentHP - damage));
		currentHP = Mathf.Max (0, currentHP - damage);
		if (currentHP <= 0) {
			die();
		}
	}

	public void damageEnemy(Piece other) {
		int damage = minDamage;
		if (attackHistogram.Length > 0) {
			int index = Random.Range(0, attackHistogram.Length);
			damage = Mathf.Max(damage, attackHistogram[index]);
		}
		Debug.Log("Attack for " + damage + " damage");
		other.takeDamage(damage);
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
		if (dead) {
			yield return null;
		} else { 
			List<GameObject> moveLocations = getMoveLocations();
			setMoveHighlights(true, moveLocations);
			GameObject selected = null;
			while (!moveLocations.Contains(selected)) {
				yield return null;
				while (!Input.GetMouseButtonDown(0)) {
					yield return null;
				}
				selected = getSelectedObject();
			}
			setMoveHighlights(false, moveLocations);
			moveTo(selected);
		}
	}

	public IEnumerator attack() {
		if (dead) {
			yield return null;
		} else {
			List<Piece> attackablePieces = getAttackablePieces();
			if (attackablePieces.Count == 0) {
				// If no attacks, just move on
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
				damageEnemy(selectedPiece);
			}
			setAttackHighlights(false);
		}
	}


	public IEnumerator initialPlacement(int id) {

		if (board == null) {
			board = GameObject.Find("Game").GetComponent<GridController> ();
		}



		Debug.Log ("initialPlacement Start");
		List<GameObject> moveLocations = getInitialLocations(id);
		setMoveHighlights(true, moveLocations);
		GameObject selected = null;
		while (!moveLocations.Contains(selected)) {
			yield return null;
			while (!Input.GetMouseButtonDown(0)) {
				yield return null;
			}
			selected = getSelectedObject();
		}
		setMoveHighlights(false, moveLocations);
		moveTo(selected);
		Debug.Log ("initialPlacement Start");
	}


	public List<GameObject> getInitialLocations(int player) {
		Debug.Log ("initialLocations Start");
		// Default movement is, let's say... everything forward, backward, left, and right.
		int row;
		if (player == 0) {
			row = 0;
		} else {
			row = board.zDimension - 1;
		}

		List<GameObject> locations = new List<GameObject> ();
		for (int i = 0; i <= board.xDimension; i++) {
				if (board.cellIsFree(i, row, this)) {
					if(!locations.Contains(board.getCellAt(i, row)))
						locations.Add(board.getCellAt (i,row));
				}

		}
		return locations;
	}


}
