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
		moveSpeed = 1f;
		rotateSpeed = 6f;
	}

	// Update is called once per frame
	void Update () {
		
	}

	void FixedUpdate(){
		float h =  Input.GetAxis ("Horizontal");
		float v = Input.GetAxis ("Vertical");
		movement.Set (h, 0.0f, v);
		//rotate input based charater's direction
		movement = Quaternion.Euler (0, transform.eulerAngles.y, 0) * movement;
		movement *= moveSpeed * Time.deltaTime;

		//movements exists
		if (movement.magnitude != 0) {
			state = CharState.walk;

			Walk (movement);
			Turn (movement);

		} else {
			state = CharState.idle;	
		}

		animator.SetInteger ("state", (int)state);
	}

	void Walk(Vector3 movement){
		rb.MovePosition (transform.position + Vector3.Slerp(transform.forward, movement, 0.99f));
		//controller.Move (movement);
	}

	void Turn(Vector3 movement){
		Quaternion newRotate = Quaternion.LookRotation (movement);
		rb.rotation = Quaternion.Slerp (rb.rotation, newRotate, rotateSpeed * Time.deltaTime);

	}
}
