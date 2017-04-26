using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constant.Enums;
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
	public GameObject baseZoneRootPrefab;
	public GameObject baseZoneMemberPrefab;
	public Material[] tileMaterials;
	public Material[] itemMaterials;
	public GameObject[,] tiles, items;
	public MapEditMode editMode = MapEditMode.tile_mode;
	public MapObjType editMapObjType = MapObjType.floor;
	public TileStyle editTileStyle = TileStyle.brown_stony;
	public ItemStyle editItemStyle;
	public List<GameObject> zones = new List<GameObject>();
	public string fileName = "Map";

	public void RemoveAllTiles(){
		List<GameObject> gameObjectList = new List<GameObject> ();
		foreach (Transform child in transform) {
			gameObjectList.Add (child.gameObject);
		}

		for (int i = 0; i < gameObjectList.Count; ++i) {
			if (Application.isPlaying == true)
				Destroy (gameObjectList [i]);
			else
				DestroyImmediate (gameObjectList [i]);
		}

		gameObjectList.Clear ();
		gameObjectList = null;
		tiles = null;
	}

	public void LoadMapDataFromFile(){
		RemoveAllTiles ();

		string filePath = Strings.Param_Load_Path + fileName;
		TextAsset asset = (TextAsset)Resources.Load (filePath, typeof(TextAsset));

		if (asset == null) {
			Debug.Log ("Can not open file on Assets/" + filePath + Strings.Param_Save_Extension);
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
		tiles = new GameObject[currentWidth,currentHeight];

		for (int i = 0; i < currentWidth; ++i) {
			for (int j = 0; j < currentHeight; ++j) {
				text = textReader.ReadLine ();
				string[] infos = text.Split (Strings.Param_tab);

				GameObject obj = Instantiate (baseTilePrefab) as GameObject;
				obj.name = i + Strings.Param__ + j;
				obj.transform.localPosition = GetVector3FromString (infos [0]);
				obj.transform.eulerAngles = GetVector3FromString (infos [1]);

				TileInfo tileInfo = obj.GetComponent<TileInfo> ();
				tileInfo.currentTileStyle = (TileStyle)(int.Parse (infos [2]));
				tileInfo.UpdateMaterial (tileMaterials[(int) editTileStyle]);

				tiles [i, j] = obj;
			}	
		}
	}

	// function that changes string (1, 2, 3) to Vector3 value
	public Vector3 GetVector3FromString(string text){
		string newText = text.Replace ('(', ' ');
		newText = newText.Replace (')', ' ');

		string[] datas = newText.Split (',');
		float x = float.Parse (datas [0]);
		float y = float.Parse (datas [1]);
		float z = float.Parse (datas [2]);

		return new Vector3 (x, y, z);
	}

	public bool CheckZoneCross(Transform t){
		Transform left, right, up, down;

		return true;
	}
}
