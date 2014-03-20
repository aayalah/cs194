using UnityEngine;
using System.Collections;

public class HealthBar : MonoBehaviour {

	public Piece myPiece;
	public Transform cube;
	public int currentHP;
	public int maxHP;
	public bool showBar;

	public Texture green;
	public Texture red;

	private int counter = 0;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if(showBar){
			counter++;
			if(counter > 100){
				showBar = false;
			}
		}else{
			counter = 0;
		}
	}

	void OnGUI(){
		if(showBar){
			float percentHealthy = (float)currentHP/(float)maxHP;
			Vector2 targetPos = Camera.main.WorldToScreenPoint (myPiece.transform.position);
			GUI.DrawTexture(new Rect(targetPos.x - 25, Screen.height - targetPos.y, 50*percentHealthy, 10), green);
			GUI.DrawTexture(new Rect(targetPos.x - 25 + 50*percentHealthy, Screen.height - targetPos.y, 50-(50*percentHealthy), 10), red);
		}
	}
	public IEnumerator changeHP(int newHP){
		yield return null;
	}
}
