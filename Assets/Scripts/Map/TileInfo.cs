using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constant;

public class TileInfo : MapObject
{
	public TileStyle currentTileStyle;
	public BoxCollider triggerCollider;
	public ItemInfo item = null;
	public bool isWater;
	public bool swimable;
	protected MeshRenderer meshRenderer;
	protected MeshCollider tileCollider;
	protected BoxCollider boxCollider;

	public void UpdateMaterial(Material[] m)
	{
		if (meshRenderer == null)
		{
			if (!SetMembers())
				return;
		}
		else if (m == null)
		{
			return;
		}

		meshRenderer.material = m[(int)currentTileStyle];
	}

	public bool IsZoneMember()
	{
		return tag.Equals(Strings.Tag_Zone_Member);
	}

	public override bool UpdateType(MapObjType t)
	{
		if (meshRenderer == null || boxCollider == null)
		{
			if (!SetMembers())
				return false;
		}

		switch (t)
		{
			case MapObjType.floor:
				boxCollider.enabled = false;
				isWater = false;
				swimable = false;
				SetIsHalf(false);
				break;
			case MapObjType.wall:
				boxCollider.enabled = true;
				isWater = false;
				swimable = false;
				SetIsHalf(false);
				break;
			case MapObjType.half_wall:
				boxCollider.enabled = true;
				isWater = false;
				swimable = false;
				SetIsHalf(true);
				break;
			case MapObjType.water:
				boxCollider.enabled = false;
				isWater = true;
				swimable = true;
				SetIsHalf(false);
				break;
			default:
				return false;
		}

		type = t;
		return true;
	}

	public override bool SetMembers()
	{
		meshRenderer = GetComponent<MeshRenderer>();
		foreach (BoxCollider b in GetComponents<BoxCollider>())
		{
			if (b.isTrigger)
			{
				triggerCollider = b;
			}
			else
			{
				boxCollider = b;
			}
		}

		if (meshRenderer == null || boxCollider == null || triggerCollider == null)
			return false;
		else
			return true;
	}

	void SetIsHalf(bool isHalf)
	{
		if (boxCollider == null)
		{
			if (!SetMembers())
				return;
		}

		if (isHalf)
		{
			boxCollider.size = new Vector3(5.0f, 0.99f, 5.0f);
		}
		else
		{
			boxCollider.size = new Vector3(10f, 0.99f, 10f);
		}
	}

}
