using UnityEngine;using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	/*
	 * Variables: Public 
	 */
	//Static Variables

	//Non-Static Variables
	public GUISkin skin;

	public Texture up;
	public Texture down;
	private static string piecePlacementText = "Click on one of the colored squares to place each of your pieces.";
	private static string pieceOrderSelectionText = "Click on your pieces in the order in which you want to move them. You can click on a piece more than once.";
	private static string pieceMovementText = "Click on a colored square to move your piece."; 
	private static string pieceAttackText = "Click on an enemy within range to attack or press SPACE to charge special";

	public Player player;
	public Camera camera;
	public UnitManager manager;
	public Clock clock;	
	private bool hintsHidden;

	public Player[] players;
	public Piece[,] playersPieces;


	/*
	 * Variables: Private 
	 */

	///Static Variables
	//Help Messages

	//boolean variables
	private bool gameIsOver = false;
	private bool playerTurn= true;
	private bool showInstructionLabel = false;
	private bool showTurnLabel = false;
	private bool[] orderFixed;
	private bool showFixedOrderGui = false;
	private bool showHelpWindow = false;

	private Rect windowRect = new Rect(Screen.width/2 - 300, 70, 600, 120);
	private int currentPlayersTurn = -1;
	private int numPlayers = 2; 
	private int stage = 0;
	private int[] currentNumberOfPlayersPieces;
	private string gameOverText = "";
	private string turnText;
	private string instructionText;
	private int numberSelectedPlayersPieces;



	void Awake() {


	}

	
	// Update is called once per frame
	void Update () {
		
		
		if (Input.GetKey ("f") && currentPlayersTurn != -1) {
			showFixedOrderGui = true;
		}
		
	}

	// Use this for initialization
	void Start() {
		//Setup
		manager = GameObject.Find("UnitManager").GetComponent<UnitManager>();
		if(!manager.kingMode) Destroy(clock);
		numberSelectedPlayersPieces = manager.turnsPerRound;
		//playersPieces = new Piece[numPlayers, numberOfPlayersPieces];
		//currentNumberOfPlayersPieces = new int[numPlayers];

		orderFixed = new bool[numPlayers];
		for (int i = 0; i < numPlayers; i++) {
			orderFixed[i] = false;
			//currentNumberOfPlayersPieces[i] = numberOfPlayersPieces;
		}

		Debug.Log("Game initialized");

		//Setup Players
		for (int i = 0; i < numPlayers; i++) {
			Vector3 v = new Vector3 (0, 0, 0);
			players[i] = (Player)Instantiate (player, v, Quaternion.identity);
			players[i].Initialize (i, camera, this, manager);
		}
		//Start Game Loop
		StartCoroutine (mainLoops ());
	}

	public IEnumerator mainLoops() {

		setInstructionText(0);
		stage = 0;
		for (int i = 0; i < numPlayers; i++) {
			setTurnText(i);
			yield return StartCoroutine(changeCameraPosition (i));
			yield return StartCoroutine(players[i].setUpPieces());
	   }

		while (allPlayersHavePieces()) {

			setInstructionText(1);
			/////Stage 1: Piece Selection
			stage = 1;
			for (int i = 0; i < numPlayers; i++) {
					yield return StartCoroutine(changeCameraPosition (i));
					setTurnText(i);
					if(!orderFixed[i]) {
						yield return StartCoroutine (players [i].choosePieces ());	
					}
					
			}

			////Stage 2: Piece Movement and Attack
			/*for (int i = 0; i < numPlayers; i++) {
				currentNumberOfPlayersPieces[i] = numberOfPlayersPieces;
			}*/

			for (int j = 0; j < numberSelectedPlayersPieces; j++) {
				for (int i = 0; i < numPlayers; i++) {
					List<Piece> playersPieces = players[i].getSelectedPieces();
					//currentPlayersTurn = i;
					if(playersPieces.Capacity >= j){
						setTurnText(i);
						setInstructionText(2);
						yield return StartCoroutine(changeCameraPosition (i));
						playersPieces[j].setColor(Color.grey);
						stage = 2;
						yield return StartCoroutine (playersPieces[j].makeMove ()); 
						yield return StartCoroutine (playersPieces[j].attack ());
						playersPieces[j].setColor(playersPieces[j].baseColor);
						playersPieces [j].numMarkers--;
					}
				}	
			}

			///Reset
			for (int i = 0; i < numPlayers; i++) {
				players[i].reset ();
				players[i].incrementClock();
			}

		}


		gameOver(1);
	}

	private bool allPlayersHavePieces() {

		for (int i = 0; i < numPlayers; i++) { 
			if (!players[i].hasPieces()) {
				return false;
			}
		}
		return true;
	}

	public void gameOver(int c) {
		int winner = 1;
		if (c == 0) {
						int maxNumPieces = 0;
						for (int i = 0; i < numPlayers; i++) {
								if (players [i].numberPiecesLeft () > maxNumPieces) {
										maxNumPieces = players [i].numberPiecesLeft ();
										winner = i + 1;
								}
						}
				} else if (c == 1) {
						for (int i = 0; i < numPlayers; i++) {
								if (players [i].hasPieces ()) {
										winner = i + 1;
								}
						}
				} else if (c == 2) {
					
					for (int i = 0; i < numPlayers; i++) {
						if (players[i].hasReachedGoal()) {							
							winner = i + 1;
						}
					}
				}
		gameOverText = "Game Over: Player " + winner + " Wins! :D :D";
		gameIsOver = true;
	}

	private void setTurnText(int p) {
		showTurnLabel = false;
		turnText = "Player: " + (p+1) + "'s Turn";
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

		GUI.skin = skin;
		if (gameIsOver) {
			GUI.Label(new Rect(Screen.width/2 - 100, Screen.height/2-10, 200, 20), gameOverText, GUI.skin.textArea);
		}
		if(!hintsHidden){
			if (showTurnLabel) {
				GUI.Box(new Rect(Screen.width/2-500,0, 1000, 75), "", GUI.skin.GetStyle("box"));
				GUI.contentColor = Color.yellow;
				GUI.skin.label.alignment = TextAnchor.MiddleCenter;
				GUI.skin.label.fontSize = 20;
				GUI.Label(new Rect(Screen.width/2 - 250, 1, 500, 50), turnText, GUI.skin.label);
			}

			if (showInstructionLabel) {
				GUI.contentColor = Color.red;
				GUI.skin.label.alignment = TextAnchor.MiddleCenter;
				GUI.skin.label.fontSize = 18;
				GUI.Label(new Rect(Screen.width/2 - 450, 25, 900, 50), instructionText, GUI.skin.label);
			}
			if(GUI.Button(new Rect(Screen.width/2+400,60, 40, 20), up, GUI.skin.GetStyle("button"))) hintsHidden = true;

		}else{
			GUI.Box(new Rect(Screen.width/2-500,0, 1000, 25), "", GUI.skin.GetStyle("box"));
			if(GUI.Button(new Rect(Screen.width/2+400,10, 40, 20), down, GUI.skin.GetStyle("button"))) hintsHidden = false;
		}

		if (showFixedOrderGui) {

			windowRect = GUI.Window (1, windowRect, pieceOrderWindow, "Fix Order");
		}

		if (showHelpWindow) {
			
			windowRect = GUI.Window (1, windowRect, helpWindow, "Help Window");
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
//
//	public void reduceNumPieces(int p) {
//
//		currentNumberOfPlayersPieces [p]--;
//
//	}
//

//	void OnGUI() {
		
//		if (gameIsOver) {
//			GUI.Label(new Rect(Screen.width/2 - 100, Screen.height/2-10, 200, 20), gameOverText, GUI.skin.textArea);
//		}
//		
//		if (showTurnLabel) {
//			//			GUI.contentColor = Color.yellow;
//			//			GUI.skin.label.alignment = TextAnchor.MiddleCenter;
//			//			GUI.skin.label.fontSize = 20;
//			//			GUI.Label(new Rect(Screen.width/2 - 200, 1, 500, 50), turnText,GUI.skin.label);
//		}
//		
//		if (showInstructionLabel) {
//			//			GUI.contentColor = Color.red;
//			//			GUI.skin.label.alignment = TextAnchor.MiddleCenter;
//			//			GUI.skin.label.fontSize = 18;
//			//			GUI.Label(new Rect(Screen.width/2 - 400, 25, 900, 50), instructionText,GUI.skin.label);
//		}
//		
//		if (showFixedOrderGui) {
//			
//			windowRect = GUI.Window (1, windowRect, pieceOrderWindow, "Fix Order");
//		}
//		


		
//	}


	void helpWindow(int windowID) {


		GUI.contentColor = Color.red;
		GUI.skin.label.alignment = TextAnchor.MiddleCenter;
		GUI.backgroundColor = Color.black;
		GUI.Label (new Rect (10, 30, 600, 45), getInstructionText(stage), GUI.skin.label);
		GUI.skin.button.alignment = TextAnchor.MiddleCenter;
		if (GUI.Button (new Rect (270, 80, 55, 30), "Close")) {
			showHelpWindow = false;
		}

	}


	
//	void pieceOrderWindow(int windowID) {
//		GUI.contentColor = Color.yellow;
//		GUI.skin.label.alignment = TextAnchor.MiddleCenter;
//		GUI.backgroundColor = Color.black;
//		GUI.Label (new Rect (10, 30, 350, 40), "Do you want to save the current order?", GUI.skin.label);
//		GUI.skin.button.alignment = TextAnchor.MiddleCenter;
//		if (GUI.Button (new Rect (60, 80, 40, 30), "Yes")) {
//			orderFixed[currentPlayersTurn] = true;
//			showFixedOrderGui = false;
//		}
//		
//		GUI.skin.button.alignment = TextAnchor.MiddleCenter;
//		
//		if(GUI.Button (new Rect (275, 80, 40, 30), "No")) {
//			orderFixed[currentPlayersTurn] = false;
//			showFixedOrderGui = false;
//		}
//	}
//
//

	public string getInstructionText(int i) {
		string instructionText = "";
		switch(i) {
			
		case 0:
			instructionText = piecePlacementText;
			break;
		case 1:
			instructionText = pieceOrderSelectionText;
			break;
		case 2:
			instructionText = pieceMovementText;
			break;
		case 3:
			instructionText = pieceAttackText;
			break;
		}

		return instructionText;
		
		
	}


}
