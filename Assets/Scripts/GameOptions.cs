using UnityEngine;
using System.Collections;
using System;
public class GameOptions : MonoBehaviour {

	/*
	 * Variables: Public 
	 */

	public GUISkin skin;

	/*
	 * Variables: Private 
	 */

	private int defaultW = 20;
	private int defaultH = 20;
	private int defaultArmySize = 5;
	private int player1Status = 0;
	private int player2Status = 1;
	private int w;
	private int h;
	private int np;
	private int nt;

	/*
	 * Variables: Private
	 */

	//Allow communication between classes
	private UnitManager man;


	private bool display = false;
	private string numTurns = "5";
	private static bool created = false;
	private string width = "20";
	private string height = "20";
	private string numberOfPieces = "5";
	private bool showMessage = false;
	private bool gameMode = false;

	void Awake() {
		
		DontDestroyOnLoad(this.gameObject);

	}


	// Use this for initialization
	void Start () {
		display = true;
		man = GameObject.Find ("UnitManager").GetComponent<UnitManager> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	/*
	 * Creates all the user interface elements and handles the user's input, placing the data in the appropriate data structures
	 *  so they can be accessed in othe parts of the code
	 * 
	 * 
	 * 
	 */ 


	void OnGUI() {
		if (display) {
						GUI.skin = skin;
						GUI.skin.label.alignment = TextAnchor.MiddleCenter;
						GUI.skin.label.fontSize = 60;
						GUI.Label (new Rect (Screen.width / 2 - 150, 30, 400, 100), "Game Options", GUI.skin.label);
						GUI.skin.label.alignment = TextAnchor.MiddleLeft;
						GUI.skin.label.fontSize = 30;
						GUI.Label (new Rect (Screen.width / 2 - 300, 120, 300, 40), "Board Configuration", GUI.skin.label);
						GUI.skin.label.fontSize = 20;
						GUI.Label (new Rect (Screen.width / 2 - 250, 150, 300, 40), "Width: ", GUI.skin.label);
						width = GUI.TextField (new Rect (Screen.width / 2, 160, 50, 20), width);
						GUI.Label (new Rect (Screen.width / 2 - 250, 180, 300, 40), "Height: ", GUI.skin.label);
						height = GUI.TextField (new Rect (Screen.width / 2, 190, 50, 20), height);
						GUI.skin.label.fontSize = 30;
						GUI.Label (new Rect (Screen.width / 2 - 300, 210, 300, 40), "Piece Configuration", GUI.skin.label);
						GUI.skin.label.fontSize = 20;
						GUI.Label (new Rect (Screen.width / 2 - 250, 240, 300, 40), "Number Of Pieces: ", GUI.skin.label);
						numberOfPieces = GUI.TextField (new Rect (Screen.width / 2, 250, 50, 20), numberOfPieces);
						GUI.skin.toggle.alignment = TextAnchor.UpperCenter;
						
			GUI.skin.label.fontSize = 30;
			GUI.Label (new Rect (Screen.width / 2 - 300, 270, 300, 40), "Player Configuration", GUI.skin.label);
			GUI.skin.label.fontSize = 20;
			GUI.Label (new Rect (Screen.width / 2 - 250, 300, 300, 40), "Player 1: ", GUI.skin.label);
			string[] toolbarStrings = new string[] {"Human", "CPU"};
			player1Status = GUI.Toolbar(new Rect(Screen.width / 2 - 75, 310, 200, 20), player1Status, toolbarStrings);
			GUI.Label (new Rect (Screen.width / 2 - 250, 330, 300, 40), "Player 2: ", GUI.skin.label);
			player2Status = GUI.Toolbar(new Rect(Screen.width / 2 - 75, 340, 200, 20), player2Status, toolbarStrings);
			GUI.skin.label.fontSize = 30;
			GUI.Label (new Rect (Screen.width / 2 - 300, 360, 300, 40), "Game Mode", GUI.skin.label);
			GUI.skin.label.fontSize = 20;
			GUI.Label (new Rect (Screen.width / 2 - 250, 390, 300, 40), "King of the Hill: ", GUI.skin.label);
			gameMode = GUI.Toggle(new Rect(Screen.width / 2 + 20, 400, 10, 40), gameMode, "", GUI.skin.toggle);
			if(gameMode) {
				GUI.Label (new Rect (Screen.width / 2 - 250, 420, 300, 40), "Number Of Turns: ", GUI.skin.label);
				numTurns = GUI.TextField (new Rect (Screen.width / 2, 430, 50, 20), numTurns);
			}


						if (GUI.Button (new Rect (Screen.width / 2, 480, 50, 20), "Enter")) {
								Int32.TryParse (width, out w);
								Int32.TryParse (height, out h);
								Int32.TryParse (numberOfPieces, out np);
								Int32.TryParse (numTurns, out nt);
								if (w <= 0 || h <= 0 || np <= 0 || nt <= 0) {
										Debug.Log (w);
										showMessage = true;

								} else {
										display = false;
										man.armySize = np;
										man.w = w;
										man.h = h;
										man.numTurns = nt;
										man.setUp();
										man.kingMode = gameMode;
										Application.LoadLevel (0);
								}
				man.p1Human = (player1Status == 0);
				man.p2Human = (player2Status == 0);

						}


						if (showMessage) {
								GUI.contentColor = Color.yellow;
				GUI.Label (new Rect (Screen.width / 2 - 250, 510, 800, 40), "One of the values you entered is not a valid value. Enter a number above 0.");
			
						}
						
						

				}

	}
}
