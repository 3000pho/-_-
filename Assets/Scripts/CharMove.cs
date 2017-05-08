using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cakeslice;
using Constant;

public class CharMove : MonoBehaviour {
	
	public Vector3 movement;
//	public Vector3 center;
	public CameraMove cam;
	public CharState state;
	public GameObject item;
	public GameObject currTarget;
	public bool fix;

	private Rigidbody rb;
	private Animator animator;
	private Transform equipPosition;
	private Transform dropPosition;
	//private CharacterController controller;
	private bool isHold;
	private float moveSpeed;
	private float rotateSpeed;
	private float holdTime;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();
		animator = GetComponent<Animator> ();
		//controller = GetComponent<CharacterController> ();
		Transform[] tempTransforms = GetComponentsInChildren<Transform>();

		foreach (Transform child in tempTransforms)
		{ 
//			if (child.name.Contains (Strings._root)) { 
//				center = child.GetComponentInChildren<Transform> ().position;
//				Debug.Log ("child pos : " + child.name);
//			} else if (child.name.Contains (Strings.equip_position)) {
			if (child.name.Contains (Strings.GameObject_equip_position)) {
				equipPosition = child;
//				Debug.Log ("child pos : " + child.name);
			} else if (child.name.Contains (Strings.GameObject_drop_position)) {
				dropPosition = child;
//				Debug.Log ("child pos : " + child.name);
				break;
			}
		}

//		equipPosition = GameObject.Find (Strings.equip_position).transform;
//		dropPosition = GameObject.Find (Strings.drop_position).transform;
//		center = GameObject.Find (Strings.center).transform.position;

		movement = new Vector3 (0, 0, 0);

		state = CharState.idle;
		item = null;
		currTarget = null;
		moveSpeed = 2f;
		rotateSpeed = 6f;
		holdTime = 0f;
		isHold = false;
		fix = false;

	}

	void FixedUpdate(){
		if (fix) {
			return;
		}

		//about move
		float h = Input.GetAxis (Strings.Input_Horizontal);
		float v = Input.GetAxis (Strings.Input_Vertical);
		movement.Set (h, 0.0f, v);
		//rotate input based camera's direction
		movement = cam.camDir * movement;
		movement *= moveSpeed * Time.fixedDeltaTime;

		//movements exists
		if (movement.magnitude != 0) {
			if (state.Equals (CharState.idle))
				state = CharState.walk;
			else if (state.Equals (CharState.pick_up))
				state = CharState.walk_during_up_item;

			//movement = Vector3.Slerp (transform.forward, movement, 0.99f);

			Walk (movement);

		} else {
			if (state.Equals (CharState.walk))
				state = CharState.idle;
			else if (state.Equals (CharState.walk_during_up_item))
				state = CharState.pick_up;

		}

		//about item
		if(Input.GetKey (KeyCode.Z) && (holdTime += Time.fixedDeltaTime) > 0.5f)
			isHold = true;

		if (Input.GetKeyUp(KeyCode.Z)) {
			if (currTarget) {
				if (item == null) {
					state = CharState.pick_up;
					fix = true;

				} else if (isHold) {
					state = CharState.change_item;
					fix = true;
				}

			} else if (item) {
				state = CharState.put_down;
				fix = true;
			}

			holdTime = 0;
			isHold = false;
		}

		animator.SetInteger (Strings.Param_state, (int)state);
	}

	void Walk(Vector3 movement){
		rb.MovePosition (transform.position + movement);
		//change directions
		Quaternion newRotate = Quaternion.LookRotation (movement);
		rb.MoveRotation (Quaternion.Slerp (rb.rotation, newRotate, rotateSpeed * Time.fixedDeltaTime));
		//controller.Move (movement);
	}

	//check next moves are forward => -90~90
	public bool CheckIsForward(Vector3 movement){
		Vector3 diff = transform.forward - movement;
		float angle = Mathf.Atan2 (diff.y, diff.x) * Mathf.Rad2Deg;

		if (angle > -90 && angle < 90)
			return true;
		else
			return false;
	}

	void OnTriggerEnter(Collider other){
		if (other.gameObject.CompareTag(Strings.Tag_Item)){
			if (currTarget)
				currTarget.GetComponentInChildren<Outline> ().enabled = false;
			currTarget = other.gameObject;
			currTarget.GetComponentInChildren<Outline> ().enabled = true;

		}
	}

	void OnTriggerExit(Collider other){
		if (currTarget == other.gameObject) {
			currTarget.GetComponentInChildren<Outline> ().enabled = false;
			currTarget = null;
		}
	}

	public void PickUp(GameObject target){
		target.transform.SetParent (equipPosition);
		target.transform.localPosition = Vector3.zero;
		target.transform.rotation = new Quaternion (0, 0, 0, 0);
		item = target;
	}

	public void PutDown(GameObject target){
		target.transform.SetParent (null);
		target.transform.position = dropPosition.position;
		item = null;
	}

//	IEnumerator waitForTransition(bool value) {
//		yield return new WaitForEndOfFrame();
//		isCanItem = value;
//	}

//	IEnumerator waitForPassFrame(int frame, doItem func, GameObject target) {
//		int playFrame = 0;
//
//		while (playFrame++ < frame) {
//			//print ("wait for end of frame..." + playFrame.ToString());
//			yield return new WaitForEndOfFrame();
//		}
//
//		func(target);
//	}
	
//	bool IsPlayingAnimation(string name){
//		if (animator.GetCurrentAnimatorStateInfo (0).IsName(name))
//			return true;
//		
////		if (animator.GetAnimatorTransitionInfo (0).IsName("0_idle -> about_item")
////			|| animator.GetAnimatorTransitionInfo(0).IsName("4_down_item -> 3_up_item")
////			|| animator.GetAnimatorTransitionInfo(0).IsName("5_during_up_item -> 4_down_item")) {
////
////			return true;
////		}
//		
//		return false;
//	}
}
