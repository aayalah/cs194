using UnityEngine;
using System.Collections;

public class JukeBox : MonoBehaviour {
	public Texture speakerOn;
	public Texture next;
	public Texture speakerOff;
	public GUIStyle style;
	public AudioClip [] jukebox = new AudioClip[5];
	private int currentSong = 0;
	private bool muted = false;
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
		audio.clip = jukebox[0];
		audio.Play();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//Displays mute/unmute button and song skip button
	void OnGUI(){
		if(!muted){
			if(GUI.Button(new Rect(Screen.width-50,10, 50, 50), speakerOn, style)){
				audio.Stop();
				muted = true;
			}
		}else{
			if(GUI.Button(new Rect(Screen.width-50,10, 50, 50), speakerOff, style)){
				audio.Play();
				muted = false;
			} 
		}
		if(GUI.Button(new Rect(Screen.width-100,10, 50, 50), next, style)){
			audio.Stop();
			currentSong = (currentSong == 4) ? 0 : currentSong+1;
			audio.clip = jukebox[currentSong];
			audio.Play();
		} 
	}
}
