using UnityEngine;using System.Collections;


public class GameManager : MonoBehaviour {

	private static string piecePlacementText = "Click on one of the yellow squares to place each of your pieces.";
	private static string pieceOrderSelectionText = "Click on your pieces in the order in which you want to move them. You can click on a piece more than once.";
	private static string pieceMovementText = "Click on a colored square to move your piece."; 

	private Rect windowRect = new Rect(20, 70, 375, 120);
	public Player player;
	public Player[] players;
	private int currentPlayersTurn = -1;
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
	private bool showTurnLabel = false; 
	private string turnText;
	private bool showInstructionLabel = false;
	private string instructionText;
	private bool[] orderFixed;
	private bool showFixedOrderGui = false;

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
		orderFixed = new bool[numPlayers];
		for (int i = 0; i < numPlayers; i++) {
			orderFixed[i] = false;
			currentNumberOfPlayersPieces[i] = numberOfPlayersPieces;
		}

		StartCoroutine (mainLoops ());
	}

	public IEnumerator mainLoops() {

		setInstructionText(0);
		for (int i = 0; i < numPlayers; i++) {
			setTurnText(i);
			yield return StartCoroutine(changeCameraPosition (i));
			yield return StartCoroutine(players[i].setUpPieces());
	   }

		while (allPlayersHavePieces()) {

						

			/////Stage 1: Piece Selection
			setInstructionText(1);
			for (int i = 0; i < numPlayers; i++) {
					yield return StartCoroutine(changeCameraPosition (i));
					setTurnText(i);
					if(!orderFixed[i]) {
						
						yield return StartCoroutine (players [i].choosePieces ());	
					}
					
			}

			Debug.Log ("start");

						setInstructionText(2);
						////Stage 2: Piece Movement and Attack
						for (int i = 0; i < numPlayers; i++) {
							currentNumberOfPlayersPieces[i] = numberOfPlayersPieces;
						}
						for (int j = 0; j < numberOfPlayersPieces; j++) {
								for (int i = 0; i < numPlayers; i++) {
									currentPlayersTurn = i;
									if(playersPieces[i, j] != null){
										setTurnText(i);
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

	private void setTurnText(int p) {
		showTurnLabel = false;
		turnText = "Player: " + p + "'s Turn";
		showTurnLabel = true;
	}

	private void setInstructionText(int i) {

		switch(i) {

			case 0:
				instructionText = piecePlacementText;
				showInstructionLabel = true;
				break;
			case 1:
			instructionText = pieceOrderSelectionText;
				showInstructionLabel = true;
				break;
			case 2:
				instructionText = pieceMovementText;
				showInstructionLabel = true;
				break;
		}
	


	}

	void OnGUI() {

		if (gameIsOver) {
			GUI.Label(new Rect(Screen.width/2 - 100, Screen.height/2-10, 200, 20), gameOverText, GUI.skin.textArea);
		}

		if (showTurnLabel) {
			GUI.contentColor = Color.yellow;
			GUI.skin.label.alignment = TextAnchor.MiddleCenter;
			GUI.skin.label.fontSize = 20;
			GUI.Label(new Rect(Screen.width/2 - 200, 1, 500, 50), turnText,GUI.skin.label);
		}

		if (showInstructionLabel) {
			GUI.contentColor = Color.red;
			GUI.skin.label.alignment = TextAnchor.MiddleCenter;
			GUI.skin.label.fontSize = 18;
			GUI.Label(new Rect(Screen.width/2 - 400, 25, 900, 50), instructionText,GUI.skin.label);
		}

		if (showFixedOrderGui) {

			windowRect = GUI.Window (1, windowRect, pieceOrderWindow, "Fix Order");
		}

	}

	void pieceOrderWindow(int windowID) {
		GUI.contentColor = Color.yellow;
		GUI.skin.label.alignment = TextAnchor.MiddleCenter;
		GUI.backgroundColor = Color.black;
		GUI.Label (new Rect (10, 30, 350, 40), "Do you want to save the current order?", GUI.skin.label);
		GUI.skin.button.alignment = TextAnchor.MiddleCenter;
		if (GUI.Button (new Rect (60, 80, 40, 30), "Yes")) {
			orderFixed[currentPlayersTurn] = true;
			showFixedOrderGui = false;
		}

		GUI.skin.button.alignment = TextAnchor.MiddleCenter;

		if(GUI.Button (new Rect (275, 80, 40, 30), "No")) {
			orderFixed[currentPlayersTurn] = false;
			showFixedOrderGui = false;
		}
	}


	// Update is called once per frame
	void Update () {


		if (Input.GetKey ("f") && currentPlayersTurn != -1) {
			showFixedOrderGui = true;
		}


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
