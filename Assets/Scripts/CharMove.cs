using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharMove : MonoBehaviour {

	public enum CharState
	{
		idle = 0,
		walk,
	}

	public float moveSpeed;
	public float rotateSpeed;
	public Vector3 movement;
	public CameraMove cam;
	public bool isForward;

	private Rigidbody rb;
	private Animator animator;
	private CharState state = CharState.idle;
	//private CharacterController controller;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();
		animator = GetComponent<Animator> ();
		//controller = GetComponent<CharacterController> ();
		movement = new Vector3 (0, 0, 0);
		moveSpeed = 2f;
		rotateSpeed = 6f;
		isForward = true;
	}

	// Update is called once per frame
	void Update () {
		
	}

	void FixedUpdate(){
		float h =  Input.GetAxis ("Horizontal");
		float v = Input.GetAxis ("Vertical");
		movement.Set (h, 0.0f, v);
		//rotate input based camera's direction
		movement = cam.camDir * movement;
		movement *= moveSpeed * Time.deltaTime;

		//movements exists
		if (movement.magnitude != 0) {
			state = CharState.walk;
			//movement = Vector3.Slerp (transform.forward, movement, 0.99f);

			isForward = CheckIsForward (movement);
			Walk (movement);
			Turn (movement);

		} else {
			state = CharState.idle;	
		}

		animator.SetInteger ("state", (int)state);
	}


	void Walk(Vector3 movement){
		rb.MovePosition (transform.position + movement);
		//controller.Move (movement);
	}

	void Turn(Vector3 movement){
		Quaternion newRotate = Quaternion.LookRotation (movement);
		rb.rotation = Quaternion.Slerp (rb.rotation, newRotate, rotateSpeed * Time.deltaTime);

	}

	//check next moves are forward => -90~90
	bool CheckIsForward(Vector3 movement){
		Vector3 diff = transform.forward - movement;
		float angle = Mathf.Atan2 (diff.y, diff.x) * Mathf.Rad2Deg;

		if (angle > -90 && angle < 90)
			return true;
		else
			return false;
	}
}
