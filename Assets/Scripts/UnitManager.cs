using UnityEngine;
using System.Collections;

public class UnitManager : MonoBehaviour {

	/*
	 * Variables: Public 
	 */

	public Piece redBumblebee;
	public Piece redWorker;
	public Piece redHornet;
	public Piece blueBumblebee;
	public Piece blueWorker;
	public Piece blueHornet;

	public int teamsBuilt = 0;
	public int armySize;
	public int numPlayers = 2;
	public int totalUnits;
	public int turnsPerRound = 3;
	public int numTurns;
	public int w;
	public int h;
	public bool kingMode;

	public int [] types;

	public bool p1Human = true;
	public bool p2Human = false;

	/*
	 * Variables: Private 
	 */

	private static bool created = false;

	//Data structures to keep tracks of the distribution of abilities each piece has
	private int[][] attacks;
	private int[][] shields;
	private int[][] specials;
	
	private bool hasSetUp = false;
	

	void Awake() {
    	if (!created) {
        	// this is the first instance - make it persist
        	DontDestroyOnLoad(this.gameObject);
        	created = true;
    } else {
        // this must be a duplicate from a scene reload - DESTROY!
        Destroy(this.gameObject);
    	} 		

	}

	// Use this for initialization
	void Start () {

		totalUnits = 0;
	}


	// Update is called once per frame
	void Update () {
	}

	public void setUp() {
		attacks = new int[armySize*numPlayers][];
		shields = new int[armySize*numPlayers][];
		specials = new int[armySize*numPlayers][];
		types = new int[armySize*numPlayers];	
		hasSetUp = true;
	}

	public bool isReady() {
		return hasSetUp;
	}


	/*Called by teamBuilder when a unit is created. Stores relevant data for that unit*/
	public void addUnit(int type, int[] att, int[] sh, int[] spec){
		types[totalUnits] = type;
		attacks[totalUnits] = att;
		shields[totalUnits] = sh;
		specials[totalUnits] = spec;
		totalUnits++;
	}

	/*Called by Player. Instantiates a prefab of the correct unit type and initializes stats based on TeamBuilder phase. Returns the instantiated unit*/
	public Piece getUnit(int id, int i){
		int p = id*armySize;
		if(types[i+p] == 1) {//unit is a bumblebee
			Piece bumble;
			if(id == 0){
				bumble = (Piece)Instantiate(redBumblebee, new Vector3((19f-(float)armySize+1)*id+(float)i,1f,19f*id), Quaternion.identity);
				bumble.transform.Rotate(new Vector3(0, 180, 0));
				bumble.teamNo = 0;
			}else{
				bumble = (Piece)Instantiate(blueBumblebee, new Vector3((19f-(float)armySize+1)*id+(float)i,1f,19f*id), Quaternion.identity);
				bumble.teamNo = 1;
			}
			bumble.transform.localScale = new Vector3(1f, 1f, 1f);
			bumble.gameObject.GetComponentInChildren<Renderer>().enabled = true;
			bumble.attackHistogram = attacks[i+p];
			bumble.defenseHistogram = shields[i+p];
			bumble.specialHistogram = specials[i+p];
			bumble.maxHP = 60;
			bumble.currentHP = bumble.maxHP;
			bumble.movementRange = 4;
			bumble.attackRange = 1;
			return bumble;
					
		}
		if(types[i+p] == 2){ //unit is a worker
			Piece worker;
			if(id == 0){
				worker = (Piece)Instantiate(redWorker, new Vector3((19f-(float)armySize+1)*id+(float)i,1f,19f*id), Quaternion.identity);
				worker.transform.Rotate(new Vector3(0, 180, 0));
				worker.teamNo = 0;
			}else{
				worker = (Piece)Instantiate(blueWorker, new Vector3((19f-(float)armySize+1)*id+(float)i,1f,19f*id), Quaternion.identity);
				worker.teamNo = 1;
			}
			worker.transform.localScale = new Vector3(.9f, .9f, .9f);
			worker.gameObject.GetComponentInChildren<Renderer>().enabled = true;
			worker.attackHistogram = attacks[i+p];
			worker.defenseHistogram = shields[i+p];
			worker.specialHistogram = specials[i+p];
			worker.maxHP = 40;
			worker.currentHP = worker.maxHP;
			worker.movementRange = 5;
			worker.attackRange = 4;
			return worker;
		}
		if(types[i+p] == 3) {//unit is a hornet
			Piece hornet;
			if(id == 0){
				hornet = (Piece)Instantiate(redHornet, new Vector3((19f-(float)armySize+1)*id+(float)i,1f,19f*id), Quaternion.identity);
				hornet.transform.Rotate(new Vector3(0, 180, 0));
				hornet.teamNo = 0;
			}else{
				hornet = (Piece)Instantiate(blueHornet, new Vector3((19f-(float)armySize+1)*id+(float)i,1f,19f*id), Quaternion.identity);
				hornet.teamNo = 1;
			}
			hornet.transform.localScale = new Vector3(.8f, .8f, .8f);
			hornet.gameObject.GetComponentInChildren<Renderer>().enabled = true;
			hornet.attackHistogram = attacks[i+p];
			hornet.defenseHistogram = shields[i+p];
			hornet.specialHistogram = specials[i+p];
			hornet.maxHP = 20;
			hornet.currentHP = hornet.maxHP;
			hornet.movementRange = 6;
			hornet.attackRange = 8;
			return hornet;
		}
		return null;
	}
}
