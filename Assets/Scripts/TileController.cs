using UnityEngine;
using System.Collections;

public class TileController : MonoBehaviour {
	public Material on;
	public Material off;
	public bool occupied;
	
	private float previousFlash;
	public int x;//EX: 12 corresponds to coordinates (1,2)
	public int y;
	public Color baseColor = Color.white;
	
	void Start () {
		this.gameObject.renderer.material.color = baseColor;
		previousFlash = -1.0f;
	}
	
	// Update is called once per frame
	void Update () {
		if(occupied && Time.timeSinceLevelLoad - previousFlash >= 1.0f){
			toggleColor(Color.yellow);
			previousFlash = Time.timeSinceLevelLoad;
		}
	}
	
	public void setBaseColor(Color c){
		baseColor = c;
		this.gameObject.renderer.material.color = baseColor;
	}
	
	void OnCollisionEnter (Collision coll) {
		occupied = true;
	}
	void OnCollisionExit(Collision coll){
		if(occupied == true){
			this.gameObject.renderer.material.color = baseColor;
		}else{
			if(coll.collider.name == "Ball1") toggleColor(Color.blue);//this.gameObject.renderer.material.color = Color.blue;
			if(coll.collider.name == "Ball2") toggleColor(Color.green);//this.gameObject.renderer.material.color = Color.green;
		}
		occupied = false;
	}
	
	void toggleColor(Color col){
		this.gameObject.renderer.material.color = this.gameObject.renderer.material.color.Equals(baseColor) ? col : baseColor;
	}
}
