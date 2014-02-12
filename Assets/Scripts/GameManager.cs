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

	void Awake() {
		
	}
	
	// Use this for initialization
	void Start() {
		UnitManager manager = GameObject.Find("UnitManager").GetComponent<UnitManager>();
		numberOfPlayersPieces = manager.armySize;
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
		currentNumberOfPlayersPieces = new int[numPlayers];
		for (int i = 0; i < numPlayers; i++) {
			currentNumberOfPlayersPieces[i] = numberOfPlayersPieces;
		}
		StartCoroutine (mainLoops ());
	}

	public IEnumerator mainLoops() {
		while (allPlayersHavePieces()) {
				for (int j = 0; j < 2; j++) {
						for (int i = 0; i < numPlayers; i++) {
								changeCameraPosition (i);
								yield return StartCoroutine (chooseStage (j, i));
						}
						yield return null;
				}
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
	
	
	public IEnumerator chooseStage(int stage, int p) {
		IEnumerator ret;
		//Debug.Log ("Inside ChooseStage");
		switch (stage) {
				
				case 0:
						yield return StartCoroutine(players[p].choosePieces());
						break;

				case 1:
						currentNumberOfPlayersPieces[p] = numberOfPlayersPieces;
						for (int i = 0; i < numberOfPlayersPieces; i++) {
							if(playersPieces[p,i] != null){
								if (playersPieces[p,i].dead) {
									currentNumberOfPlayersPieces[p]--;
								} else {
									yield return StartCoroutine(playersPieces [p, i].makeMove()); 
									yield return StartCoroutine(playersPieces[p, i].attack ());
								}
							}
						}
						break;
				}
		yield return null;
		
	}

	public void changeCameraPosition(int player) {
		if (player == 0) {
			Vector3 pos = new Vector3(7, 10, -5);
			camera.transform.position = pos;
			camera.transform.rotation =  Quaternion.Euler(45, 1, 0);
		} else if (player == 1) {
			Vector3 pos = new Vector3(14, 9, 25);
			camera.transform.position = pos;
			camera.transform.rotation = Quaternion.Euler(40, 174, 355);
		}


	}
	
	
}
