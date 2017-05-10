using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Constant;

public class MapManager : MonoBehaviour {
	public int minWidth = 1;
	public int minHeight = 1;
	public int maxWidth = 100;
	public int maxHeight = 100;
	public int currentWidth = 8;
	public int currentHeight = 11;
	public GameObject baseTilePrefab;
	public GameObject baseItemPrefab;
	public GameObject baseZonePrefab;
	public Material[] tileMaterials;
	public GameObject[] itemModels;
	public GameObject[,] tiles;
	public List<ZoneInfo> zones = new List<ZoneInfo> ();
	public MapEditMode editMode = MapEditMode.tile_mode;
	public MapObjType editMapObjType = MapObjType.floor;
	public TileStyle editTileStyle = TileStyle.brown_stony;
	public ItemStyle editItemStyle;
	public string fileName = "Map";

	public void CreateTiles(){
		int width = currentWidth;
		int height = currentHeight;
		TileInfo tile;

		tiles = new GameObject[width, height];
		for (int i = 0; i < width; ++i) {
			for (int j = 0; j < height; ++j) {
				tiles [i, j] = Instantiate (baseTilePrefab) as GameObject;
				tiles [i, j].transform.parent = transform;
				tiles [i, j].transform.localPosition = new Vector3 (i, -0.49f, j);
				tiles [i, j].name = i.ToString () + Strings.Param__ + j.ToString ();

				tile = tiles[i,j].GetComponent<TileInfo> ();
				tile.SetMembers ();
				if (!tile.UpdateType (editMapObjType))
					tile.UpdateType (MapObjType.floor);
				tile.currentTileStyle = editTileStyle;
				tile.UpdateMaterial (tileMaterials);

			}
		}
	}

	public void RemoveAllTiles(){
		List<GameObject> gameObjectList = new List<GameObject> ();
		foreach (Transform child in transform) {
			gameObjectList.Add (child.gameObject);
		}

		for (int i = 0; i < gameObjectList.Count; ++i) {
			DestroyGameObject (gameObjectList [i]);
		}

		gameObjectList.Clear ();
		gameObjectList = null;
		tiles = null;
		zones.Clear ();
	}

	public void LoadMapDataFromFile(){
		if (baseTilePrefab == null || baseItemPrefab == null || baseZonePrefab == null) {
			Debug.LogError ("Some prefabs are null.");
			return;
		}

		RemoveAllTiles ();

		string filePath = Strings.Param_Load_Path + fileName;
		TextAsset asset = (TextAsset)Resources.Load (filePath, typeof(TextAsset));

		if (asset == null) {
			Debug.LogError ("Can not open file on Assets/" + filePath + Strings.Param_Save_Extension);
			return;
		}

		TextReader textReader = new StringReader (asset.text);

		// read width and height
		string text = textReader.ReadLine();
		string widthText = text.Substring (text.IndexOf (' ') + 1);
		currentWidth = int.Parse (widthText);

		text = textReader.ReadLine ();
		string heightText = text.Substring (text.IndexOf (' ') + 1);
		currentHeight = int.Parse (heightText);

		// create map
		string[] infos;
		TileInfo tile;
		ZoneInfo zone;
		GameObject obj;
		// load tiles, items
		tiles = new GameObject[currentWidth, currentHeight];

		for (int i = 0; i < currentWidth; ++i) {
			for (int j = 0; j < currentHeight; ++j) {
				text = textReader.ReadLine ();
				infos = text.Split (Strings.Param_tab);

				tiles [i, j] = Instantiate (baseTilePrefab) as GameObject;
				tiles [i, j].name = i.ToString () + Strings.Param__ + j.ToString ();
				tiles [i, j].transform.parent = transform;
				tiles [i, j].transform.localPosition = Strings.GetVector3FromString (infos [0]);
				tiles [i, j].transform.eulerAngles = Strings.GetVector3FromString (infos [1]);
				tiles [i, j].tag = infos [2];

				tile = tiles [i, j].GetComponent<TileInfo> ();
				tile.SetMembers ();
				tile.UpdateType ((MapObjType)(int.Parse (infos [3])));
				tile.currentTileStyle = (TileStyle)(int.Parse (infos [4]));
				tile.UpdateMaterial (tileMaterials);

				if (!infos [5].Equals (Strings.Param_null)) {
					obj = Instantiate(baseItemPrefab) as GameObject;
					obj.name = tile.name + Strings.Param__ + Strings.Param_item;
					obj.transform.parent = tile.transform;
					obj.transform.localPosition = new Vector3 (0, 0.49f, 0);
					tile.item = obj.GetComponent<ItemInfo> ();
					tile.item.UpdateType ((MapObjType)(int.Parse (infos [6])));
					tile.item.currentItemStyle = (ItemStyle)(int.Parse (infos [7]));
					tile.item.UpdateStyle (itemModels);
				}
			}
		}

		// load zones
		if (zones.Count != 0)
			zones.Clear ();
		text = textReader.ReadLine ();
		string zonesCountText = text.Substring (text.IndexOf (' ') + 1);
		int zones_Count = int.Parse (zonesCountText);
		int colliders_Count;
		string[] tile_index;

		for (int k = 0; k < zones_Count; k++) {
			text = textReader.ReadLine ();
			infos = text.Split (Strings.Param_tab);

			obj = Instantiate(baseZonePrefab) as GameObject;
			obj.name = Strings.Param_zone + zones.Count;
			obj.transform.parent = transform;
			obj.transform.localPosition = Vector3.zero;
			zone = obj.GetComponent<ZoneInfo> ();
			zone.UpdateType ((MapObjType)int.Parse (infos [0]));

			colliders_Count = int.Parse (infos [1]);
			for (int l = 0; l < colliders_Count; l++) {
				tile_index = infos [l + 2].Split (Strings.Param__);
				zone.AddZoneMember (tiles [int.Parse (tile_index [0]), int.Parse (tile_index [1])].GetComponent<BoxCollider> ());
			}
			
			zones.Add (zone);
		}

	}


