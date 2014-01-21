using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour {
	public Transform ball1;
	public Transform ball2;
	
	private int polarity;
	private Transform activeBall;
	Vector3 prevPosition;
	bool zoomIn;
	// Use this for initialization
	void Start () {
		zoomIn = false;
		activeBall = ball1;
		polarity = -1;
		camera.gameObject.transform.position = activeBall.transform.position + Vector3.forward*polarity*5 + Vector3.up*2;
		camera.gameObject.transform.LookAt(activeBall);
	}
	
	// Update is called once per frame
	void Update () {
		camera.gameObject.transform.position = activeBall.transform.position + Vector3.forward*polarity*5 + Vector3.up*2;
		camera.gameObject.transform.LookAt(activeBall);
		if(zoomIn){
			camera.gameObject.transform.position += transform.forward * Time.deltaTime;
			Debug.Log(transform.position);
			if(transform.localPosition.z >= 10){
				zoomIn = false;
				this.enabled = false;
				camera.gameObject.transform.localPosition = prevPosition;
			}
		}
		if(Input.GetKeyDown(KeyCode.Space)){
			activeBall = activeBall.Equals(ball1) ? ball2 : ball1;
			polarity = -polarity;
			camera.gameObject.transform.position = activeBall.transform.position + Vector3.forward*5 + Vector3.up*2;
			camera.gameObject.transform.LookAt(activeBall);
		}
		if(Input.GetKeyDown(KeyCode.A)){
			zoomIn = true;
			prevPosition = transform.position;
		}
	}
}
