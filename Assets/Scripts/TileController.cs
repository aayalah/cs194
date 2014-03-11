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

	public bool isKingTile = false;

	void Start() {
		off = this.gameObject.renderer.material;
		this.gameObject.renderer.material.color = baseColor;
		previousFlash = -1.0f;
	}

	// Update is called once per frame
	void Update () {
		if (flashing) {
			this.renderer.material = on;
		} else {
			this.renderer.material = off;
		}
	}

	public void setBaseColor(Color c) {
		baseColor = c;
	}
	public void setColor(Color c) {
		gameObject.renderer.material.color = c;
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
}