using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constant;

public class TileInfo : MapObject{
	public TileStyle currentTileStyle;
	public BoxCollider boxCollider;
	public ItemInfo item = null;
	public bool isWater;
	public bool swimable;
	protected MeshRenderer meshRenderer;
	protected MeshCollider tileCollider;

	public void UpdateMaterial(Material[] m){
		if (meshRenderer == null) {
			if (!SetMembers ())
				return;
		} else if (m == null) {
			return;
		}

		meshRenderer.material = m [(int)currentTileStyle];
	}

	public bool IsZoneMember(){
		return tag.Equals(Strings.Tag_Zone_Member);
	}

	public override bool UpdateType(MapObjType t){
		if (meshRenderer == null || boxCollider == null) {
			if (!SetMembers ())
				return false;
		}

		switch (t) {
		case MapObjType.floor:
			boxCollider.isTrigger = true;
			isWater = false;
			swimable = false;
			SetIsHalf (false);
			break;
		case MapObjType.wall:
			boxCollider.isTrigger = false;
			isWater = false;
			swimable = false;
			SetIsHalf (false);
			break;
		case MapObjType.half_wall:
			boxCollider.isTrigger = false;
			isWater = false;
			swimable = false;
			SetIsHalf (true);
			break;
		case MapObjType.water:
			boxCollider.isTrigger = true;
			isWater = true;
			swimable = true;
			SetIsHalf (false);
			break;
		default:
			return false;
		}

		type = t;
		return true;
	}

	public override bool SetMembers ()
	{
		meshRenderer = GetComponent<MeshRenderer> ();
		boxCollider = GetComponent<BoxCollider> ();
		tileCollider = GetComponent<MeshCollider> ();

		if (meshRenderer == null || boxCollider == null || tileCollider == null)
			return false;
		else
			return true;
	}

	void SetIsHalf(bool isHalf){
		if (boxCollider == null) {
			if(!SetMembers())
				return;
		}

		if (isHalf) {
			boxCollider.size = new Vector3 (7.5f, 0.99f, 7.5f);
		} else {
			boxCollider.size = new Vector3 (10f, 0.99f, 10f);
		}
	}

}
