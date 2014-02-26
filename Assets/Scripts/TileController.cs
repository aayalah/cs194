using UnityEngine;
using System.Collections;

public class TileController : MonoBehaviour {
	public Material on;
	public Material off;
	public bool flashing;

	private float previousFlash;
	public int x;
	public int z;
	public Color baseColor = Color.white;

	void Start() {
		this.gameObject.renderer.material.color = baseColor;
		previousFlash = -1.0f;
	}

	// Update is called once per frame
	void Update () {
		/*if (flashing) {
			setColor(Color.yellow);
		} else {
			setColor(baseColor);
		}*/
	}

	public void setBaseColor(Color c) {
		baseColor = c;
	}
	public void setColor(Color c) {
		gameObject.renderer.material.color = c;
	}

	/*
	void OnCollisionEnter (Collision coll) {
		flashing = true;
	}

	void OnCollisionExit(Collision coll) {
		if (flashing) {
			this.gameObject.renderer.material.color = baseColor;
		} else {
			if (coll.collider.name == "Ball1") {
				toggleColor(Color.blue);
			}
			if (coll.collider.name == "Ball2") {
				toggleColor(Color.green);
			}
		}
		flashing = false;
	}

	public void setFlashing(bool shouldBeFlashing) {
		flashing = shouldBeFlashing;
	}

	public void startFlashing() {
		setFlashing(true);
	}

	public void stopFlashing() {
		setFlashing(false);
	}	
	*/
}