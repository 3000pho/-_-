using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constant;

public class CameraMove : MonoBehaviour {
	public CharMove target;
	public Quaternion camDir;

	private float dist_h;
	private float dist_v;
	//private float followSpeed;
	private float turnSpeed;
	private float rotateSpeed;
	private float minZoom;
	private float maxZoom;
	private Vector3 offset;
	private Vector2 horizon;

	// Use this for initialization
	void Start () {
		dist_h = 5f;
		dist_v = 5f;
		turnSpeed = 5f;
		rotateSpeed = 200f;

		Follow ();
		offset = transform.position - target.transform.position;
		camDir = Quaternion.Euler (0, transform.eulerAngles.y, 0);
		horizon = new Vector2 ();

		minZoom = 2f;
		maxZoom = 10f;
		//followSpeed = target.moveSpeed - 0.5f;

	}
	
	// Update is called once per frame
	void Update () {
		//about mouse input
		float mouseX = Input.GetAxis(Strings.Input_Mouse_X);
		float mouseWheel = Input.GetAxis(Strings.Input_Mouse_ScrollWheel);

		Rotate (mouseX);
		Zoom (mouseWheel);

	}

	void LateUpdate(){
		//about move camera
		if(target.movement.magnitude != 0){
			if (target.CheckIsForward(target.movement)) {
				//follow the target behind if movements exists
				Follow ();

			} else
				Back ();
		}

		ApplyChanges ();

		//view the target
		transform.LookAt (target.transform);

	}

	//when characters moves forward
	void Follow(){
		float currYAngle = Mathf.LerpAngle (transform.eulerAngles.y, target.transform.eulerAngles.y, turnSpeed * Time.deltaTime);
		Quaternion targetRotate = Quaternion.Euler (0, currYAngle, 0);
		transform.position = target.transform.position - (targetRotate * Vector3.forward * dist_h) + Vector3.up * dist_v;
		//transform.position = Vector3.Lerp (transform.position, camPos, followSpeed * Time.deltaTime);

	}

	//when characters moves back
	void Back(){
		transform.position = target.transform.position + offset;
	}

	void Rotate(float mouseX){
		transform.RotateAround (target.transform.position, Vector3.up, mouseX * rotateSpeed * Time.deltaTime);
	}

	void Zoom(float mouseWheel){
		Vector3 dist = transform.position - target.transform.position;
		Vector3	toTarget = Vector3.Normalize (dist);
		toTarget *= mouseWheel * turnSpeed;
		if((mouseWheel > 0 && offset.magnitude > minZoom) || (mouseWheel < 0 && offset.magnitude < maxZoom))
			transform.position -= toTarget;
		
	}

	void ApplyChanges(){
		offset = transform.position - target.transform.position;
		camDir = Quaternion.Euler (0, transform.eulerAngles.y, 0);
		horizon.Set (offset.x, offset.z);
		dist_h = horizon.magnitude;
		dist_v = Mathf.Abs (offset.y);
		
	}
}