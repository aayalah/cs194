using UnityEngine;
using System.Collections;
using System;

public class Clock : MonoBehaviour {

	private static int totalTime = 300;
	private float startTime;
	private float timeRemaining;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		float timeRemaining = totalTime - (Time.time - startTime);

		int minutes = Convert.ToInt32(timeRemaining / 60);

		int seconds = Convert.ToInt32(timeRemaining % 60);

		guiText.text = minutes.ToString() + ":" + seconds.ToString("D2");

	}

	public bool outOfTime() {

		return totalTime - (Time.time - startTime) <= 0;

	}

	public void startTimeCountdown() {

		startTime = Time.time;

	}


}
