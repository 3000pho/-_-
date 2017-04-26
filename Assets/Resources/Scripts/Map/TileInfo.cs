using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constant.Enums;

public class TileInfo : MapObject{
	public TileStyle currentTileStyle;
	public BoxCollider boxCollider;
	public MeshCollider tileCollider;
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
		case MapObjType.floor:
			boxCollider.enabled = false;
			isWater = false;
			swimable = false;
			gettable = false;
			SetIsHalf (false);
			break;
		case MapObjType.wall:
			boxCollider.enabled = true;
			isWater = false;
			swimable = false;
			gettable = false;
			SetIsHalf (false);
			break;
		case MapObjType.half_wall:
			boxCollider.enabled = true;
			isWater = false;
			swimable = false;
			gettable = false;
			SetIsHalf (true);
			break;
		case MapObjType.water:
			boxCollider.enabled = false;
			isWater = true;
			swimable = true;
			gettable = false;
			SetIsHalf (false);
			break;
		default:
			return;
		}

		type = t;
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

	public override void SetOriginal ()
	{
		UpdateType (type);
		tileCollider.enabled = true;
	}

	void SetIsHalf(bool isHalf){
		if (boxCollider == null)
			return;

		if (isHalf)
			boxCollider.size.Set (7.5f, 0.99f, 7.5f);
		else
			boxCollider.size.Set (10f, 0.99f, 10f);
		
	}

}
