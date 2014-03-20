using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	/*
	 * Variables: Public 
	 */

	public GUISkin skin;
	public Texture up;
	public Texture down;

	//Allow communication between classes

	public Player player;
	public Camera camera;
	public UnitManager manager;
	public GridController board;
	public Clock clock;	

	//data structures for storing players and pieces
	public Player[] players;
	public Piece[,] playersPieces;
	public bool[] usingAI;


	/*
	 * Variables: Private 
	 */

	//static variables
	private static string piecePlacementText = "Click on one of the colored squares to place each of your pieces.";
	private static string pieceOrderSelectionText = "Click on your pieces in the order in which you want to move them. You can click on a piece more than once.";
	private static string pieceMovementText = "Click on a colored square to move your piece."; 
	private static string pieceAttackText = "Click on an enemy within range to attack or press SPACE to charge special";
	private static string pieceAttackWithFullSpecialText = "Your special attack is charged! Press space to use it, or click to attack normally.";


	//boolean variables
	private bool gameIsOver = false;
	private bool playerTurn= true;
	private bool showInstructionLabel = false;
	private bool showTurnLabel = false;
	private bool[] orderFixed;
	private bool showFixedOrderGui = false;
	private bool showHelpWindow = false;
	private bool hintsHidden;
	
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
	/*
	 *Sets up all the data structures that are required to keep track of the players and their pieces 
	 * 
	 * 
	 */

	void Start() {

		//Setup
		manager = GameObject.Find("UnitManager").GetComponent<UnitManager>();
		board = GameObject.Find("Game").GetComponent<GridController> ();
		if(!manager.kingMode) Destroy(clock);
		numberSelectedPlayersPieces = manager.turnsPerRound;


		//determines if there are AI players
		usingAI = new bool[numPlayers];
		usingAI[0] = !manager.p1Human;
		usingAI[1] = !manager.p2Human;


		orderFixed = new bool[numPlayers];
		for (int i = 0; i < numPlayers; i++) {
			orderFixed[i] = false;
		}

		//Setup Players
		for (int i = 0; i < numPlayers; i++) {
			Vector3 v = new Vector3 (0, 0, 0);
			players[i] = (Player)Instantiate (player, v, Quaternion.identity);
			players[i].Initialize (i, camera, this, manager);
		}

		//Start Game Loop
		StartCoroutine (mainLoops ());
	
	}
	/*
	 *	Manages the main loop of the game. The game is divided into three phases: selection of pieces, 
	 *  movement or charging of special ability for each piece and attacking of pieces. At the beginning   
	 *  of the game there is an additional phase where you place each or your pieces on the board
	 */
	public IEnumerator mainLoops() {

		//Changes the instruction that appear at the top of the screen that servas a guide for the players
		setInstructionText(0);
		stage = 0;
		for (int i = 0; i < numPlayers; i++) {
			setTurnText(i);
			yield return StartCoroutine(changeCameraPosition (i));
			if(usingAI[i]){
				yield return StartCoroutine(players[i].AIsetUpPieces());
			}else{
				yield return StartCoroutine(players[i].setUpPieces());
			}
	   }
		int round = 0;

		while (allPlayersHavePieces()) {

			setInstructionText(1);
			/////Stage 1: Piece Selection
			stage = 1;
			for (int k = 0; k < numPlayers; k++) {
				int i = Mathf.Abs(k - (round % 2));
				yield return StartCoroutine(changeCameraPosition (i));
				setTurnText(i);
				if(!orderFixed[i] || (players[i].getSelectedPieces().Count != manager.numTurns)) {
					if(usingAI[i]){
						yield return StartCoroutine (players [i].AIchoosePieces());
					}else{
						yield return StartCoroutine (players [i].choosePieces ());
					}
				}	
					
			}

			for (int j = 0; j < numberSelectedPlayersPieces; j++) {
				for (int k = 0; k < numPlayers; k++) {

					int i = Mathf.Abs(k - (round % 2));
					List<Piece> playersPieces = players[i].getSelectedPieces();
					if(playersPieces.Count > j){
						setTurnText(i);
						setInstructionText(2);
						yield return StartCoroutine(changeCameraPosition (i));
						stage = 2;

						if(usingAI[i]){
							if (j < playersPieces.Count) {	// To avoid an error if the piece has just died
								yield return StartCoroutine(playersPieces[j].AImakeMove());
							}
							if (j < playersPieces.Count && playersPieces[j].hasAttackablePieces()) {
								if (playersPieces[j].canUseSpecial()) {
									setInstructionText(4);
								} else {
									setInstructionText(3);
								}
								yield return StartCoroutine(playersPieces[j].AIattackOrCharge());
							}
						}else{
							if (j < playersPieces.Count) {
								yield return StartCoroutine (playersPieces[j].makeMove()); 
							}
							if (j < playersPieces.Count && playersPieces[j].hasAttackablePieces()) {
								if (playersPieces[j].canUseSpecial()) {
									setInstructionText(4);
								} else {
									setInstructionText(3);
								}
								yield return StartCoroutine (playersPieces[j].attackOrCharge());
							}

						}

						//playersPieces[j].setColor(playersPieces[j].baseColor);
						if (j < playersPieces.Count) {
							playersPieces [j].numMarkers--;
						}
						players[k].incrementClock();
					}
				}

				if(!allPlayersHavePieces()) {
					break;
				}
			}

			///Reset
			for (int i = 0; i < numPlayers; i++) {
				if(!orderFixed[i]) {
					players[i].reset ();
				}
			}
			round++;

		}


		gameOver(1);
	}

	/*
	 * Checks to see if any of the players have any pieces left 
	 * 
	 */
	private bool allPlayersHavePieces() {

		for (int i = 0; i < numPlayers; i++) { 
			if (!players[i].hasPieces()) {
				return false;
			}
		}
		return true;
	}

	/*
	 * Runs through all the end conditions of the game and checks to see if any of them
	 * are satisfied. In the normal game mode, the game ends when one of the players does
	 * not have any pieces. In the King of the Hill game mode the game ends when one of the
	 * players has stayed on the specified square for the specified number of turns
	 */ 

	public void gameOver(int c) {
		int winner = 1;
		if (c == 1) {
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


	/*
	 * Updates the text that displays, whose turn it is, to the correct player 
     *
	 */ 
	private void setTurnText(int p) {
		showTurnLabel = false;
		turnText = "Player: " + (p+1) + "'s Turn";
		showTurnLabel = true;
	}



	/*
	 * Updates the text that describes what the player should do next to the appropriate message
	 * depending on whhat part of the round the player is currently at
	 * 
	 */

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
			case 3:
				instructionText = pieceAttackText;
				break;
			case 4:
				instructionText = pieceAttackWithFullSpecialText;
				break;
		}
	


	}


	/*
	 * Manages and creates the user interface elements, such as hint window, that appear on the screen.
	 * 
	 */
	void OnGUI() {

		GUI.skin = skin;
		if (gameIsOver) {
			GUI.Label(new Rect(Screen.width/2 - 100, Screen.height/2-10, 200, 20), gameOverText, GUI.skin.textArea);
		}
		if(!hintsHidden){
			GUIStyle style4 = new GUIStyle ();
			GUIStyle style3 = new GUIStyle ();


			if (showTurnLabel) {
				GUI.Box(new Rect(Screen.width/2-500,0, 1000, 75), "", GUI.skin.GetStyle("box"));
				style3.normal.textColor = Color.yellow;
				style3.fontSize = 20;
				style3.alignment = TextAnchor.MiddleCenter;

				GUI.Label(new Rect(Screen.width/2 - 250, 1, 500, 50), turnText, style3);
			}

			if (showInstructionLabel) {

				style4.normal.textColor = Color.red;
				style4.fontSize = 18;
				style4.alignment = TextAnchor.MiddleCenter;

				GUI.Label(new Rect(Screen.width/2 - 450, 25, 900, 50), instructionText, style4);
			}
			if(GUI.Button(new Rect(Screen.width/2+400,60, 40, 20), up, GUI.skin.GetStyle("button"))) hintsHidden = true;

		}else{
			GUI.Box(new Rect(Screen.width/2-500,0, 1000, 25), "", GUI.skin.GetStyle("box"));
			if(GUI.Button(new Rect(Screen.width/2+400,10, 40, 20), down, GUI.skin.GetStyle("button"))) hintsHidden = false;
		}


		GUIStyle style1 = new GUIStyle ();
		GUIStyle style2 = new GUIStyle ();

		style1.normal.textColor = Color.red;
		style1.fontSize = 18;
		GUI.Label (new Rect (Screen.width / 2 - 600, Screen.height - 25, 300, 40), "Fix Piece Order: ", style1);
		orderFixed[0] = GUI.Toggle(new Rect(Screen.width / 2 - 455, Screen.height - 25, 10, 40), orderFixed[0], "", GUI.skin.toggle);

		style2.normal.textColor = Color.blue;
		style2.fontSize = 18;
		GUI.Label (new Rect (Screen.width / 2 + 400, Screen.height - 25, 300, 40), "Fix Piece Order: ", style2);
		orderFixed[1] = GUI.Toggle(new Rect(Screen.width / 2 + 545, Screen.height - 25, 10, 40), orderFixed[1], "", GUI.skin.toggle);


	}

	/*
	 * Moves the camera so that it pointing at the right side of the board.
	 * When a players turn ends, the camera rotates to the other side of the
	 * board.
	 * 
	 */
	public IEnumerator changeCameraPosition(int player) {
		Vector3 oldPos = camera.transform.position;
		Quaternion oldRot = camera.transform.rotation;
		Vector3 newPos = new Vector3(0,0,0);
		Quaternion newRot = Quaternion.Euler(0,0,0);
		if (player == 0) {
			newPos = new Vector3((float)board.xDimension/2f, (float)board.zDimension/2f+2.5f, -3.5f);
			newRot = Quaternion.Euler(50, 0, 0);
		} else if (player == 1) {
			newPos = new Vector3((float)board.xDimension/2f, (float)board.zDimension/2f+2.5f, board.zDimension+3.5f);
			newRot = Quaternion.Euler(50, 180, 0);
		}

		int numSteps = 40;
		for (int i = 1; i <= numSteps; i++) {
			camera.transform.position = Vector3.Lerp(oldPos, newPos, ((float) i / numSteps));
			camera.transform.rotation = Quaternion.Slerp(oldRot, newRot, ((float) i / numSteps));
			yield return null;
		}
	}

	/*
	 * Returns the instruction that should be displayed depending on which 
	 * stage or phase the game is currently at
	 * 
	 */ 
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
		case 4:
			instructionText = pieceAttackWithFullSpecialText;
			break;
		}

		return instructionText;
		
		
	}


}
