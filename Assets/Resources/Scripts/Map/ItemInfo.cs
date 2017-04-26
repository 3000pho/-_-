using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constant.Enums;

public class ItemInfo : MapObject{
	public ItemStyle currentItemStyle;
	public BoxCollider boxCollider;
	protected MeshRenderer meshRenderer;
	protected bool isWater;
	protected bool swimable;
	protected bool gettable;

	public void UpdateMaterial(Material m){
		if (meshRenderer == null || m == null)
			return;

		meshRenderer.material = m;
	}

	public override void UpdateType(MapObjType t){
		if (meshRenderer == null || boxCollider == null) {
			if (!SetMembers ())
				return;
		}

		switch (type) {
		case MapObjType.item:
			boxCollider.enabled = false;
			isWater = false;
			swimable = false;
			gettable = false;
			break;
		case MapObjType.decoration:
			boxCollider.enabled = true;
			isWater = false;
			swimable = false;
			gettable = false;
			break;
		case MapObjType.gettable_item:
			boxCollider.enabled = false;
			isWater = false;
			swimable = false;
			gettable = true;
			break;
		case MapObjType.obstacle:
			boxCollider.enabled = true;
			isWater = false;
			swimable = false;
			gettable = false;
			break;
		default:
			return;
		}

		type = t;
	}

	public override void SetOriginal ()
	{
		UpdateType (type);
	}

	public override bool SetMembers ()
	{
		meshRenderer = GetComponent<MeshRenderer> ();
		boxCollider = GetComponent<BoxCollider> ();

		if (meshRenderer == null || boxCollider == null)
			return false;
		else
			return true;
	}
}

