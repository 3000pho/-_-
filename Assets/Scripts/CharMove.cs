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
		change_item,
		put_down,
	}
	delegate void doItem(GameObject target);

	public Vector3 movement;
	public CameraMove cam;
	public bool isForward;

	private Rigidbody rb;
	private Animator animator;
	private CharState state;
	private GameObject item;
	private GameObject currTarget;
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

		//about item
		if(Input.GetKey (KeyCode.Z) && count++ > 10)
			isHold = true;

		if (Input.GetKeyUp(KeyCode.Z)) {
			doItem func;

			if (currTarget) {
				if (item == null) {
					state = CharState.pick_up;
					func = new doItem (PickUp);
					StartCoroutine (waitForPassFrame(9,func, currTarget));

				} else if (isHold) {
					state = CharState.change_item;
					animator.SetBool ("isChange", true);
					func = new doItem (ChangeItem);
					StartCoroutine (waitForPassFrame (15,func,currTarget));

				}
			} else if(item){
				state = CharState.put_down;
				animator.SetBool ("isChange", false);
				func = new doItem (PutDown);
				StartCoroutine (waitForPassFrame (6, func, item));
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
			currTarget = other.gameObject;
			currTarget.GetComponentInChildren<Outline> ().enabled = true;

		}
	}

	void OnTriggerExit(Collider other){
		if (currTarget) {
			currTarget.GetComponentInChildren<Outline> ().enabled = false;
			currTarget = null;
		}
	}

	void PickUp(GameObject target){
		target.transform.SetParent (equipPosition);
		target.transform.localPosition = Vector3.zero;
		target.transform.rotation = new Quaternion (0, 0, 0, 0);
		item = target;
	}

	void PutDown(GameObject target){
		target.transform.SetParent (null);
		target.transform.position = dropPosition.position;
		item = null;
	}

	void ChangeItem(GameObject target){
		PutDown (item);
		PickUp (target);
		state = CharState.pick_up;
	}

	IEnumerator waitForPassFrame(int frame, doItem func, GameObject target) {
		int playFrame = 0;

		while (playFrame++ < frame) {
			//print ("wait for end of frame..." + playFrame.ToString());
			yield return new WaitForEndOfFrame();
		}

		func(target);
	}
		
}
