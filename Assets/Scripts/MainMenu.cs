using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {
	
	private UnitManager manager;
	private GameOptions options;
	public GUISkin skin;
	// Use this for initialization
	void Start () {
		manager = GameObject.Find("UnitManager").GetComponent<UnitManager>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI(){
		GUI.skin = skin;
		GUI.Label(new Rect(Screen.width/2-400, 50, 800, 100), "Bees With Jetpacks!", GUI.skin.GetStyle("label"));
		GUI.contentColor = Color.black;
		GUI.Label(new Rect(Screen.width/2-410, 50, 800, 100), "Bees With Jetpacks!", GUI.skin.GetStyle("label"));
		if(manager.teamsBuilt == 0){
			if(!manager.isReady()) {
				manager.setUp();
			}

			if (GUI.Button (new Rect (Screen.width/2-150,150,300,75), "P1 Team Creation", GUI.skin.GetStyle("button"))) {
				Application.LoadLevel(1);
			}
			if (GUI.Button (new Rect (Screen.width/2-100,225,200,50), "Game Options", GUI.skin.GetStyle("button"))) {
				Application.LoadLevel(3);
			}

		}else{
			GUI.enabled = false;
			GUI.Button (new Rect (Screen.width/2-100,225,200,50), "Game Options", GUI.skin.GetStyle("button"));
			GUI.enabled = true;
		}
		if(manager.teamsBuilt == 1){
			if (GUI.Button (new Rect (Screen.width/2-150,150,300,75), "P2 Team Creation", GUI.skin.GetStyle("button"))) {
				Application.LoadLevel(1);
			}
		}
		if(manager.teamsBuilt == 2){
			if (GUI.Button (new Rect (Screen.width/2-150,150,300,75), "Begin Game", GUI.skin.GetStyle("button"))) {
				Application.LoadLevel(2);
			}
		}
		if (GUI.Button (new Rect (Screen.width/2-100,275,200,50), "Quit", GUI.skin.GetStyle("button"))) {
			Application.Quit();
		}
	}
}
