using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cakeslice;

public class CharMove : MonoBehaviour {

	public enum CharState
	{
		idle = 0,
		walk,
		get_item,
		pick_up,
		down_item,
		walk_during_up_item,
	}
	//delegate void doItem(GameObject target);

	public Vector3 movement;
	public CameraMove cam;
	public bool isForward;
	public bool fix;
	public CharState state;
	public GameObject item;
	public GameObject currTarget;

	private Rigidbody rb;
	private Animator animator;
	private Transform equipPosition;
	private Transform dropPosition;
	//private CharacterController controller;
	private bool isHold;
	private float moveSpeed;
	private float rotateSpeed;
	private int count;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();
		animator = GetComponent<Animator> ();
		//controller = GetComponent<CharacterController> ();
		state = CharState.idle;
		movement = new Vector3 (0, 0, 0);
		moveSpeed = 2f;
		rotateSpeed = 6f;
		isForward = true;
		isHold = false;
		fix = false;
		item = null;
		currTarget = null;
		equipPosition = GameObject.Find ("equip_position").transform;
		dropPosition = GameObject.Find("drop_position").transform;
		count = 0;
	}

	// Update is called once per frame
	void Update () {
		
	}

	void FixedUpdate(){
		if (fix)
			return;
		
		float h = Input.GetAxis ("Horizontal");
		float v = Input.GetAxis ("Vertical");
		movement.Set (h, 0.0f, v);
		//rotate input based camera's direction
		movement = cam.camDir * movement;
		movement *= moveSpeed * Time.deltaTime;

		//movements exists
		if (movement.magnitude != 0) {
			if(IsAnimation("5_during_up_item"))
				state = CharState.walk_during_up_item;
			else
				state = CharState.walk;
			//movement = Vector3.Slerp (transform.forward, movement, 0.99f);

			isForward = CheckIsForward (movement);
			Walk (movement);
			Turn (movement);

		} else {
			if(IsAnimation("6_walk_during_up_item"))
				state = CharState.pick_up;
			else
				state = CharState.idle;
		}
	
		//about item
		if(Input.GetKey (KeyCode.Z) && count++ > 10)
			isHold = true;

		if (Input.GetKeyUp(KeyCode.Z)) {

			if (currTarget) {
				if (item == null) {
					state = CharState.pick_up;

				} else if (isHold) {
					state = CharState.down_item;
					animator.SetBool ("isChange", true);

				}
			} else if (item) {
				state = CharState.down_item;
				animator.SetBool ("isChange", false);
			}

			count = 0;
			isHold = false;
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

	void OnTriggerEnter(Collider other){
		if (other.gameObject.CompareTag("Item")){
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

	public void ChangeItem(GameObject target){
		PutDown (item);
		PickUp (target);
	}
	
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
	
	bool IsAnimation(string name){
		if (animator.GetCurrentAnimatorStateInfo (0).IsName(name))
			return true;
		
//		if (animator.GetAnimatorTransitionInfo (0).IsName("0_idle -> about_item")
//			|| animator.GetAnimatorTransitionInfo(0).IsName("4_down_item -> 3_up_item")
//			|| animator.GetAnimatorTransitionInfo(0).IsName("5_during_up_item -> 4_down_item")) {
//
//			return true;
//		}
		
		return false;
	}
}
