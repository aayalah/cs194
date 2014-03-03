using UnityEngine;
using System.Collections;

public class UnitManager : MonoBehaviour {
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
	public int [] types;
	private int[][] attacks;
	private int[][] shields;
	private int[][] specials;
	private static bool created = false;

	public int w;
	public int h;
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
		//DontDestroyOnLoad(this.gameObject);
		totalUnits = 0;
//		attacks = new int[armySize*numPlayers][];
//		shields = new int[armySize*numPlayers][];
//		specials = new int[armySize*numPlayers][];
//		types = new int[armySize*numPlayers];
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
	
	public void addUnit(int type, int[] att, int[] sh, int[] spec){
		types[totalUnits] = type;
		attacks[totalUnits] = att;
		shields[totalUnits] = sh;
		specials[totalUnits] = spec;
		totalUnits++;
	}
	
	public Piece getUnit(int id, int i){
		int p = id*armySize;
		if(types[i+p] == 1) {
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
			bumble.maxHP = 100;
			bumble.currentHP = bumble.maxHP;
			bumble.movementRange = 4;
			bumble.attackRange = 1;
			return bumble;
					
		}
		if(types[i+p] == 2){ 
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
			worker.maxHP = 50;
			worker.currentHP = worker.maxHP;
			worker.movementRange = 5;
			worker.attackRange = 3;
			return worker;
		}
		if(types[i+p] == 3) {
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
