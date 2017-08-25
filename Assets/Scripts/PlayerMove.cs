using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using cakeslice;
using Constant;

public class PlayerMove : MonoBehaviour
{
	public CameraMove cam;
	public Image holdProgress;
	public Transform PanelInventory;

	public Vector3 movement;
	public PlayerState state;
	public ItemInfo item;
	public TileInfo currTile;
	public bool fix;

	Transform equipPosition;
	List<Collider> enteredTiles;

	Inventory inventory;
	ZoneInfo zone;
	ItemInfo targetItem;

	Rigidbody rb;
	Animator animator;
	PlayerState prevState;

	bool isHold;
	float moveSpeed;
	float rotateSpeed;
	float holdTime;


	void Start()
	{
		//initialize
		cam.target = this;
		rb = GetComponent<Rigidbody>();
		animator = GetComponent<Animator>();
		Transform[] tempTransforms = GetComponentsInChildren<Transform>();

		foreach (Transform child in tempTransforms)
		{
			if (child.name.Contains(Strings.GameObject_equip_position))
			{
				equipPosition = child;
			}
		}

		movement = Vector3.zero;
		enteredTiles = new List<Collider>();
		if(holdProgress)
			holdProgress.fillAmount = 0f;
		inventory = new Inventory ();

		string[] infos;
		foreach (Transform panel in PanelInventory) {
			if (panel.name.Equals (Strings.GameObject_PanelItems)) {
				foreach (Transform button in panel) {
					infos = button.name.Substring (6).Split (Strings.Param__);
					int x = int.Parse (infos [1]);
					int y = int.Parse (infos [0]);

					button.GetComponent<Button> ().onClick.AddListener (
						delegate {
							inventory.SetCursor (x, y);
						}
					);

					inventory.invenImage [y, x] = button.GetChild (0).GetComponent<Image> ();
					if (inventory.cursor == null) {
						Transform t = button.GetChild (0).GetChild (0);
						if (t != null) {
							inventory.cursor = t.GetComponent<Image> ();
						}
					}
				}

			} else if (panel.name.Equals (Strings.GameObject_PanelCursor)) {
				inventory.cursorImage = panel.GetChild (0).GetComponent<Image> ();
				inventory.cursorText = panel.GetChild (1).GetComponent<Text> ();
			} else if (panel.name.Equals (Strings.GameObject_PanelCurrent)) {
				inventory.currentImage = panel.GetChild (0).GetComponent<Image> ();
				inventory.currentText = panel.GetChild (1).GetComponent<Text> ();
			}
		}

		state = PlayerState.idle;
		item = null;
		currTile = null;
		zone = null;
		targetItem = null;
		moveSpeed = 2f;
		rotateSpeed = 6f;
		holdTime = 0f;
		isHold = false;
		fix = false;

		//load

	}

	void FixedUpdate()
	{
		if (fix)
		{
			return;
		}

		float h = Input.GetAxis(Strings.Input_Horizontal);
		float v = Input.GetAxis(Strings.Input_Vertical);

		if (state.Equals (PlayerState.inventory)) {
			Debug.Log ("h,v: "+h+","+v);
			inventory.MoveCursor (h, v);

		} else {
			//move character
			movement.Set (h, 0.0f, v);
			//rotate input based camera's direction
			movement = cam.camDir * movement;
			movement *= moveSpeed * Time.fixedDeltaTime;

			//movements exists
			if (movement.magnitude != 0) {
				if (state.Equals (PlayerState.idle))
					state = PlayerState.walk;
				else if (state.Equals (PlayerState.pick_up))
					state = PlayerState.walk_during_up_item;

				//movement = Vector3.Slerp (transform.forward, movement, 0.99f);

				Walk (movement);

			} else {
				if (state.Equals (PlayerState.walk))
					state = PlayerState.idle;
				else if (state.Equals (PlayerState.walk_during_up_item))
					state = PlayerState.pick_up;

			}

			//change tile
			SetCurrentTile ();

			//interact item
			if (Input.GetKey (KeyCode.Z)) {
				holdTime += Time.fixedDeltaTime;
				holdProgress.fillAmount = holdTime / 0.5f;

				if (holdTime > 0.5f) {
					isHold = true;
				}
			}

			if (Input.GetKeyUp (KeyCode.Z)) {
				if (targetItem) {
					if (targetItem.gettable) {
						if (item) {
							// unable to get item message
						} else {
							if (isHold) {
								state = PlayerState.get_item;
								fix = true;
								targetItem.UpdateType (MapObjType.item);
							}
						}

					} else {
						if (item) {
							state = PlayerState.change_item;
							fix = true;
						} else {
							state = PlayerState.pick_up;
							fix = true;
						}
					}
				

				} else if (item) {
					if (isHold) {
						// put item to inventory
						state = PlayerState.put_inventory;
						fix = true;

					} else {
						state = PlayerState.put_down;
						fix = true;
						
					}
				}

				holdTime = 0f;
				isHold = false;
				holdProgress.fillAmount = 0f;
			}
		}

		//inventory
		if (Input.GetKeyUp (KeyCode.F)) {
			PanelInventory.gameObject.SetActive (!PanelInventory.gameObject.activeSelf);
			if (PanelInventory.gameObject.activeSelf) {
				prevState = state;
				state = PlayerState.inventory;
			} else {
				state = prevState;
			}
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
		targetItem = currTile.item;
	}

	public void PutDown(ItemInfo target)
	{
		target.transform.SetParent(currTile.transform);
		target.transform.localPosition = new Vector3(0, 0.49f, 0);
		item = null;
		targetItem = currTile.item;
	}

	public void PutItem(){
		item.gameObject.SetActive (false);
		inventory.AddItem (item);
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
