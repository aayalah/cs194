using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {

	/*
	 * Variables: Public 
	 */

	//Allow communication between classes
	public Piece piece;
	public Camera camera;
	public UnitManager um;	
	public GameManager game;
	public ClockKing clock;


	/*
	 * Variables: Private
	 */

	//data structures for storing pieces
	private List<Piece> pieceArray;
	private List<Piece> selectedPieceArray;

	private int numPieces = 0;
	private int numberOfPieces;
	private int numSelectedPieces;
	private int id;
	private bool isKing = false;


	// Use this for initialization
	void Start () {
	
	}



	/* Returns array of the pieces the player has selected
	 */ 

	public List<Piece> getSelectedPieces() {

		return selectedPieceArray;

	}

	/* 
	 * Sets up data structures to manage the pieces for the AI players
	 * 
	 * 
	 */ 

	public IEnumerator AIsetUpPieces() {
		numberOfPieces = um.armySize;
		numSelectedPieces = um.armySize;
		pieceArray = new List<Piece>();
		selectedPieceArray = new List<Piece>();
		
		for(int i = 0; i < numberOfPieces; i++) {
			Piece piece = um.getUnit(id, i);
			piece.Initialize(this, game);
			piece.id = "player" + id;
			piece.player = this;
			piece.tag = "piece";
			pieceArray.Add (piece);
			if(id == 0) {
				pieceArray[i].baseColor = Color.red;
			} else {
				pieceArray[i].baseColor = Color.blue;
			}
			
			yield return StartCoroutine(pieceArray[i].AIinitialPlacement(id));
			
		}
		yield return new WaitForSeconds(2f);
		
		yield return null;

	}


	/* 
	 * Sets up data structures to manage the pieces for the human players
	 * 
	 * 
	 */ 

	public IEnumerator setUpPieces() {

		numberOfPieces = um.armySize;
		numSelectedPieces = um.armySize;
		pieceArray = new List<Piece>();
		selectedPieceArray = new List<Piece>();

		for(int i = 0; i < numberOfPieces; i++) {
			Piece piece = um.getUnit(id, i);
			piece.Initialize(this, game);
			piece.id = "player" + id;
			piece.player = this;
			piece.tag = "piece";
			pieceArray.Add (piece);
			if(id == 0) {
				pieceArray[i].baseColor = Color.red;
			} else {
				pieceArray[i].baseColor = Color.blue;
			}

			yield return StartCoroutine(pieceArray[i].initialPlacement(id));

		}

		yield return null;

	}


	/*
 	 * Sets up the data structurs that help it communicate with other scripts
 	 * If the King of the Hill mode is being played then the data structure for this mode is set up
     */
	public void Initialize(int id, Camera camera, GameManager game, UnitManager man){
		this.id = id;
		this.camera = camera;
		this.game = game;
		um = man;

		Vector3 v = new Vector3 (0, 0, 0);

		if(um.kingMode){
			clock = (ClockKing)Instantiate (clock, v, Quaternion.identity);
			clock.Initialize(id);
		}
	}

	public Player(){

	}
	
	public int getId() {
		return id;
	}

	
	/*
     * Manages the piece selection phase of the game for the AI players
	 *
     */

	public IEnumerator AIchoosePieces() {
		int turnsAssigned = 0;
		for(int j = selectedPieceArray.Count; j < um.turnsPerRound; j++){
			int rand = Random.Range(0, pieceArray.Count-1);
			int bestScore = 0;
			Piece bestPiece = pieceArray[rand];
			for(int i = 0; i < pieceArray.Count; i++){
				Piece piece = pieceArray[i];
				int score = 0;
				if(piece.currentHP < 20) score = (j == 0) ? score + 5 : score - 5;
				if(piece.getAttackablePieces().Count > 0) score += 10;
				if(score > bestScore){
					bestScore = score;
					bestPiece = piece;
				}
			}
			selectedPieceArray.Add (bestPiece);
			bestPiece.numMarkers++;
			numPieces++;
		}
		yield return new WaitForSeconds(2);
		//yield return StartCoroutine(choosePieces());
	}

	/*
     * Manages the piece selection phase of the game for the human players
	 *
     */
	
	public IEnumerator choosePieces() {
		numPieces = selectedPieceArray.Count;
		Piece chosenPiece = null;
		while (numPieces < um.turnsPerRound) {
			GameObject selected;
			if (Input.GetMouseButtonDown (0)) {
				Ray ray = camera.ScreenPointToRay (Input.mousePosition);
				RaycastHit hit;
				if (Physics.Raycast (ray, out hit, Mathf.Infinity)) { 
					selected = hit.transform.gameObject;
					if(selected.tag.Equals("piece")) {
						chosenPiece = (Piece)selected.GetComponent (typeof(Piece));
						string temp = chosenPiece.id;
						string test = "player" + getId();
						if (temp.Equals(test)) {
							selectedPieceArray.Add (chosenPiece);
							chosenPiece.setColor(Color.red);
							chosenPiece.numMarkers++;
							numPieces++;
						}
					}
				}

			} 
			yield return null;					
		}
		foreach(Piece p in selectedPieceArray) {
			p.setColor(p.baseColor);
		}
	}



	/*
     * Clears the array that stores the pieces that player has selected, so that it is ready fir the next round of piece selection
     */ 
	public void reset() {
		numPieces = 0;
		selectedPieceArray.Clear();

	}


	//returns if the player has any pieces left
	public bool hasPieces() {

		return pieceArray.Count > 0;

	}

	//removes one of the players pieces from the data structures that 
	//track the players pieces
	public void removePiece(Piece piece) {

		pieceArray.Remove (piece);
		for(int i = 0; i < selectedPieceArray.Count; i++) {
			selectedPieceArray.Remove (piece);
		}
	}

	//returns the number of pieces the player has left
	public int numberPiecesLeft() {

		return pieceArray.Count;
	}

	/*
	 * If the King of the Hill game mode is being played, this method calls a method
	 * on the clock object, that increments the number of turns that the player has had a piece
     * on the specified square
     */

	public void incrementClock() {

		if (isKing) {
				clock.addTurn ();
		}
	}


	/*
	 * Returns true/false if the player has been on a the specified square for the maximum number of turns.
	 * Used when King of the Hill mode is being played
	 */
	public bool hasReachedGoal() {
		return clock.reachedGoal();
	}

	/* 
     * Used when King of the Hill mode is being played
     * Sets the player as the current King 
     */
    public void setKing(){
		isKing = true;
	}

	/* 
     * Used when King of the Hill mode is being played
     * Removes the player from being King
     */
	public void removeKing(){
		isKing = false;
	}

	// Update is called once per frame
	void Update () {

		if (um.kingMode) {
			if(clock.reachedGoal()){
				game.gameOver(2);
			}
		}
		
		
	}
}