	public void AddOrEditItem(TileInfo tile){
		if (tile.item) {
			tile.item.currentItemStyle = editItemStyle;
			tile.item.UpdateStyle (itemModels);

		} else {
			//create item
			GameObject item = Instantiate(baseItemPrefab) as GameObject;
			item.name = tile.name + Strings.Param__ + Strings.Param_item;
			item.transform.parent = tile.transform;
			item.transform.localPosition = new Vector3 (0, 0.49f, 0);
			ItemInfo newItem = item.GetComponent<ItemInfo> ();
			newItem.UpdateType (editMapObjType);
			newItem.currentItemStyle = editItemStyle;
			newItem.UpdateStyle (itemModels);
			tile.item = newItem;
		}
	}

	public void RemoveItem(TileInfo tile){
		if (tile.item) {
			DestroyGameObject (tile.item.gameObject);
			tile.item = null;
		}
	}

	public bool AddZone(Collider c){
		int i = (int) c.transform.position.x;
		int j = (int) c.transform.position.z;
		List<ZoneInfo> exist = new List<ZoneInfo> ();//left, right, up, down
		bool canAdd = true;
		TileInfo tile = null;
		ZoneInfo tmp;

		//check left
		if (i > 0) {
			tile = tiles [i - 1, j].GetComponent<TileInfo> ();
			if (tile.IsZoneMember ())
				exist.Add (tile.transform.parent.GetComponent<ZoneInfo> ());
		}

		//check right
		if (i < currentWidth - 1) {
			tile = tiles [i + 1, j].GetComponent<TileInfo> ();
			if (tile.IsZoneMember ()) {
				tmp = tile.transform.parent.GetComponent<ZoneInfo> ();
				if(!exist.Contains(tmp))
					exist.Add (tmp);
			}
		}

		//check up
		if (j < currentHeight - 1) {
			tile = tiles [i, j + 1].GetComponent<TileInfo> ();
			if (tile.IsZoneMember ()) {
				tmp = tile.transform.parent.GetComponent<ZoneInfo> ();
				if(!exist.Contains(tmp))
					exist.Add (tmp);
			}
		}

		//check down
		if (j > 0) {
			tile = tiles [i, j - 1].GetComponent<TileInfo> ();
			if (tile.IsZoneMember ()) {
				tmp = tile.transform.parent.GetComponent<ZoneInfo> ();
				if(!exist.Contains(tmp))
					exist.Add (tmp);
			}
		}

		//compare tags
		if(exist.Count > 0){
			string t = exist[0].gameObject.tag;
			for (i = 1; i < exist.Count; i++)
				canAdd &= t.Equals (exist [i].gameObject.tag);
		}

		if (canAdd) {
			if (exist.Count > 0) {
				for (i = 1; i < exist.Count; i++) {
					exist [0].Merge (exist [i]);
					zones.Remove (exist [i]);
					DestroyGameObject (exist [i].gameObject);

				}

				exist[0].AddZoneMember (c);

			} else {
				//create zone
				GameObject zone = Instantiate(baseZonePrefab) as GameObject;
				zone.name = Strings.Param_zone + zones.Count;
				zone.transform.parent = transform;
				zone.transform.position = Vector3.zero;
				ZoneInfo newZone = zone.GetComponent<ZoneInfo> ();
				newZone.UpdateType (editMapObjType);
				newZone.AddZoneMember (c);
				zones.Add (newZone);
			}
		}

		exist.Clear ();
		exist = null;
		return canAdd;
	}

