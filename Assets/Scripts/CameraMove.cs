using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour {
	public CharMove target;

	private float dist_h;
	private float dist_v;
	//private float followSpeed;
	private float turnSpeed;
	private float rotateSpeed;

	// Use this for initialization
	void Start () {
		dist_h = 5;
		dist_v = 5;
		//followSpeed = target.moveSpeed - 0.5f;
		turnSpeed = 5f;
		rotateSpeed = 200f;
		follow ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void LateUpdate(){
		//follow the target behind if movements exists
		if(target.movement.magnitude != 0){
			//change direction smoothly
			follow();
		}

		//rotate camera for input
		float mouseX = Input.GetAxis("Mouse X");
		transform.RotateAround (target.transform.position, Vector3.up, mouseX * rotateSpeed * Time.deltaTime);

		//view the target
		transform.LookAt (target.transform);
	}

	void follow(){
		float currYAngle = Mathf.LerpAngle (transform.eulerAngles.y, target.transform.eulerAngles.y, turnSpeed * Time.deltaTime);
		Quaternion targetRotate = Quaternion.Euler (0, currYAngle, 0);

		transform.position = target.transform.position - (targetRotate * Vector3.forward * dist_h) + Vector3.up * dist_v;
		//transform.position = Vector3.Lerp (transform.position, camPos, followSpeed * Time.deltaTime);
	}
}