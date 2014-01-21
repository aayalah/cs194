using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour {
	public Camera overhead;
	public Camera ball1fp;
	public Camera ball2fp;
	
	private Camera current;
	bool zoomIn;
	bool enableFirstPerson;
	private bool prev;
	int counter;
	
	private bool p1turn;
	// Use this for initialization
	void Start () {
		enableFirstPerson = false;
		counter = 0;
		p1turn = true;
		ball1fp.enabled = false;
		ball2fp.enabled = false;
		overhead.enabled = true;
		prev = overhead.enabled;
	}
	
	// Update is called once per frame
	void Update () {
		if(ball1fp.enabled == false && ball2fp.enabled == false && overhead.enabled == false){
			activateFpCam();
		}
		if(overhead.enabled != prev){
			prev = overhead.enabled;
		}
		
		if(Input.GetKeyDown(KeyCode.LeftShift)){
			togglePOV();
		}
		if(Input.GetKeyDown(KeyCode.Space)){
			toggleTeam();
		}
		//if(Input.GetKeyDown(KeyCode.A)){
			//if(overhead.enabled == true){ 
				//enableFirstPerson = true;
			//}
		//}
	}
	
	void togglePOV(){
		if(overhead.enabled == false){ activateOverheadcam();}// = !overhead.enabled;
		else{
			if(p1turn) activateP1cam();
			if(!p1turn) activateP2cam();
		}
	}
	void toggleTeam(){
		p1turn = !p1turn;//!p1turn;
		if(overhead.enabled == false){
			activateFpCam();
		}
	}
	void activateFpCam(){
		if(p1turn) activateP1cam();
			if(!p1turn) activateP2cam();
	}
	void activateP1cam(){
		overhead.enabled = false;
		ball2fp.enabled = false;
		ball1fp.enabled = true;
		current = ball1fp;
	}
	void activateP2cam(){
		overhead.enabled = false;
		ball2fp.enabled = true;
		ball1fp.enabled = false;
		current = ball2fp;
	}
	void activateOverheadcam(){
		overhead.enabled = true;
		ball2fp.enabled = false;
		ball1fp.enabled = false;
		current = overhead;
	}
}
