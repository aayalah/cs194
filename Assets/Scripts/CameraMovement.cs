using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {

	public Camera camera;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame


	public float speed = 10.0f;
	public float smooth = 2.0f;
	public float tiltAngle = 30.0f;

	void Update()
	{
		Vector3 movement = Vector3.zero;
		
		if (Input.GetKey("w"))
			movement.z++;
		if (Input.GetKey("s"))
			movement.z--;
		if (Input.GetKey("a"))
			movement.x--;
		if (Input.GetKey("d"))
			movement.x++;
		transform.Translate(movement * speed * Time.deltaTime, Space.Self);
		if (Input.GetKey ("r")) {
			float v = Input.GetAxis("Vertical");
			float tSensitivity = 1.0f;
			transform.Rotate(Vector3.right * v * tSensitivity);
		}



		
	}
	


		
}
