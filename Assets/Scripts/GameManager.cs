﻿using UnityEngine;using System.Collections;


public class GameManager : MonoBehaviour {
	
	public Player player;
	public Player[] players;
	private int numPlayers = 2; 
	private bool playerTurn= true;
	private int stage = 0;
	public Camera camera;
	public Piece[,] playersPieces;
	public int numberOfPlayersPieces = 3;
	
	void Awake() {
		
	}
	
	// Use this for initialization
	void Start() {
		Debug.Log("Game initialized");
		for (int i = 0; i < numPlayers; i++) {
			int x = 0;
			int y = 0;
			int z = 0;
			Vector3 v = new Vector3 (x, y, z);
			players[i] = (Player)Instantiate (player, v, Quaternion.identity);
			players[i].Initialize (i, camera, this);
		}
		playersPieces = new Piece[numPlayers, numberOfPlayersPieces];

		StartCoroutine (mainLoops ());
	}

	public IEnumerator mainLoops() {
		for(int j = 0; j < 2; j++) {
			for(int i = 0; i < numPlayers; i++) {
<<<<<<< HEAD
=======
				//Debug.Log ("TEST");
>>>>>>> d537077b2f35e34288fb6ad14dae55b72f85b164
				yield return StartCoroutine(chooseStage(j, i));
			}
		}
		yield return null;
	}
	
	// Update is called once per frame
	void Update () {


		 

		
	}
	
	
	public IEnumerator chooseStage(int stage, int p) {
		IEnumerator ret;
<<<<<<< HEAD
=======
		//Debug.Log ("Inside ChooseStage");
>>>>>>> d537077b2f35e34288fb6ad14dae55b72f85b164
		switch (stage) {
				
				case 0:
						yield return StartCoroutine(players[p].choosePieces());
						break;

				case 1:
						for (int i = 0; i < numberOfPlayersPieces; i++) {
								yield return StartCoroutine(playersPieces [p, i].makeMove()); 											
						}
						break;
				}
<<<<<<< HEAD
=======
		//Debug.Log ("End ChooseStage");
>>>>>>> d537077b2f35e34288fb6ad14dae55b72f85b164
		yield return null;
		
	}
	
	
}
