﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {
	public Piece piece;
	private List<Piece> pieceArray;
	private List<Piece> selectedPieceArray;
	public Camera camera;
	public int numPieces = 0;
	public UnitManager um;	
	public GameManager game;
	public ClockKing clock;
	private int numberOfPieces;
	private int numSelectedPieces;
	private int id;


	// Use this for initialization
	void Start () {
	
	}

	public List<Piece> getSelectedPieces() {

		return selectedPieceArray;

	}


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

	
	//Keeps track of pieces
	
	// Keeps track of stats
	
	
	
	public Player(){

	}
	
	public int getId() {
		return id;
	}
	
	public IEnumerator choosePieces() {
		numPieces = 0;
		Piece chosenPiece = null;
		while (numPieces != um.turnsPerRound) {
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
	
//	private bool containsPiece(Piece[] playersPieces,Piece piece) {
//		
//		for(int i = 0; i < numberOfPieces; i++) {
//			if((playersPieces[i] != null) && (playersPieces[i].Equals(piece))) {
//
//				return true;
//			}
//			
//		}
//		return false;
//	}


	public void reset() {
		numPieces = 0;
		selectedPieceArray.Clear();

	}


	public bool hasPieces() {

		return pieceArray.Capacity > 0;

	}

	public void removePiece(Piece piece) {

		pieceArray.Remove (piece);
		selectedPieceArray.Remove (piece);

	}

	public int numberPiecesLeft() {

		return pieceArray.Capacity;
	}


	public void startClock() {

		clock.unpause();

	}

	public void stopClock() {
		clock.pause();
	}

	public float getTime(){

		return clock.getTimePassed ();
	
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
