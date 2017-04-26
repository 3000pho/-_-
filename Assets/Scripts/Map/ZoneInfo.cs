using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constant.Enums;

public class ZoneInfo : MapObject {
	public bool eventDone = false; // using in CharMove's trigger
	List<Collider> colliders = new List<Collider>();

	public override void UpdateType(MapObjType t){
		switch (type) {
		case MapObjType.zone:
			break;
		default:
			return;
		}

		type = t;
	}

	//call if the member object located cross
	public void AddZoneMember(Collider c){
		c.gameObject.transform.SetParent (transform);
		colliders.Add (c);

	}

	public void RemoveZoneMember(Collider c, bool isDestroy = true){
		c.gameObject.transform.SetParent (null);
		colliders.Remove (c);
		if(isDestroy)
			Destroy (c.gameObject);
	}

	public void Merge(ZoneInfo zone){
		foreach(Collider c in zone.colliders){
			zone.RemoveZoneMember (c, false);
			AddZoneMember (c);
		}
		Destroy (zone);
	}

	public void Destroy(){
		colliders.Clear ();
		Destroy (gameObject);
	}

	public override bool SetMembers ()
	{
		throw new System.NotImplementedException ();
	}

	public override void SetColliders (bool enable){
		foreach (Collider c in colliders) {
			c.enabled = enable;
		}
		
	}
}
