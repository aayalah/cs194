using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	public Piece piece;
	public Piece[] pieceArray;
	public int numberOfPieces = 3;
	
	public GameManager game;
	private int id;
	public Camera camera;
	public int numPieces = 0;
	
	// Use this for initialization
	void Start () {
		UnitManager um = GameObject.Find("UnitManager").GetComponent<UnitManager>();
		pieceArray = new Piece[numberOfPieces];
		
		if (id == 0) {

			//Debug.Log (id);
			for (int i = 0; i < numberOfPieces; i++) {
				int x = 5 * i;
				int y = 1;
				int z = 0;
				Vector3 v = new Vector3 (x, y, z);
				pieceArray [i] = um.getUnit(id, i);
				//pieceArray [i] = (Piece)Instantiate (piece, v, Quaternion.identity);
				pieceArray [i].Initialize (this, game);
				pieceArray [i].id = "player" + id;
<<<<<<< HEAD
				pieceArray[i].x = x;
				pieceArray[i].z = z;
				pieceArray[i].player = this;
=======
				pieceArray[i].x = (int)pieceArray[i].gameObject.transform.position.x;
				pieceArray[i].z = (int)pieceArray[i].gameObject.transform.position.z;
>>>>>>> 51bd40b0ff76ad6f72d69f067a06a051a456c32d
			}
		} else {
			//Debug.Log(id);
			for(int i = 0; i < numberOfPieces; i++) {
				int x = 5*i;
				int y = 1;
				int z = 18;
				Vector3 v = new Vector3 (x, y, z);
				pieceArray[i] = um.getUnit(id, i);
				pieceArray[i].Initialize(this, game);
				pieceArray [i].id = "player" + id;
<<<<<<< HEAD
				pieceArray[i].x = x;
				pieceArray[i].z = z;
				pieceArray[i].player = this;
=======
				pieceArray[i].x = (int)pieceArray[i].gameObject.transform.position.x;
				pieceArray[i].z = (int)pieceArray[i].gameObject.transform.position.z;
>>>>>>> 51bd40b0ff76ad6f72d69f067a06a051a456c32d
			}
			
			
		}
		
		
	}
	
	public void Initialize(int id, Camera camera, GameManager game){
		this.id = id;
		this.camera = camera;
		this.game = game;
		
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
		//Debug.Log ("Inside ChoosePieces");
		Piece chosenPiece = null;
		while (numPieces != numberOfPieces) {
			Transform selected;
			if (Input.GetMouseButtonDown (0)) {
				Ray ray = camera.ScreenPointToRay (Input.mousePosition);
				RaycastHit hit;
				if (Physics.Raycast (ray, out hit, Mathf.Infinity)) { 
					selected = hit.transform;
					chosenPiece = (Piece)selected.gameObject.GetComponent (typeof(Piece));
					if (chosenPiece) {
						string temp = chosenPiece.id;
						string test = "player" + getId();
						if (temp.Equals(test) && (!containsPiece(game.playersPieces, getId(),chosenPiece))) {
							game.playersPieces [getId(), numPieces] = chosenPiece;
							chosenPiece.renderer.material.SetColor ("_Color", Color.red);
							numPieces++;
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
		
		//if(stats.removeMoney(findPieceCost(pieceId))){
		//addPiece(pieceId);		
		//} else {
		
		//}
		
		
		
	}
	
	
	// Update is called once per frame
	void Update () {
		
		
		
	}
}
