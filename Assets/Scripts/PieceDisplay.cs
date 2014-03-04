//using UnityEngine;
//using System.Collections;
//
//
//
//public class PieceDisplay : MonoBehaviour {
//
//	private Rect windowRect = new Rect(Screen.width/2 - 450, 300, 900, 120);
//	bool display = false;
//
//
//	// Use this for initialization
//	void Start () {
//	
//	}
//	
//
//	// Update is called once per frame
//	void Update () {
//	
//	}
//
//
//	void OnGUI() {
//
//    	
//		if (display) {
//
//
//		}
//
//
//		if (GUI.Button (new Rect (50, 300, 55, 30), "+")) {
//			display = true;
//		}
//
//
//
//	}
//
//
//
//	void helpWindow(int windowID) {
//		
//		
//		GUI.contentColor = Color.red;
//		GUI.skin.label.alignment = TextAnchor.MiddleCenter;
//		GUI.backgroundColor = Color.black;
//		GUI.Label (new Rect (10, 30, 600, 45), getInstructionText(stage), GUI.skin.label);
//		GUI.skin.button.alignment = TextAnchor.MiddleCenter;
//		if (GUI.Button (new Rect (270, 80, 55, 30), "-")) {
//			display = false;
//		}
//		
//	}
//
//
//
