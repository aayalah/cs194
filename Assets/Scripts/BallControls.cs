using UnityEngine;
using System.Collections;

public class BallControls : MonoBehaviour {
	// Use this for initialization
	bool onGround;
	bool isInit;
	
	public int hp;
	public int polarity;
	public bool isActive;
	private Transform currentTile;
	private Vector3 correctedPosition;
	public int x;
	public int z;
	private float timeSinceJump;
	private Vector3 newOrientation;
	private float turnSpeed = 1f;
	float rotationAmount = 90.0f;
	bool rotate = false;
	float amountRotated = 0f;
	
	bool targetSelect = false;
	string SELECT_A_TARGET = "Select a target.";
	
	void Start () {
		hp = 10;
		//isActive = false;
		//newYRotation = new Transform();
		Time.timeScale = 2.5f;
		isInit = true;
		transform.forward = Vector3.forward * polarity;
	}
	
	// Update is called once per frame
	void Update () {
		if(rotate){
			float f = rotationAmount*Time.deltaTime;
			if(f > 0 && amountRotated < rotationAmount){
				amountRotated += f;
				transform.Rotate(0, f*turnSpeed, 0);
			}
			if(f > 0 && amountRotated > rotationAmount){
				amountRotated = rotationAmount;
				transform.eulerAngles = newOrientation;
			}
			if(f < 0 && amountRotated > rotationAmount){
				amountRotated += f;
				transform.Rotate(0, f*turnSpeed, 0);
			}
			if(f < 0 && amountRotated < rotationAmount){
				transform.eulerAngles = newOrientation;
				amountRotated = rotationAmount;
			}
			if(amountRotated == rotationAmount){
				rotate = false;
			}
		}
		/*Input Listeners*/
		if(Input.GetKeyDown(KeyCode.Space)){
			isActive = !isActive;
		}
		if(Input.GetKeyUp(KeyCode.UpArrow) && isActive){
			if(onGround && !rotate){
				timeSinceJump = Time.timeSinceLevelLoad;
				correctedPosition += transform.forward;
				rigidbody.AddForce(Vector3.up*5+transform.forward, ForceMode.VelocityChange);
			}
		}
		if(Input.GetKeyUp(KeyCode.LeftArrow) && isActive){
			if(onGround && !rotate){
				timeSinceJump = Time.timeSinceLevelLoad;
				rotationAmount = -90.0f;
				amountRotated = 0f;
				transform.Rotate(0, rotationAmount, 0);
				newOrientation = transform.localEulerAngles;
				transform.Rotate(0, -rotationAmount, 0);
				rotate = true;
				}
		}
		if(Input.GetKeyUp(KeyCode.RightArrow) && isActive){
			if(onGround && !rotate){
				timeSinceJump = Time.timeSinceLevelLoad;
				rotationAmount = 90.0f;
				amountRotated = 0f;
				transform.Rotate(0, rotationAmount, 0);
				newOrientation = transform.localEulerAngles;
				transform.Rotate(0, -rotationAmount, 0);
				rotate = true;
				}
		}
		if(Input.GetKeyUp(KeyCode.A) && isActive){
			if(onGround && !rotate){
				targetSelect = true;
			}
		}
		if(Input.GetMouseButtonDown(0)){
			if(targetSelect){
				Ray ray = this.GetComponentInChildren< Camera >().ScreenPointToRay(Input.mousePosition);
        		RaycastHit hit;
        		if(Physics.Raycast(ray, out hit, Mathf.Infinity)){
					if(hit.collider.name == "Ball2"){
						GameObject target = hit.transform.gameObject;
						target.GetComponent<BallControls>().applyDamage(4);
						targetSelect = false;
					}
				}
			}
		}
	}
	
	void OnCollisionEnter(Collision coll){
		if(isInit){
			isInit = false;
			correctedPosition = transform.position;
		}
		correctedPosition.y = transform.position.y;
		transform.position = correctedPosition;
		onGround = true;
		currentTile = coll.transform;
		if (coll.transform.GetComponent<TileController> ()) {
			x = coll.transform.GetComponent<TileController> ().x;
			z = coll.transform.GetComponent<TileController> ().z;
		}
	}
	
	
	void OnCollisionExit(){
		onGround = false;
	}
	
	void OnGUI(){
		if(targetSelect){
			GUI.Label(new Rect(Screen.width/2-40, Screen.height/2-125, 300, 200), SELECT_A_TARGET);
		}
	}
	void applyDamage(int dam){
		hp -= dam;
		if(hp <= 0){
			Destroy(this.gameObject);
		}
	}
}
