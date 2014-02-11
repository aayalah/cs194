using UnityEngine;
using System.Collections;

public class UnitManager : MonoBehaviour {
	public Piece brute;
	public Piece grunt;
	public Piece ranger;
	
	public int teamsBuilt = 0;
	public int armySize = 5;
	public int numPlayers = 2;
	
	public int totalUnits;
	public int [] types;
	private int[][] attacks;
	private int[][] shields;
	private int[][] specials;
	private static bool created = false;
	
	
	
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
		
		attacks = new int[armySize*numPlayers][];
		shields = new int[armySize*numPlayers][];
		specials = new int[armySize*numPlayers][];
		types = new int[armySize*numPlayers];
	}
	
	// Update is called once per frame
	void Update () {
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
			Piece bru = (Piece)Instantiate(brute, new Vector3((19f-(float)armySize+1)*id+(float)i,1f,19f*id), Quaternion.identity);
			bru.renderer.enabled = true;
			bru.attackHistogram = attacks[i+p];
			bru.defenseHistogram = shields[i+p];
			bru.specialHistogram = specials[i+p];
			bru.maxHP = 100;
			bru.currentHP = bru.maxHP;
			bru.movementRange = 4;
			bru.attackRange = 1;
			return bru;
					
		}
		if(types[i+p] == 2){ 
			Piece gru = (Piece)Instantiate(grunt, new Vector3((19f-(float)armySize+1)*id+(float)i,1f,19f*id), Quaternion.identity);
			gru.renderer.enabled = true;
			gru.attackHistogram = attacks[i+p];
			gru.defenseHistogram = shields[i+p];
			gru.specialHistogram = specials[i+p];
			gru.maxHP = 50;
			gru.currentHP = gru.maxHP;
			gru.movementRange = 5;
			gru.attackRange = 3;
			return gru;
		}
		if(types[i+p] == 3) {
			Piece ran = (Piece)Instantiate(ranger, new Vector3((19f-(float)armySize+1)*id+(float)i,1f,19f*id), Quaternion.identity);
			ran.renderer.enabled = true;
			ran.attackHistogram = attacks[i+p];
			ran.defenseHistogram = shields[i+p];
			ran.specialHistogram = specials[i+p];
			ran.maxHP = 100;
			ran.currentHP = ran.maxHP;
			ran.movementRange = 6;
			ran.attackRange = 8;
			return ran;
		}
		return null;
	}
}
