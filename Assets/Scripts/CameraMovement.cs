using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {

	public Camera camera;
	private int turn = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame


	public float speed = 10.0f;
	public float smooth = 2.0f;
	public float tiltAngle = 30.0f;
	public float fDistance = 1f;
	public float fSpeed = 1f;


	void Update()
	{
		Vector3 movement = Vector3.zero;

		float fOrbitCircumfrance = 2F*fDistance*Mathf.PI;
		float fDistanceDegrees = (fSpeed / fOrbitCircumfrance) * 360;
		float fDistanceRadians = (fSpeed / fOrbitCircumfrance) * 2*Mathf.PI;

		if (Input.GetKey ("w"))
			movement.z++;
		if (Input.GetKey("s"))
			movement.z--;
		if (Input.GetKey("a"))
			movement.x--;
		if (Input.GetKey("d"))
			movement.x++;
		transform.Translate(movement * speed * Time.deltaTime, Space.Self);
		if ( Input.GetKey(KeyCode.LeftArrow) )
			transform.RotateAround(transform.position, Vector3.up, -fDistanceRadians);
		if ( Input.GetKey(KeyCode.RightArrow) )
			transform.RotateAround(transform.position, Vector3.up, fDistanceRadians);


		
	}



		
}
