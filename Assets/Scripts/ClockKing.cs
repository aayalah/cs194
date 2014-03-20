using UnityEngine;
using System.Collections;
using System;
public class ClockKing : MonoBehaviour {

	/*
	 * Variables: Private 
	 */
	private int maxTurns = 2;
	private int numTurns = 0;
	private bool isPaused = true;

	// Use this for initialization
	void Start () {
		UnitManager manager = GameObject.Find("UnitManager").GetComponent<UnitManager>();
		maxTurns = manager.numTurns;
	}

	
	// Update is called once per frame
	void Update () {

		guiText.text = numTurns.ToString();

	}

	//Increments the variable that keeps track of the number of times the player has had a piece on the
	//square
	public void addTurn() {
		numTurns++;
	}


	/*
	 * Creates the two counters that are displayed on the screen, that keep track
	 * of the number of turns a player has had a piece on the square
	 */ 
	public void Initialize(int id) {

		if (id == 0) {
			guiText.pixelOffset= new Vector2(450, 580);
			guiText.color = Color.red;
		} else {
			guiText.pixelOffset = new Vector2(870, 580);
			guiText.color = Color.blue;
		}



	}

	/*
	 * Checks whether the player has had a piece on the square for the required number of turns
	 * 
	 */ 
	public bool reachedGoal(){


		return numTurns >= maxTurns;

	}
	

}
