using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	public Piece piece;
	public Piece[] pieceArray;
	private int numberOfPieces = 5;
	private int turnsPerRound = 3;
	
	public GameManager game;
	private int id;
	public Camera camera;
	public int numPieces = 0;
	public UnitManager um;

	// Use this for initialization
	void Start () {
	
	}


	public IEnumerator setUpPieces() {

		numberOfPieces = um.armySize;
		pieceArray = new Piece[numberOfPieces];

		for(int i = 0; i < numberOfPieces; i++) {
			pieceArray[i] = um.getUnit(id, i);
			pieceArray[i].Initialize(this, game);
			pieceArray[i].id = "player" + id;
			pieceArray[i].player = this;
			pieceArray[i].tag = "piece";
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
	}
	
	
	public class Info {
		
		private int money;
		
		public void addMoney(int mon) {
			money = mon;
		}
		
	}
	
	
	
	//Keeps track of pieces
	
	public int initialMoney = 100;
	public Info stats = new Info();
	
	// Keeps track of stats
	
	
	
	public Player(){
		stats.addMoney(initialMoney);
		//this.game = game;
	}
	
	public int getId() {
		return id;
	}
	
	public IEnumerator choosePieces() {
		numPieces = 0;
		Piece chosenPiece = null;
		while (numPieces != turnsPerRound) {
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
						if (temp.Equals(test) /*&& (!containsPiece(game.playersPieces, getId(),chosenPiece))*/) {
							game.playersPieces [getId(), numPieces] = chosenPiece;
							chosenPiece.numMarkers++;
							numPieces++;
							Debug.Log(numPieces + "/" + turnsPerRound);
						}
					}
				}
				
			} 
			yield return null;					
		}
	}
	
	private bool containsPiece(Piece[,] playersPieces, int p, Piece piece) {
		
		for(int i = 0; i < numberOfPieces; i++) {
			//Debug.Log("player" + p + "piece" + i);
			if((playersPieces[p,i] != null) && (playersPieces[p,i].Equals(piece))) {

				return true;
			}
			
		}
		return false;
	}
	
	
	public void addMoney(int money) {
		stats.addMoney(money);
		
	}
	
	public void buyPiece(int pieceId) {
		
	}

	public void reset() {
//		for (int i = 0; i < numPieces; i++) {
//			game.playersPieces[id, i] = null;
//		}
		numPieces = 0;


		}
	
	
	// Update is called once per frame
	void Update () {
		
		
		
	}
}