	public void RemoveZone(Collider c){
		ZoneInfo zone = c.transform.parent.GetComponent<ZoneInfo> ();
		if (zone.RemoveZoneMember (c)) {
			zones.Remove (zone);
			DestroyGameObject (zone.gameObject);
		}
	}

	public static void DestroyGameObject(GameObject obj){
		if (Application.isPlaying == true)
			Destroy (obj);
		else
			DestroyImmediate (obj);
	}

	public bool LoadFromHierarchy(){
		bool repairZone = false;
		bool repairColliders;
		int i = 0;
		int j = 0;
		int width = 0;
		int height = 0;
		int zoneCount = 0;
		string[] infos;
		List<GameObject> gameObjectList = new List<GameObject> ();
		List<int> index_i = new List<int> ();
		List<int> index_j = new List<int> ();

		//check game objects from hierarchy in this map manager
		foreach (Transform child in transform) {
			gameObjectList.Add (child.gameObject);
			if (child.gameObject.name.Contains (Strings.Param_zone)) {
				zoneCount++;
				foreach (Transform zoneMember in child) {
					infos = zoneMember.gameObject.name.Split (Strings.Param__);
					index_i.Add (int.Parse (infos [0]));
					index_j.Add (int.Parse (infos [1]));

					if (width < index_i [index_i.Count - 1])
						width = index_i [index_i.Count - 1];
					if (height < index_j [index_j.Count - 1])
						height = index_j [index_j.Count - 1];
				}

			} else {
				infos = child.gameObject.name.Split (Strings.Param__);
				index_i.Add (int.Parse (infos [0]));
				index_j.Add (int.Parse (infos [1]));

				if (width < index_i [index_i.Count - 1])
					width = index_i [index_i.Count - 1];
				if (height < index_j [index_j.Count - 1])
					height = index_j [index_j.Count - 1];
			}
		}

		width += 1;
		height += 1;

		//reload zones
		if (zoneCount != zones.Count) {
			Debug.Log ("reload zones");
			zones.Clear ();
			repairZone = true;
		}

		//reload tiles
		if (currentWidth == width && currentHeight == height) {
			Debug.Log ("reload tiles");
			if (tiles == null)
				tiles = new GameObject[width, height];
			
			foreach (GameObject obj in gameObjectList) {
				if (obj.name.Contains (Strings.Param_zone)) {
					repairColliders = false;
					if (repairZone) {
						zones.Add (obj.GetComponent<ZoneInfo> ());
						if (zones [zones.Count - 1].colliders.Count == 0) {
							Debug.Log ("repair colliders");
							repairColliders = true;
						}
					}

					foreach (Transform zoneMember in obj.transform) {
						tiles[index_i[i++],index_j[j++]] = zoneMember.gameObject;
						if (repairColliders)
							zones [zones.Count - 1].colliders.Add (zoneMember.GetComponent<BoxCollider> ());
					}
				} else {
					tiles [index_i [i++], index_j [j++]] = obj;
				}
			}

		} else {
			if (gameObjectList.Count > 0) {
				Debug.LogWarning ("current game objects in map manager are defferent from currentWidth and currentHeight somehow.");
				gameObjectList.Clear ();
			}

			gameObjectList = null;
			return false;
		}

		gameObjectList.Clear ();
		gameObjectList = null;
		return true;
	}
}
