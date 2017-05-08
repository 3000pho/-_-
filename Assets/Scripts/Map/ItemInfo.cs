using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constant;

public class ItemInfo : MapObject{
	public ItemStyle currentItemStyle;
	public BoxCollider boxCollider;
	public bool gettable;
	protected MeshRenderer meshRenderer;
	protected int itemCode{ get; set;}

	public void UpdateStyle(Material[] m){
		if (meshRenderer == null || m == null)
			return;

		meshRenderer.material = m [(int)currentItemStyle];
	}

	public override bool UpdateType(MapObjType t){
		if (meshRenderer == null || boxCollider == null) {
			if (!SetMembers ())
				return false;
		}

		switch (t) {
		case MapObjType.item:
			boxCollider.enabled = false;
			gettable = false;
			break;
		case MapObjType.decoration:
			boxCollider.enabled = true;
			gettable = false;
			break;
		case MapObjType.gettable_item:
			boxCollider.enabled = false;
			gettable = true;
			break;
		case MapObjType.obstacle:
			boxCollider.enabled = true;
			gettable = false;
			break;
		default:
			return false;
		}

		type = t;
		return true;
	}

	public override void SetColliders (bool enable)
	{
		if (enable) {
			UpdateType (type);
		} else {
			boxCollider.enabled = false;
		}
	}

	public override bool SetMembers ()
	{
		meshRenderer = GetComponent<MeshRenderer> ();

		if (meshRenderer == null || boxCollider == null)
			return false;
		else
			return true;
	}
}

