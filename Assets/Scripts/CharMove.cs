using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using cakeslice;
using Constant;

public class CharMove : MonoBehaviour
{
	public Vector3 movement;
	//	public Vector3 center;
	public CameraMove cam;
	public CharState state;
	public ItemInfo item;
	public TileInfo currTile;
	public Image holdProgress;
	public bool fix;

	private ZoneInfo zone;
	private ItemInfo targetItem;
	private Rigidbody rb;
	private Animator animator;
	private Transform equipPosition;
	private List<Collider> enteredTiles;
	//	private Transform dropPosition;
	//private CharacterController controller;
	private bool isHold;
	private float moveSpeed;
	private float rotateSpeed;
	private float holdTime;

	// Use this for initialization
	void Start()
	{
		rb = GetComponent<Rigidbody>();
		animator = GetComponent<Animator>();
		//controller = GetComponent<CharacterController> ();
		Transform[] tempTransforms = GetComponentsInChildren<Transform>();

		foreach (Transform child in tempTransforms)
		{
			if (child.name.Contains(Strings.GameObject_equip_position))
			{
				equipPosition = child;
				//				Debug.Log ("child pos : " + child.name);
			}
		}

		equipPosition = GameObject.Find(Strings.GameObject_equip_position).transform;
		//dropPosition = GameObject.Find (Strings.GameObject_drop_position).transform;
		//center = GameObject.Find (Strings.center).transform.position;

		movement = Vector3.zero;
		enteredTiles = new List<Collider>();
		if(holdProgress)
			holdProgress.fillAmount = 0f;

		state = CharState.idle;
		item = null;
		currTile = null;
		zone = null;
		targetItem = null;
		moveSpeed = 2f;
		rotateSpeed = 6f;
		holdTime = 0f;
		isHold = false;
		fix = false;

	}

	void FixedUpdate()
	{
		if (fix)
		{
			return;
		}

		//move character
		float h = Input.GetAxis(Strings.Input_Horizontal);
		float v = Input.GetAxis(Strings.Input_Vertical);
		movement.Set(h, 0.0f, v);
		//rotate input based camera's direction
		movement = cam.camDir * movement;
		movement *= moveSpeed * Time.fixedDeltaTime;

		//movements exists
		if (movement.magnitude != 0)
		{
			if (state.Equals(CharState.idle))
				state = CharState.walk;
			else if (state.Equals(CharState.pick_up))
				state = CharState.walk_during_up_item;

			//movement = Vector3.Slerp (transform.forward, movement, 0.99f);

			Walk(movement);

		}
		else
		{
			if (state.Equals(CharState.walk))
				state = CharState.idle;
			else if (state.Equals(CharState.walk_during_up_item))
				state = CharState.pick_up;

		}

		//change tile
		SetCurrentTile();

		//interact item
		if (Input.GetKey(KeyCode.Z)) {
			holdTime += Time.fixedDeltaTime;
			holdProgress.fillAmount = holdTime / 0.5f;

			if (holdTime > 0.5f)
			{
				isHold = true;
			}
		}

		if (Input.GetKeyUp(KeyCode.Z))
		{
			if (targetItem)
			{
				if (targetItem.gettable)
				{
					if(item == null)
					{
						if (isHold)
						{
							state = CharState.get_item;
							fix = true;
							targetItem.UpdateType(MapObjType.item);
						}
					}

				}else
				{
					if (item == null)
					{
						state = CharState.pick_up;
						fix = true;
					}
					else if (isHold)
					{
						state = CharState.change_item;
						fix = true;
					}
				}
				

			}
			else if (item)
			{
				state = CharState.put_down;
				fix = true;
			}

			holdTime = 0f;
			isHold = false;
			holdProgress.fillAmount = 0f;
		}

		animator.SetInteger(Strings.Param_state, (int)state);
	}

	void Walk(Vector3 movement)
	{
		rb.MovePosition(transform.position + movement);
		//change directions
		Quaternion newRotate = Quaternion.LookRotation(movement);
		rb.MoveRotation(Quaternion.Slerp(rb.rotation, newRotate, rotateSpeed * Time.fixedDeltaTime));
		//controller.Move (movement);
	}

	//check next moves are forward => -90~90
	public bool CheckIsForward(Vector3 movement)
	{
		Vector3 diff = transform.forward - movement;
		float angle = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;

		if (angle > -90 && angle < 90)
			return true;
		else
			return false;
	}

	void OnTriggerEnter(Collider other)
	{
		//Debug.Log ("enter "+other);
		enteredTiles.Add(other);

	}


	void OnTriggerExit(Collider other)
	{
		//Debug.Log ("exit "+other);
		enteredTiles.Remove(other);

	}

	public void PickUp(ItemInfo target)
	{
		target.transform.SetParent(equipPosition);
		target.transform.localPosition = Vector3.zero;
		target.transform.rotation = new Quaternion(0, 0, 0, 0);
		item = target;

	}

	public void PutDown(ItemInfo target)
	{
		target.transform.SetParent(currTile.transform);
		target.transform.localPosition = new Vector3(0, 0.49f, 0);
		item = null;
	}

	void SetCurrentTile()
	{
		if (enteredTiles.Count <= 0)
		{
			return;
		}

		//find the nearest one
		Collider nearest = enteredTiles[0];
		float dis = Vector3.Distance(transform.position, nearest.transform.position);
		float other_dis;

		for (int i = 1; i < enteredTiles.Count; i++)
		{
			other_dis = Vector3.Distance(transform.position, enteredTiles[i].transform.position);
			if (dis > other_dis)
			{
				dis = other_dis;
				nearest = enteredTiles[i];
			}

		}

		//change current tile
		if (currTile == null || !currTile.triggerCollider.Equals(nearest))
		{
			if (currTile)
				currTile.GetComponent<cakeslice.Outline>().enabled = false;
			currTile = nearest.GetComponent<TileInfo>();
			nearest.GetComponent<cakeslice.Outline>().enabled = true;
			if (currTile.item)
				targetItem = currTile.item.GetComponent<ItemInfo>();
			else
				targetItem = null;

			// 1. enter the zone -> start event
			// 2. after the zone's event -> enter the another zone
			// 3. after the zone's event -> enter the non zone
			if (nearest.gameObject.CompareTag(Strings.Tag_Zone_Member))
			{
				if (zone == null)
				{
					zone = nearest.transform.parent.GetComponent<ZoneInfo>();

					//start event
					Debug.Log("start event");
					zone.eventDone = true;

				}
				else if (!zone.colliders.Contains(nearest))
				{
					zone.eventDone = false;

					zone = nearest.transform.parent.GetComponent<ZoneInfo>();

					//start event
					Debug.Log("start event");
					zone.eventDone = true;
				}

			}
			else
			{
				if (zone)
				{
					zone.eventDone = false;
					zone = null;
				}
			}
		}

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
