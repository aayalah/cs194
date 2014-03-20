using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {
  public Piece creator;
  public int damage = 10;
  public int OUT_OF_BOUNDS = 100;
  public Vector3 velocity = new Vector3(1, 0, 0);
  public int FRAMES_PER_SECOND = 30;

	// Use this for initialization
	void Start () {
	 gameObject.renderer.material.color = Color.red;
	}
	
	// Update is called once per frame
	void Update () {
    transform.position += velocity / FRAMES_PER_SECOND;
	  if (Mathf.Abs(transform.position.x) > OUT_OF_BOUNDS || 
        Mathf.Abs(transform.position.z) > OUT_OF_BOUNDS) {
      Destroy(gameObject);
    }
	}

  // Disappear on contact with another object; if it's a piece, 
  // damage it.
  void OnTriggerEnter(Collider coll) {
    Debug.Log("Collided with " + coll.gameObject.name);
    Piece otherPiece = coll.gameObject.GetComponent<Piece>();
    if (otherPiece && otherPiece != creator) {
      otherPiece.takeDamage(damage);
    }
    if (!otherPiece || otherPiece != creator) {
      Destroy(gameObject);
    }
  }
}
