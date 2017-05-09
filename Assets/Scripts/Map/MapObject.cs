using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constant;

public abstract class MapObject : MonoBehaviour{
	public MapObjType type;

	public abstract bool SetMembers ();
	public abstract bool UpdateType(MapObjType t);

}
