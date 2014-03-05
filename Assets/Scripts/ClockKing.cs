using UnityEngine;
using System.Collections;
using System;
public class ClockKing : MonoBehaviour {

	private float maxTime = 120;
	private float startTime = 0;
	private float passedTime = 0;
	private bool isPaused = true;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {

		if (!isPaused) {
						float timePassed = Time.time - startTime + passedTime;
			
						int minutes = Convert.ToInt32 (timePassed / 60);
			
						int seconds = Convert.ToInt32 (timePassed % 60);
			
						guiText.text = minutes.ToString () + ":" + seconds.ToString ("D2");
		} else {
			guiText.text = "0:00";

		}

	}

	public void Initialize(int id) {

		if (id == 0) {
			guiText.pixelOffset= new Vector2(450, 580);
			guiText.color = Color.blue;
		} else {
			guiText.pixelOffset = new Vector2(870, 580);
			guiText.color = Color.green;
		}



	}

	public float getTimePassed(){
		passedTime += Time.time - startTime;
		return passedTime;
	}

	public void startTimer(){
		
		startTime = Time.time;

	}

	public void pause(){

		passedTime += Time.time - startTime;
		isPaused = true;
	}

	public void unpause() {
		startTime = Time.time;
		isPaused = false;

	}


	public bool reachedGoal(){

		float totalTime =  Time.time - startTime + passedTime;
		return totalTime >= maxTime;

	}
	

}
