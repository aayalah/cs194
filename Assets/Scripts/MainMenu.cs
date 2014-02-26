using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {
	
	private UnitManager manager;
	private GameOptions options;
	// Use this for initialization
	void Start () {
		manager = GameObject.Find("UnitManager").GetComponent<UnitManager>();
		//options = GameObject.Find("GameOptions").GetComponent<GameOptions>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI(){
		if (GUI.Button (new Rect (Screen.width/2-75,50,150,100), "Game Options", GUI.skin.GetStyle("button"))) {
			Application.LoadLevel(3);
		}
		
		if(manager.teamsBuilt == 0){
			if(!manager.isReady()) {
				manager.setUp();
			}

			if (GUI.Button (new Rect (Screen.width/2-75,200,150,100), "P1 Team Creation", GUI.skin.GetStyle("button"))) {
				Application.LoadLevel(1);
			}
		}
		if(manager.teamsBuilt == 1){
			if (GUI.Button (new Rect (Screen.width/2-75,200,150,100), "P2 Team Creation", GUI.skin.GetStyle("button"))) {
				Application.LoadLevel(1);
			}
		}
		if(manager.teamsBuilt == 2){
			if (GUI.Button (new Rect (Screen.width/2-75,200,150,100), "Begin Game", GUI.skin.GetStyle("button"))) {
				Application.LoadLevel(2);
			}
		}
	}
}
