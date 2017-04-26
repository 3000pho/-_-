using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constant.Enums;

public class ZoneInfo : MapObject {
	public bool eventDone; // using in CharMove's trigger
	Collider[] zone;

	public override void UpdateType(MapObjType t){
		switch (type) {
		case MapObjType.zone:
			break;
		default:
			return;
		}

		type = t;
	}

	public bool isInZone(Collider c){
		foreach(Collider member in zone){
			if (member.Equals (c))
				return true;
		}

		return false;
	}

	//call if the member object located cross
	public void AddZoneMember(GameObject obj){
		obj.transform.SetParent (transform);
		zone[zone.Length] = obj.GetComponent<BoxCollider> ();
	}

	public override bool SetMembers ()
	{
		throw new System.NotImplementedException ();
	}

	public override void SetOriginal ()
	{
		throw new System.NotImplementedException ();
	}
}
