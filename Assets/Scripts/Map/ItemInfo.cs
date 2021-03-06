﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constant;

public class ItemInfo : MapObject
{
	public ItemStyle currentItemStyle;
	public BoxCollider boxCollider;
	public bool gettable;
	protected int itemCode;

	public void UpdateStyle(GameObject[] m)
	{
		if (m == null)
			return;

		if (transform.childCount > 0)
		{
			//remove current model and change the model
			MapManager.DestroyGameObject(transform.GetChild(0).gameObject);

		}

		//add model from fbx file
		GameObject obj = Instantiate(m[(int)currentItemStyle]) as GameObject;
		obj.transform.parent = transform;
		obj.transform.localPosition = new Vector3(0, -0.49f, 0);
		obj.transform.localScale = GetResizedVector3(obj);

	}

	public override bool UpdateType(MapObjType t)
	{
		if (boxCollider == null)
		{
			if (!SetMembers())
				return false;
		}

		BoxCollider box = transform.parent.GetComponent<BoxCollider>();

		switch (t)
		{
			case MapObjType.item:
				boxCollider.enabled = false;
				box.isTrigger = true;
				gettable = false;
				break;
			case MapObjType.decoration:
				boxCollider.enabled = true;
				box.isTrigger = true;
				gettable = false;
				break;
			case MapObjType.gettable_item:
				boxCollider.enabled = false;
				box.isTrigger = true;
				gettable = true;
				break;
			case MapObjType.obstacle:
				boxCollider.enabled = true;
				box.isTrigger = false;
				gettable = false;
				break;
			default:
				return false;
		}

		type = t;
		return true;
	}

	public override bool SetMembers()
	{
		boxCollider = GetComponent<BoxCollider>();

		if (boxCollider == null)
			return false;
		else
			return true;
	}

	Vector3 GetResizedVector3(GameObject obj)
	{
		LODGroup lodGroup = obj.GetComponent<LODGroup>();
		if (lodGroup)
		{
			float resized = boxCollider.size.x / lodGroup.size * 0.9f;
			return new Vector3(resized, resized, resized);
		}
		return obj.transform.localScale;
	}

	public override bool Equals (object other)
	{
		if (other != null && other is ItemInfo) {
			return itemCode.Equals ((other as ItemInfo).itemCode);
		}
		return false;
	}
}

