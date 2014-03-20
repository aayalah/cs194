using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Piece : MonoBehaviour {

	/*
	 * Variables: Public 
	 */

	//Allow communication between classes
	public GameManager game;
	public Player player;
	public GridController board;

	public Texture orderMarker;
	public Texture boxTex;
	public string id;
	public int teamNo;
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
	public int specialStrength = 50;
	public int experience = 0;

	public Color baseColor = Color.white;
	public bool dead = false;
	public int x = 0;
	public int z = 0;
	public bool movesHighlighted = false;
	public bool attacksHighlighted = false;
	public int numMarkers = 0;
	public int LERP_STEPS_PER_TILE = 12;
	public int attackFor = -1;
	public int shieldFor = -1;
	public int chargeFor = -1;
	public bool specialAttacking;
	public float timeOfDisplay = 0;
	public float timer = 0;
	public int counter = 0;

	/*
	 * Variables: Private 
	 */

	//Allow communication between classes
	private HealthBar healthBar;

	class MessageInfo {
		public string message;
		public int framesAlive;
	};

	public GameObject explosion;

	private List<MessageInfo> messages = new List<MessageInfo>();
	public int messageHeight = 20;
	public int messageWidth = 50;
	public int messageLifetime = 30;
	public float messageHeightPerFrame = 1.0f;

	private Rect windowRect = new Rect(20, 100, 250, 300);
	private bool flashing = false;
	private int flashingTimer = 5;
	private int flash;
	private bool showGUI = false;
	private int guiTimer = 10;
	private int guiT;
	private int specialTimer = 3;
	private float lastMoveTime;
	private float startingY;
	private float direction = 0;
	private bool showcaseRotate = false;
	protected bool decidingToAttackOrCharge = false;
	protected string optionChosen = "Move";
	private Vector3 startingRotation;

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

		if(attackFor != -1 || shieldFor != -1 || chargeFor != -1 || specialAttacking){
			if(counter < 100){
				counter++;
			}else{
				counter = 0;
				attackFor = -1;
				shieldFor = -1;
				chargeFor = -1;
				specialAttacking = false;
			}
		}
		//if(notMoving){
			idle();
		//}

	}

	// Idle floating animation
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

	// Return the object the user's pointing at (or clicking on), 
	// if it matches the given tag.
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

	// Smoothly move from oldPos to newPos
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

	// Move up, then over, then down, from a given old position to a given new one.
	public IEnumerator movePhysically(int oldX, int oldZ, int newX, int newZ) {
		float halfPieceHeight = 1.5f;//transform.localScale.y / 2; // This is no longer accurate with meshes
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


	// Move this piece's internal, code representation to the given tile. 
	// (basically a subroutine for lerpTo)
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

	// Move this piece's internal representation AND in-game object to the 
	// given tile.
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

	// moveTo, but using the tile's coordinates instead of the object itself.
	public void moveToCoords(int xCoord, int zCoord) {
		moveTo (board.getCellAt (xCoord, zCoord));
	}

	// lerpTo, but using the tile's coordinates instead of the object itself.
	public IEnumerator lerpToCoords(int xCoord, int zCoord) {
		yield return StartCoroutine(lerpTo(board.getCellAt (xCoord, zCoord)));
	}

	// Returns a list of tiles to which this piece can move.
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

	// Highlights the tiles to which this piece can move.
	public void setMoveHighlights(bool onOrOff, List<GameObject> locations = null) {
		if (locations == null) {
			locations = getMoveLocations();
		}
		movesHighlighted = onOrOff;
		foreach (GameObject tile in locations) {
			tile.GetComponent<TileController>().setFlashing(onOrOff);
			TileController tc = tile.GetComponent<TileController>();
			tc.setColor(onOrOff ? baseColor : tc.baseColor);
		}
	}

	// Returns a list of tiles which this piece can attack
	// (default implementation, to be overridden by subclasses)
	public virtual List<GameObject> getAttackableTiles() {
		return getMoveLocations();
	}

	// Returns a list of pieces which this piece can attack
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

	public bool hasAttackablePieces() {
		return getAttackablePieces().Count > 0;
	}
	

	// Highlights the tiles which this piece can attack (or the provided alternate list, 
	// if there is one)
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

	// Set HP to 0, remove from play
	public void die() {
		currentHP = 0;
		dead = true;
		setAttackHighlights(false);
		setMoveHighlights(false);
		gameObject.transform.position = new Vector3(0,-100000,0);
		TileController tile = board.getCellAt (this.x, this.z).transform.GetComponent<TileController> ();
		if (tile.isKingTile) {
			player.removeKing();
		}
		board.removePiece(this);
		player.removePiece(this);
	}

	// Calculate a shield value to see whether damage gets shielded. If so, block damage; 
	// if not, reduce HP.
	public void takeDamage(int damage) {
		int index = Random.Range(0, defenseHistogram.Length);
		int shield = defenseHistogram[index];

		shieldFor = shield/2;
		timeOfDisplay = Time.deltaTime;
		if(shield/2 >= damage){
			damage = 0;
		} 

		Debug.Log("Remaining HP: " + (currentHP - damage));
		currentHP = Mathf.Max (0, currentHP - damage);
		healthBar.currentHP = currentHP;
		healthBar.showBar = true;
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
		attackFor = damage;
		timeOfDisplay = Time.deltaTime;
		other.takeDamage(damage);
	}

	/******************************
	 * STUFF REQUIRING PLAYER INPUT
	 * ****************************/

	// "Charge" the special attack -- after enough charges it will be usable
	public void incrementSpecial() {
		int index = Random.Range(0, specialHistogram.Length);
		int val = specialHistogram[index];
		chargeFor = val;
		currentSpecial = Mathf.Min(maxSpecial, currentSpecial + val);
	}

	public bool canUseSpecial() {
		return currentSpecial >= maxSpecial;
	}

	// Subroutine of the specialAttack() methods -- create an explosion effect 
	// at the given tile, and do damage to the pieces near it.
	protected virtual IEnumerator specialDoDamage(TileController tile) {
		GameObject exp = (GameObject) Instantiate(explosion, tile.transform.position, explosion.transform.rotation);
		specialAttacking = true;

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
		yield return new WaitForSeconds(3.0f);
		
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
		Destroy(exp);
	}

	// AI special-attacks the enemy with highest HP.
	public virtual IEnumerator AIspecialAttack() {
		if (dead || currentSpecial != maxSpecial) {
			yield return null;
		} else {
			currentSpecial = 0;
			GameObject selected = null;

			List<GameObject> allTiles = new List<GameObject>();
			GameObject cellToAttack = null;
			int enemyMaxHP = -1;

			for (int i = 0; i < board.xDimension; i++) {
				for (int j = 0; j < board.zDimension; j++) {
					GameObject cell = board.getCellAt(i,j);
					allTiles.Add(cell);
					Piece p = board.getPieceAt(i,j);
					if (p && p.player != this.player && p.currentHP > enemyMaxHP) {
						enemyMaxHP = p.currentHP;
						cellToAttack = cell;
					}
				}
			}

			setAttackHighlights(true, allTiles);

			if (!dead) {
				TileController tile = cellToAttack.GetComponent<TileController>();
				setAttackHighlights(false, allTiles);
				yield return StartCoroutine(specialDoDamage(tile));
				yield return new WaitForSeconds(1.5f);
			}
		}
	}

	// Select a tile and special-attack whatever's near it.
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
				if (dead) {
					break;
				}
				yield return null;
				while (!Input.GetMouseButtonDown(0)) {
					yield return null;
				}
				selected = getSelectedObject("Tile");
			}
			if (selected) {
				TileController tile = selected.GetComponent<TileController>();

				setAttackHighlights(false, allTiles);
				
				yield return StartCoroutine(specialDoDamage(tile));
			}
		}
	}

	// Scoring for King of the Hill
	private int scoreLocation(GameObject location){
		TileController tileController = location.GetComponent<TileController>();
		int score = 0;
		int tempX = this.x;
		int tempZ = this.z;
		this.x = tileController.x;
		this.z = tileController.z;

		if(teamNo == 0 && z <= board.zDimension/2){
			score += (z-tempZ)/2;
			//score += Mathf.Abs(tempX-x)/3;
		}
		if(teamNo == 1 && z >= board.zDimension/2){
			score += (tempZ-z)/2;
			//score += Mathf.Abs(tempX-x)/3;
		}

		List<Piece> enemiesInRange = getAttackablePieces();
		List<GameObject> attackableTiles = getAttackableTiles();
		if(enemiesInRange.Count > 0){
			score += 10;
		}
		for(int i = 0; i < enemiesInRange.Count; i++){
			Piece enemy = enemiesInRange[i];
			if(enemy.currentHP <= maxSkill(attackHistogram)){
				score += 10;
			}else if(enemy.currentHP <= maxSkill(attackHistogram)*2){
				score += 5;
			}
		}
		for(int i = 0; i < attackableTiles.Count; i++){
			TileController tc = attackableTiles[i].GetComponent<TileController>();
			score += Mathf.Abs(tc.x-x)/2;
			score += Mathf.Abs(tc.z-z)/2;
		}

		this.x = tempX;
		this.z = tempZ;
		return score;
	}

	// AI moves to whatever tile scoreLocation() says is best.
	public IEnumerator AImakeMove(){
		if (dead) {
			yield return null;
		} else { 
		List<GameObject> moveLocations = getMoveLocations();
		int rand = Random.Range(0, moveLocations.Count);
		int bestScore = 0;
		GameObject bestLocation = moveLocations[rand];
		for(int i = 0; i < moveLocations.Count; i++){
			GameObject location = moveLocations[i];
			int score = scoreLocation(location);
			if(score > bestScore){
				bestLocation = location;
				bestScore = score;
			}
		}
		if(bestScore == 0) bestLocation = moveLocations[rand];
			setMoveHighlights(true, moveLocations);
			yield return new WaitForSeconds(1.5f);
			if (!dead) {
				yield return StartCoroutine(lerpTo(bestLocation));
				setMoveHighlights(false, moveLocations);
				yield return new WaitForSeconds(1.5f);
			}
		}
	}

	// Select a tile to move to, then move there
	public IEnumerator makeMove() {
		if (dead) {
			yield return null;
		} else { 
			List<GameObject> moveLocations = getMoveLocations();
			setMoveHighlights(true, moveLocations);
			GameObject selected = null;
			while (!moveLocations.Contains(selected)) {
				if (dead) {
					break;
				}
				yield return null;
				while (!Input.GetMouseButtonDown(0)) {
					yield return null;
				}
				selected = getSelectedObject("Tile");
			}
			setMoveHighlights(false, moveLocations);
			if (selected) {
				yield return StartCoroutine(lerpTo(selected));
			}
		}
	}

	// AI randomly chooses to charge special, use special, or attack 
	// normally. If attacking normally it attacks the piece with 
	// lowest HP.
	public virtual IEnumerator AIattackOrCharge(){
		List<Piece> attackablePieces = getAttackablePieces();
		if (dead || attackablePieces.Count == 0) {
			yield return null;
		} else {
			setAttackHighlights(true);
			if (Random.value < 0.75) {
				// attack
				int lowestHP = 100;
				Piece choice = attackablePieces[0];
				for(int i = 0; i < attackablePieces.Count; i++){
					Piece piece = attackablePieces[i];
					if (piece.currentHP < lowestHP){
						lowestHP = piece.currentHP;
						choice = piece;
					}
				}
				if (!dead) {
					damageEnemy(choice);
					yield return new WaitForSeconds(1.5f);
				}
			} else {
				// charge
				yield return new WaitForSeconds(0.5f);
				if (!dead) {
					if (currentSpecial < maxSpecial) {
						incrementSpecial();
					} else {
						yield return StartCoroutine(AIspecialAttack());
					}
				}
			}
			setAttackHighlights(false);
			if (!dead) {
				yield return new WaitForSeconds(1.5f);
			}
		}
	}

	// Choose whether to attack a piece or charge/use special attack.
	public virtual IEnumerator attackOrCharge() {
		List<Piece> attackablePieces = getAttackablePieces();
		if (dead || attackablePieces.Count == 0) {
			yield return null;
		} else {
			game.getInstructionText(3);
			setAttackHighlights(true);
			GameObject selectedObject = null;
			Piece selectedPiece = null;
			while (!attackablePieces.Contains(selectedPiece)) {
				if (dead) {
					break;
				}
				yield return null;
				while (!Input.GetMouseButtonDown(0) && !Input.GetKeyUp("space")) {
					yield return null;
				}
				if (Input.GetMouseButtonDown(0)) {
					selectedObject = getSelectedObject();
					if (selectedObject) {
						selectedPiece = selectedObject.GetComponent<Piece>();
					}
				} else if (Input.GetKeyUp("space")) {
					if (currentSpecial < maxSpecial) {
						incrementSpecial();
					} else {
						yield return StartCoroutine(specialAttack());
					}
					break;
				}
			}
			if (selectedPiece) {
				damageEnemy(selectedPiece);
			}
		}
		setAttackHighlights(false);
		yield return new WaitForSeconds(1);
	}

	// AI places pieces randomly.
	public IEnumerator AIinitialPlacement(int id){
		if (board == null) {
			board = GameObject.Find("Game").GetComponent<GridController> ();
		}
		List<GameObject> moveLocations = getInitialLocations(id);
		setMoveHighlights(true, moveLocations);
		yield return new WaitForSeconds(2f);
		int index = Random.Range(0, moveLocations.Count);
		moveTo(moveLocations[index]);
		setMoveHighlights(false, moveLocations);
	}

	// Place pieces in their starting positions on the board.
	public IEnumerator initialPlacement(int id) {

		if (board == null) {
			board = GameObject.Find("Game").GetComponent<GridController> ();
		}

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
	}

	// Returns a list of the available locations for a starting piece.
	public List<GameObject> getInitialLocations(int player) {
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
		if(healthBar != null) {
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

		// Labels that appear after attacks/shields/specials:
		if(attackFor != -1){
			Vector2 targetPos = Camera.main.WorldToScreenPoint (transform.position);
			GUIStyle style = new GUIStyle ();
			style.fontSize = 20;
			style.normal.textColor = Color.red;
			GUI.Label(new Rect(targetPos.x - 50, Screen.height - targetPos.y - 100, 75, 10), "ATTACK FOR: " + attackFor, style);
		}

		if(shieldFor != -1){
			Vector2 targetPos = Camera.main.WorldToScreenPoint (transform.position);
			GUIStyle style = new GUIStyle ();
			style.fontSize = 20;
			style.normal.textColor = Color.blue;
			GUI.Label(new Rect(targetPos.x - 50, Screen.height - targetPos.y - 100, 75, 10), "SHIELD FOR: " + shieldFor, style);
		}

		if(chargeFor != -1){
			Vector2 targetPos = Camera.main.WorldToScreenPoint (transform.position);
			GUIStyle style = new GUIStyle ();
			style.fontSize = 20;
			style.normal.textColor = Color.yellow;
			GUI.Label(new Rect(targetPos.x - 50, Screen.height - targetPos.y - 100, 75, 10), "CHARGE FOR: " + chargeFor, style);
		}

		if(specialAttacking){
			Vector2 targetPos = Camera.main.WorldToScreenPoint (transform.position);
			GUIStyle style = new GUIStyle ();
			style.fontSize = 20;
			style.normal.textColor = Color.yellow;
			GUI.Label(new Rect(targetPos.x - 50, Screen.height - targetPos.y - 100, 75, 10), "FIRING SPECIAL", style);
		}
	}

	// Unit stats screen.
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
