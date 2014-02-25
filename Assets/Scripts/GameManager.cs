using UnityEngine;using System.Collections;


public class GameManager : MonoBehaviour {

	public Player player;
	public Player[] players;
	private int numPlayers = 2; 
	private bool playerTurn= true;
	private int stage = 0;
	public Camera camera;
	public Piece[,] playersPieces;
	public int numberOfPlayersPieces = 3;
	private int[] currentNumberOfPlayersPieces;
	private string gameOverText = "";
	private bool gameIsOver = false;
	public UnitManager manager;

	void Awake() {

	}

	// Use this for initialization
	void Start() {
		manager = GameObject.Find("UnitManager").GetComponent<UnitManager>();
		numberOfPlayersPieces = manager.armySize;
		Debug.Log("Game initialized");
		for (int i = 0; i < numPlayers; i++) {
			int x = 0;
			int y = 0;
			int z = 0;
			Vector3 v = new Vector3 (x, y, z);
			players[i] = (Player)Instantiate (player, v, Quaternion.identity);
			players[i].Initialize (i, camera, this, manager);
		}
		playersPieces = new Piece[numPlayers, numberOfPlayersPieces];
		currentNumberOfPlayersPieces = new int[numPlayers];
		for (int i = 0; i < numPlayers; i++) {
			currentNumberOfPlayersPieces[i] = numberOfPlayersPieces;
		}
		StartCoroutine (mainLoops ());
	}

	public IEnumerator mainLoops() {


		for (int i = 0; i < numPlayers; i++) {
			yield return StartCoroutine(changeCameraPosition (i));
			yield return StartCoroutine(players[i].setUpPieces());
	   	}

		while (allPlayersHavePieces()) {



						

						/////Stage 1: Piece Selection
						for (int i = 0; i < numPlayers; i++) {
								yield return StartCoroutine(changeCameraPosition (i));
								yield return StartCoroutine (players [i].choosePieces ());
						}

			Debug.Log ("start");
						////Stage 2: Piece Movement and Attack
						for (int i = 0; i < numPlayers; i++) {
							currentNumberOfPlayersPieces[i] = numberOfPlayersPieces;
						}
						for (int j = 0; j < numberOfPlayersPieces; j++) {
								for (int i = 0; i < numPlayers; i++) {
									if(playersPieces[i, j] != null){
										yield return StartCoroutine(changeCameraPosition (i));
										if (playersPieces [i, j].dead) {
												currentNumberOfPlayersPieces [i]--;
										} else {
											playersPieces [i, j].setColor(Color.grey);				
											yield return StartCoroutine (playersPieces [i, j].makeMove ()); 
											yield return StartCoroutine (playersPieces [i, j].attack ());
											playersPieces [i, j].setColor(playersPieces [i, j].baseColor);
										}
									}	
								}
						}
						///Reset
						for (int i = 0; i < numPlayers; i++) {
							players[i].reset ();
						}
									
		Debug.Log ("ENd");

		}

		gameOver();
	}

	private bool allPlayersHavePieces() {

		for (int i = 0; i < numPlayers; i++) { 
			if (currentNumberOfPlayersPieces [i] == 0) {
					return false;
			}
		}
		return true;
	}

	private void gameOver() {
		int winner = 1;
		for (int i = 0; i < numPlayers; i++) {
			if (currentNumberOfPlayersPieces[i] > 0) {
				winner = i+1;
			}
		}
		gameOverText = "Game Over: Player " + winner + " Wins! :D :D";
		gameIsOver = true;
	}

	void OnGUI() {
		if (gameIsOver) {
			GUI.Label(new Rect(Screen.width/2 - 100, Screen.height/2-10, 200, 20), gameOverText, GUI.skin.textArea);
		}
	}
	// Update is called once per frame
	void Update () {





	}

	public IEnumerator changeCameraPosition(int player) {
		Vector3 oldPos = camera.transform.position;
		Quaternion oldRot = camera.transform.rotation;
		Vector3 newPos = new Vector3(0,0,0);
		Quaternion newRot = Quaternion.Euler(0,0,0);
		if (player == 0) {
			newPos = new Vector3(10, 10, -5);
			newRot = Quaternion.Euler(45, 0, 0);
		} else if (player == 1) {
			newPos = new Vector3(10, 10, 25);
			newRot = Quaternion.Euler(45, 180, 0);
		}

		int numSteps = 40;
		for (int i = 1; i <= numSteps; i++) {
			camera.transform.position = Vector3.Lerp(oldPos, newPos, ((float) i / numSteps));
			camera.transform.rotation = Quaternion.Slerp(oldRot, newRot, ((float) i / numSteps));
			yield return null;
		}
	}

	public void reduceNumPieces(int p) {

				currentNumberOfPlayersPieces [p]--;

		}



}
