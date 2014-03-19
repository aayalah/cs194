using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Piece : MonoBehaviour {
	private Rect windowRect = new Rect(20, 100, 250, 300);
	public Texture orderMarker;
	public string id;
	public int teamNo;
	public GameManager game;
	public int maxHP = 10;
	public int currentHP = 10;
	public int[] attackHistogram = {5};
	public int[] defenseHistogram = {5};
	public int[] specialHistogram = {5};
	public int maxSpecial = 20;
	public int currentSpecial = 0;
	public int minDamage;
	public int attackModifier = 0;
	public int defenseModifier = 0;
	public int movementRange = 3;
	public int attackRange = 1;
	public int specialRange = 2;
	public int specialStrength = 20;
	public int experience = 0;
	public Color baseColor = Color.white;
	public bool dead = false;
	private bool flashing = false;
	private int flashingTimer = 5;
	private int flash;
	public int x = 0;
	public int z = 0;
	public Player player;
	public GridController board;
	private bool showGUI = false;
	public bool movesHighlighted = false;
	public bool attacksHighlighted = false;
	private int guiTimer = 10;
	private int guiT;
	private int specialTimer = 3;
	private float lastMoveTime;
	public int LERP_STEPS_PER_TILE = 12;
	private float startingY;
	private float direction = 0;
	private bool showcaseRotate = false;
	protected bool decidingToMoveOrCharge = false;
	protected string optionChosen = "Move";
	private Vector3 startingRotation;
	public int numMarkers = 0;
	private HealthBar healthBar;

	public void Initialize(Player player, GameManager game) {
		this.player = player;
		this.game = game;
		this.healthBar = transform.gameObject.GetComponent<HealthBar> ();
		healthBar.myPiece = this;
	}

	// Use this for initialization
	void Start () {
		while(direction == 0){
			direction = Random.Range(-1, 1);
		}
		minDamage = 1;
		lastMoveTime = Time.timeSinceLevelLoad;
		startingY = transform.position.y;
		//gameObject.renderer.material.color = Color.white;
		if(Application.loadedLevel == 1){
			showcaseRotate = true;
		}
		if(Application.loadedLevel == 2){
			board = GameObject.Find("Game").GetComponent<GridController> ();
			GameObject startingCell = board.getCellAt(x, z);
			moveTo(startingCell);
			transform.position = new Vector3(0,-100000,0);
			showcaseRotate = false;
			healthBar.currentHP = currentHP;
			healthBar.maxHP = maxHP;
			//setColor(baseColor);
		}
		startingRotation = transform.localEulerAngles;
	}
	
	// Update is called once per frame
	void Update () {

		if(flashing && flash == 0) {
			//setColor(baseColor);
			stopFlashing();
		}else if (flashing) {
			//setColor(Color.red);
			flash--;
		} 

		if (showGUI && guiT == 0) {
			showGUI = false;
			healthBar.showBar = false;
		} else if (showGUI) {
			guiT--;
		}
		//if(notMoving){
			idle();
		//}

	}

	void idle(){
		if(direction == 1){
			if(transform.position.y > startingY + .5){
				direction = -1;
			}
			else{
				transform.position = transform.position + new Vector3(0, .0025f*direction, 0);
			}
		}
		else{
			if(transform.position.y < startingY+.25){
				direction = 1;
			}
			else{
				transform.position = transform.position + new Vector3(0, .0025f*direction, 0);
			}
		}
		if(showcaseRotate)
			transform.Rotate(new Vector3(0f, 1f, 0f));
	}

	/************************
	 * STUFF W/O PLAYER INPUT
	 * **********************/

	private IEnumerator actuallyYield() {
		yield return null;
	}
	// Do a raycast to wherever mouse is pointing (?) and return the object 
	// that was hit.
	public GameObject getSelectedObject() {
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, Mathf.Infinity)) {
			return hit.transform.gameObject;
		} else {
			return null;
		}
	}

	public GameObject getSelectedObject(string tag){
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit[] hits;
		hits = Physics.RaycastAll(ray, 100f);
		int i = 0;
		while(i < hits.Length){
			Debug.Log(hits[i].transform.gameObject.tag);
			if(hits[i].transform.gameObject.tag.Equals(tag)){
				return hits[i].transform.gameObject;
			}
			i++;
		}
		return null;
	}

	public IEnumerator lerpPosition(Vector3 oldPos, Vector3 newPos) {
		int steps = (int) (Vector3.Distance(oldPos, newPos) * LERP_STEPS_PER_TILE);
		for (int i = 1; i <= steps; i++) {
			transform.position = Vector3.Lerp(oldPos, newPos, (float) i / steps);
			yield return null;
		}
	}

	public IEnumerator leanForward(Vector3 direction){
		direction.Normalize();
		transform.localEulerAngles = direction*30f;
		if(teamNo == 0) transform.Rotate(new Vector3(0, 180, 0));
		yield return null;
	}

	public IEnumerator leanBack(Vector3 direction){
		transform.localEulerAngles = startingRotation;
		yield return null;
	}

	public IEnumerator movePhysically(int oldX, int oldZ, int newX, int newZ) {
		float halfPieceHeight = transform.localScale.y / 2;
		float maxHeight = board.maxHeightOnPath(oldX, oldZ, newX, newZ) + halfPieceHeight;
		GameObject currentCell = board.getCellAt(oldX, oldZ);
		TileController t = currentCell.transform.GetComponent<TileController> ();
		if(t.isKingTile){
			player.removeKing();
		}
		GameObject endCell = board.getCellAt(newX, newZ);
		float currentHeight = currentCell.transform.position.y + currentCell.transform.localScale.y / 2;
		float endHeight = endCell.transform.position.y + endCell.transform.localScale.y;// / 2;

		Vector3 start = transform.position;
		Vector3 aboveStart = new Vector3(start.x, maxHeight + halfPieceHeight, start.z);
		Vector3 end = new Vector3(endCell.transform.position.x, endHeight + halfPieceHeight, endCell.transform.position.z);
		Vector3 aboveEnd = new Vector3(end.x, maxHeight + halfPieceHeight, end.z);

		// First shift up...
		yield return StartCoroutine(lerpPosition(start, aboveStart));

		//yield return StartCoroutine(leanForward(aboveEnd-aboveStart));

		// Then over...
		yield return StartCoroutine(lerpPosition(aboveStart, aboveEnd));

		//yield return StartCoroutine(leanBack(aboveEnd-aboveStart));

		// Then down.
		yield return StartCoroutine(lerpPosition(aboveEnd, end));
	}

	public void moveTo(GameObject tile, bool changePosition = true) {
		/*
		TileController oldController = board.getCellAt(x, z).GetComponent<TileController>();
		oldController.setColor(oldController.baseColor);
		*/
		TileController newController = tile.GetComponent<TileController>();
		//ewController.setColor(Color.black);
		board.movePiece(this, newController.x, newController.z);
		x = newController.x;
		z = newController.z;
		if (changePosition) {
			transform.position = tile.transform.position + new Vector3(0,tile.transform.localScale.y/2,0) + new Vector3(0, transform.localScale.y, 0);
		}
	}

	public IEnumerator lerpTo(GameObject tile) {
		TileController t = tile.transform.GetComponent<TileController> ();
		if(t.isKingTile){
			player.setKing();
		}
		


		int oldX = x;
		int oldZ = z;
		TileController tileController = tile.GetComponent<TileController>();
		moveTo(tile, false);
		yield return StartCoroutine(movePhysically(oldX, oldZ, tileController.x, tileController.z));
	}

	public void moveToCoords(int xCoord, int zCoord) {
		moveTo (board.getCellAt (xCoord, zCoord));
	}

	public IEnumerator lerpToCoords(int xCoord, int zCoord) {
		yield return StartCoroutine(lerpTo(board.getCellAt (xCoord, zCoord)));
	}

	public virtual List<GameObject> getMoveLocations() {
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
			TileController tc = tile.GetComponent<TileController>();
			tc.setColor(onOrOff ? baseColor : tc.baseColor);
		}
	}

	public virtual List<GameObject> getAttackableTiles() {
		return getMoveLocations();
	}

	public virtual List<Piece> getAttackablePieces() {
		List<Piece> pieces = new List<Piece>();
		List<GameObject> tiles = getAttackableTiles();
		foreach (GameObject tile in tiles) {
			TileController controller = tile.GetComponent<TileController>();
			Piece other = board.getPieceAt(controller.x, controller.z);
			if (other && other.player != this.player) {
				pieces.Add(other);
			}
		}

		return pieces;
	}
	
	/*
	public virtual void setAttackHighlights(bool onOrOff) {
		attacksHighlighted = onOrOff;
		foreach (Piece piece in getAttackablePieces()) {
			GameObject tile = board.getCellAt(piece.x, piece.z);
			//piece.setColor(onOrOff ? Color.yellow : piece.baseColor);
		}
	}
	*/
  public virtual void setAttackHighlights(bool onOrOff, List<GameObject> tiles = null) {
  	if (tiles == null) {
  		tiles = getAttackableTiles();
  	}
    attacksHighlighted = onOrOff;
    foreach (Piece piece in getAttackablePieces()) {
      GameObject tile = board.getCellAt(piece.x, piece.z);
      piece.setColor(onOrOff ? Color.yellow : piece.baseColor);
    }
    foreach (GameObject tile in tiles) {
      TileController tc = tile.GetComponent<TileController>();
      tc.setColor(onOrOff ? Color.cyan : tc.baseColor);
    }
  }

	public void setColor(Color color) {
		//gameObject.renderer.material.color = color;
	}

	public void die() {
		currentHP = 0;
		dead = true;
		gameObject.transform.position = new Vector3(0,-100000,0);
		TileController tile = board.getCellAt (this.x, this.z).transform.GetComponent<TileController> ();
		if (tile.isKingTile) {
			player.removeKing();
		}
		board.removePiece(this);
		player.removePiece(this);
	}

	/*
	public IEnumerator flashHealthBar() {
		healthBar.showBar = true;
		yield return new WaitForSeconds(1);
		healthBar.showBar = false;
	}
	*/

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
		healthBar.currentHP = currentHP;
		healthBar.showBar = true;
		//StartCoroutine(flashHealthBar());
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

	public void incrementSpecial() {
		int index = Random.Range(0, specialHistogram.Length);
		int val = specialHistogram[index];

		currentSpecial = Mathf.Min(maxSpecial, currentSpecial + val);
	}

	public virtual IEnumerator specialAttack() {
		if (dead || currentSpecial != maxSpecial) {
			yield return null;
		} else {
			currentSpecial = 0;
			GameObject selected = null;

			List<GameObject> allTiles = new List<GameObject>();
			for (int i = 0; i < board.xDimension; i++) {
				for (int j = 0; j < board.zDimension; j++) {
					allTiles.Add(board.getCellAt(i,j));
				}
			}

			setAttackHighlights(true, allTiles);

			while (!selected || !selected.GetComponent<TileController>()) {
				yield return null;
				while (!Input.GetMouseButtonDown(0)) {
					yield return null;
				}
				selected = getSelectedObject("Tile");
			}
			TileController tile = selected.GetComponent<TileController>();

			setAttackHighlights(false, allTiles);
			
			for (int i = -specialRange; i <= specialRange; i++) {
				for (int j = -specialRange; j <= specialRange; j++) {
					if (Mathf.Abs(i)+Mathf.Abs(j) <= specialRange) {
						GameObject attackedCell = board.getCellAt(tile.x+i, tile.z+j);
						if (attackedCell) {
							TileController attackedTile = attackedCell.GetComponent<TileController>();
							attackedTile.setColor(Color.red);
						}
					}
				}
			}
			yield return new WaitForSeconds(1);
			
			for (int i = -specialRange; i <= specialRange; i++) {
				for (int j = -specialRange; j <= specialRange; j++) {
					if (Mathf.Abs(i)+Mathf.Abs(j) <= specialRange) {
						Piece attackedPiece = board.getPieceAt(tile.x+i, tile.z+j);
						GameObject attackedCell = board.getCellAt(tile.x+i, tile.z+j);
						if (attackedPiece) {
							attackedPiece.takeDamage(specialStrength / (Mathf.Abs(i)+Mathf.Abs(j)+1));
						}
						if (attackedCell) {
							TileController attackedTile = attackedCell.GetComponent<TileController>();
							attackedTile.setColor(attackedTile.baseColor);
						}
					}
				}
			}		
		}
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
				selected = getSelectedObject("Tile");
			}
			setMoveHighlights(false, moveLocations);
			yield return StartCoroutine(lerpTo(selected));
		}
	}

	public virtual IEnumerator attack() {
		if (dead) {
			yield return null;
		} else {
			List<Piece> attackablePieces = getAttackablePieces();
			if (attackablePieces.Count == 0) {
				// If no attacks, just move on
				yield return null;
			} else {
				game.getInstructionText(3);
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

	public IEnumerator moveOrCharge() {
		if (dead) {
			yield return null;
		} else {
			decidingToMoveOrCharge = true;
			// We'll make the decision
			while (decidingToMoveOrCharge) {
				yield return null;
			}
			if (optionChosen == "Move") {
				yield return StartCoroutine(makeMove());
			} else if (optionChosen == "Charge") {
				incrementSpecial();
			} else if (optionChosen == "SpecialAttack") {
				yield return StartCoroutine(specialAttack());
			} else {
				Debug.Log("Unknown option " + optionChosen + " chosen for piece action.");
			}
		}
	}


	public IEnumerator initialPlacement(int id) {

		if (board == null) {
			board = GameObject.Find("Game").GetComponent<GridController> ();
		}



		Debug.Log ("initialPlacement Start");
		List<GameObject> moveLocations = getInitialLocations(id);
		setMoveHighlights(true, moveLocations);
		GameObject hoveredOver = null;
		while (!moveLocations.Contains(hoveredOver)) {
			yield return null;
			while (!Input.GetMouseButtonDown(0)) {
				GameObject temp = getSelectedObject();
				if (moveLocations.Contains(temp)) {
					hoveredOver = temp;
					moveTo(hoveredOver);
				}
				yield return null;
			}
		}
		setMoveHighlights(false, moveLocations);
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

	public void setFlashing(bool shouldBeFlashing) {
		flashing = shouldBeFlashing;

	}
	
	public void startFlashing() {
		setFlashing(true);
		flash = flashingTimer;
	}
	
	public void stopFlashing() {
		setFlashing(false);

	}

	void OnMouseOver() {
		showGUI = true;
		if (healthBar) {
			healthBar.showBar = true;
		}
		guiT = guiTimer;
	}


	void OnGUI() {
		//windowRect = new Rect(Input.mousePosition.x - Input.mousePosition.x%10, Input.mousePosition.y - Input.mousePosition.y%10, 250, 300);
		if (showGUI) {
			windowRect = GUI.Window (0, windowRect, DoMyWindow, "Piece Info");
		}
		//Vector2 targetPos = Camera.main.WorldToScreenPoint (transform.position);
		//GUI.Box(new Rect(targetPos.x-20, targetPos.y, 40, 10), "foo");

		if (decidingToMoveOrCharge) {
			if (GUI.Button(new Rect(10, 60, 300, 40), "Move")) {
				decidingToMoveOrCharge = false;
				optionChosen = "Move";
			}
			string specialText = currentSpecial == maxSpecial ? "SpecialAttack" : "Charge";
			if (GUI.Button(new Rect(10, 100, 300, 40), specialText)) {
				decidingToMoveOrCharge = false;
				optionChosen = specialText;
			}
		}
	}

	void DoMyWindow(int windowID) {
		GUI.skin.label.alignment = TextAnchor.MiddleLeft;;
		GUI.Label (new Rect (10, 20, 300, 40), "Type: " + getName());
		GUI.Label (new Rect (10, 40, 300, 40), "Attack Range: " + attackRange);
		GUI.Label (new Rect (10, 60, 300, 40), "Movement Range: " + movementRange);
		GUI.Label (new Rect (10, 80, 300, 40), "Hit Points: " + currentHP + "/" + maxHP);
		GUI.Label (new Rect (10, 100, 300, 40), "Special Points: " + currentSpecial + "/" + maxSpecial);

		GUI.Label (new Rect (10, 120, 300, 40), "Max Attack: " + maxSkill(attackHistogram));
		GUI.Label (new Rect (10, 140, 300, 40), "Average Attack: " + averageSkill(attackHistogram));
		GUI.Label (new Rect (10, 160, 300, 40), "Max Shield: " + maxSkill(defenseHistogram));
		GUI.Label (new Rect (10, 180, 300, 40), "Average Shield: " + averageSkill(defenseHistogram));
		GUI.Label (new Rect (10, 200, 300, 40), "Max Special: " + maxSkill(specialHistogram));
		GUI.Label (new Rect (10, 220, 300, 40), "Average Special: " + averageSkill(specialHistogram));
		GUI.Label (new Rect (10, 250, 100, 40), "Turns: ");

		for(int i = 0; i < numMarkers; i++){
			GUI.DrawTexture(new Rect(75+(50*i), 250, 40, 40), orderMarker);
		}

	}

	float averageSkill(int[] skill){
		int counter = 0;
		int sum = 0;
		for(int i = 0; i < skill.Length; i++){
			sum += skill[i];
		}
		return Mathf.Round(((float)sum/(float)skill.Length)*100f)/100f;
	}
	
	float maxSkill(int[] skill){
		int max = 0;
		for(int i = 0; i < skill.Length; i++){
			if(skill[i] > max){
				max = skill[i];
			}
		}
		return max;
	}

	string getName() {

		int index = this.name.IndexOf ("(");
		return this.name.Substring(0, index);
	}




}
