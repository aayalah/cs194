using UnityEngine;
using System.Collections;
using System;
public class ClockKing : MonoBehaviour {

	private int maxTurns = 2;
	private int numTurns = 0;
	private bool isPaused = true;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {

		guiText.text = numTurns.ToString();

	}


	public void addTurn() {
		numTurns++;
	}

	public void Initialize(int id) {

		if (id == 0) {
			guiText.pixelOffset= new Vector2(450, 580);
			guiText.color = Color.red;
		} else {
			guiText.pixelOffset = new Vector2(870, 580);
			guiText.color = Color.blue;
		}



	}

	public bool reachedGoal(){


		return numTurns >= maxTurns;

	}
	

}
