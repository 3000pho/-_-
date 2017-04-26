using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constant.Enums;

public abstract class MapObject : MonoBehaviour{
	protected MapObjType type;

	public abstract bool SetMembers ();
	public abstract void UpdateType(MapObjType t);
	public abstract void SetOriginal();
}
