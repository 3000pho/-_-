using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constant;

public class ZoneInfo : MapObject {
	public bool eventDone = false; // using in CharMove's trigger
	public List<Collider> colliders = new List<Collider>();

	public override bool UpdateType(MapObjType t){
		switch (t) {
		case MapObjType.zone_portal:
			gameObject.tag = Strings.Tag_Portal;
			break;
		case MapObjType.zone_autosave:
			gameObject.tag = Strings.Tag_Auto_Save;
			break;
		case MapObjType.zone_event:
			gameObject.tag = Strings.Tag_Event;
			break;
		default:
			return false;
		}

		type = t;
		return true;
	}

	//call if the member object located cross
	public void AddZoneMember(Collider c){
		c.gameObject.tag = Strings.Tag_Zone_Member;
		c.gameObject.transform.SetParent (transform);
		colliders.Add (c);
	}

	public void Merge(ZoneInfo zone){
		foreach (Collider c in zone.colliders) {
			c.gameObject.transform.SetParent (transform);
			colliders.Add (c);
		}
		zone.colliders.Clear ();
		zone.colliders = null;
	}

	//if there's no child return true
	public bool RemoveZoneMember(Collider c){
		c.gameObject.tag = Strings.Tag_Untagged;
		c.gameObject.transform.SetParent (transform.parent);
		colliders.Remove (c);
		if (colliders.Count == 0) {
			colliders = null;
			return true;
		}

		return false;
	}

	public void RemoveAllZoneMembers(){
		if(colliders.Count > 0)
			while(!RemoveZoneMember(colliders[0]));
	}

	public override bool SetMembers ()
	{
		throw new System.NotImplementedException ();
	}

}
