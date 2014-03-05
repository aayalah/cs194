﻿using UnityEngine;
using System.Collections;
using System;
public class GameOptions : MonoBehaviour {

	private static bool created = false;
	private string width = "20";
	private string height = "20";
	private string numberOfPieces = "5";
	private bool showMessage = false;

	public GUISkin skin;
	public int defaultW = 20;
	public int defaultH = 20;
	public int defaultArmySize = 5;

	public int w;
	public int h;
	public int np;

	private bool display = false;
	private UnitManager man;

	void Awake() {
		
		DontDestroyOnLoad(this.gameObject);

	}


	// Use this for initialization
	void Start () {
		display = true;
		man = GameObject.Find ("UnitManager").GetComponent<UnitManager> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	void OnGUI() {
		if (display) {
			GUI.skin = skin;
						GUI.skin.label.alignment = TextAnchor.MiddleCenter;
						GUI.skin.label.fontSize = 60;
						GUI.Label (new Rect (Screen.width / 2 - 150, 30, 400, 100), "Game Options", GUI.skin.label);
						GUI.skin.label.alignment = TextAnchor.MiddleLeft;
						GUI.skin.label.fontSize = 30;
						GUI.Label (new Rect (Screen.width / 2 - 300, 120, 300, 40), "Board Configuration", GUI.skin.label);
						GUI.skin.label.fontSize = 20;
						GUI.Label (new Rect (Screen.width / 2 - 250, 150, 300, 40), "Width: ", GUI.skin.label);
						width = GUI.TextField (new Rect (Screen.width / 2, 160, 50, 20), width);
						GUI.Label (new Rect (Screen.width / 2 - 250, 180, 300, 40), "Height: ", GUI.skin.label);
						height = GUI.TextField (new Rect (Screen.width / 2, 190, 50, 20), height);
						GUI.Label (new Rect (Screen.width / 2 - 250, 210, 300, 40), "Number Of Pieces: ", GUI.skin.label);
						numberOfPieces = GUI.TextField (new Rect (Screen.width / 2, 220, 50, 20), numberOfPieces);
						if (GUI.Button (new Rect (Screen.width / 2, 250, 50, 20), "Enter")) {
								Int32.TryParse (width, out w);
								Int32.TryParse (height, out h);
								Int32.TryParse (numberOfPieces, out np);
								display = false;
								man.armySize = (np > 0) ? np: defaultArmySize;
								man.w =  (w > 0) ? w : defaultW;
								man.h = (h > 0) ? h : defaultH;
								man.setUp();
								Application.LoadLevel (0);

						}


						if (showMessage) {
								GUI.contentColor = Color.yellow;
								GUI.Label (new Rect (Screen.width / 2 - 250, 280, 300, 40), "Enter an integer above 0.");
			
						}
				}

	}
}
