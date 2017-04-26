using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Constant;
using Constant.Enums;
using System.IO;

[CustomEditor(typeof(MapManager))]
public class MapManagerInspector : Editor {
	MapManager mapManager;
	Texture texture = null;
	bool isShowTileFold = false;

	void OnEnable(){
		mapManager = target as MapManager;
		if(texture == null)
			switch (mapManager.editMode) {
			case MapEditMode.tile_mode:
				texture = mapManager.tileMaterials [(int) mapManager.editTileStyle].mainTexture;
				break;
			case MapEditMode.item_mode:
				texture = mapManager.itemMaterials [(int)mapManager.editItemStyle].mainTexture;
				break;
			case MapEditMode.zone_mode:
				break;
			default:
				break;
			}

	}

	public override void OnInspectorGUI ()
	{
		CommonEditorUi.DrawSeparator (Color.cyan);
		DrawMinMaxValues (mapManager);
		CommonEditorUi.DrawSeparator (Color.cyan);
		DrawBasePrefabObject (mapManager);
		DrawCurrentMinMax (mapManager);
		DrawGenerateButton (mapManager);
		CommonEditorUi.DrawSeparator (Color.cyan);
		DrawMaterials (mapManager);
		CommonEditorUi.DrawSeparator (Color.cyan);
		DrawSaveLoadButton (mapManager);
		CommonEditorUi.DrawSeparator (Color.cyan);
		ShowTiles (mapManager);

		if (GUI.changed)
			EditorUtility.SetDirty (target);
	}

	public void OnSceneGUI(){
		if (Application.isPlaying)
			return;
		
		MapManager mapManager = target as MapManager;

		Handles.BeginGUI ();

		if (GUI.Button (new Rect (10, 10, 100, 30), Strings.Label_Tile_Mode)) {
			mapManager.editMode = MapEditMode.tile_mode;

		}
		if (GUI.Button (new Rect (10, 50, 100, 30), Strings.Label_Item_Mode)) {
			mapManager.editMode = MapEditMode.item_mode;
		}
		if (GUI.Button (new Rect (10, 90, 100, 30), Strings.Label_Zone_Mode)) {
			mapManager.editMode = MapEditMode.zone_mode;
		}

		GUI.color = Color.green;
		GUI.Label (new Rect (120, 10, 500, 30), Strings.Label_Edit_Mode + mapManager.editMode);
		GUI.Label (new Rect (120, 50, 500, 30), Strings.Label_MapObjType + mapManager.editMapObjType);
		GUI.color = Color.white;

		if (texture)
			GUI.Box (new Rect (Screen.width - 200, Screen.height - 250, 150, 150), texture);

		Handles.EndGUI ();

		int controlId = GUIUtility.GetControlID (FocusType.Passive);
		HandleUtility.AddDefaultControl (controlId);

		Event e = Event.current;
		if (e.isKey) {
			if (e.character == '3')
				mapManager.editMode = MapEditMode.tile_mode;
			if (e.character == '4')
				mapManager.editMode = MapEditMode.item_mode;
			if (e.character == '5')
				mapManager.editMode = MapEditMode.zone_mode;

			int delta = 0;
			if (e.character == 'a' || e.character == 'A')
				delta = -1;
			if (e.character == 's' || e.character == 'S')
				delta = 1;

			if (delta != 0) {
				if (e.shift) {
					if (!((mapManager.editMapObjType <= 0 && delta < 0) || (mapManager.editMapObjType + 1 >= MapObjType.max_value && delta > 0)))
						mapManager.editMapObjType += delta;

				} else {
					switch (mapManager.editMode) {
					case MapEditMode.tile_mode:
						if ((mapManager.editTileStyle <= 0 && delta < 0) || (mapManager.editTileStyle + 1 >= TileStyle.max_value && delta > 0))
							break;
						mapManager.editTileStyle += delta;
						texture = mapManager.tileMaterials [(int)mapManager.editTileStyle].mainTexture;
						break;
					case MapEditMode.item_mode:
						if ((mapManager.editItemStyle <= 0 && delta < 0) || (mapManager.editItemStyle + 1 >= ItemStyle.max_value && delta > 0))
							break;
						mapManager.editItemStyle += delta;
						texture = mapManager.itemMaterials [(int)mapManager.editItemStyle].mainTexture;
						break;
					case MapEditMode.zone_mode:
						break;
					default:
						break;
					}
				}
			}

//			Handles.EndGUI ();
		}

		if (e.type == EventType.mouseDown || e.type == EventType.mouseDrag) {
//			if (e.alt) {
//				return;
//			}

			Vector2 mousePosition = e.mousePosition;
			Ray ray = HandleUtility.GUIPointToWorldRay (mousePosition);
			RaycastHit hit;

			if (Physics.Raycast (ray, out hit, 1000f)) {
	//			Debug.Log ("mousePosition: "+mousePosition);
	//			Debug.Log ("ray: " + ray.ToString());

				GameObject targetObj = hit.transform.gameObject;
//				Debug.Log ("target transform: " + hit.transform.position);

				if (e.button == 0) { // left click button
					if (e.shift) { // while shift key pressed
						
					} else {
						switch (mapManager.editMode) {
						case MapEditMode.tile_mode:
							TileInfo tileInfo = targetObj.GetComponent<TileInfo> ();
							tileInfo.currentTileStyle = mapManager.editTileStyle;
							tileInfo.UpdateMaterial (mapManager.tileMaterials[(int) mapManager.editTileStyle]);
							break;
						case MapEditMode.item_mode:
							ItemInfo itemInfo = targetObj.GetComponent<ItemInfo> ();

							break;
						case MapEditMode.zone_mode:
							break;
						default:
							break;
						}
					}

				} else if (e.button == 1) { // right click button
					if (e.shift) { // while shift key pressed
						
					} else {
						hit.transform.localEulerAngles += new Vector3 (0f, 90f, 0f);
					}
				}
			}
		}

	}

	public void CreateTiles(MapManager mapManger){
		int width = mapManger.currentWidth;
		int height = mapManger.currentHeight;

		mapManger.tiles = new GameObject[width, height];
		for (int i = 0; i < width; ++i) {
			for (int j = 0; j < height; ++j) {
				GameObject obj = Instantiate (mapManger.baseTilePrefab) as GameObject;
				obj.transform.parent = mapManger.transform;
				obj.transform.localPosition = new Vector3 (i, -0.499f, j);
				obj.name = i + "_" + j;
				mapManger.tiles [i, j] = obj;

				TileInfo tileInfo = mapManger.tiles [i, j].GetComponent<TileInfo> ();
				tileInfo.SetMembers ();
				tileInfo.currentTileStyle = mapManger.editTileStyle;
				tileInfo.UpdateMaterial (mapManger.tileMaterials[(int) mapManger.editTileStyle]);
			}
		}
	}

	public void ShowTiles(MapManager mapManger){
		if (mapManger.tiles == null) {
			GUI.color = Color.red;
			GUILayout.TextField ("=== no tiles ===");
			GUI.color = Color.white;
			return;
		}

		isShowTileFold = EditorGUILayout.Foldout (isShowTileFold, Strings.Label_Tiles_List);
		if (isShowTileFold == false)
			return;

		int width = mapManger.tiles.GetLength (0);
		int height = mapManger.tiles.GetLength (1);
		EditorGUILayout.LabelField (Strings.Label_Tiles_Width + width);
		EditorGUILayout.LabelField (Strings.Label_Tiles_Height + height);

		for (int i = 0; i < width; ++i) {
			for (int j = 0; j < height; ++j) {
				string text = string.Format (Strings.Format_Tile_Index, i, j);
				EditorGUILayout.ObjectField (text, mapManger.tiles [i, j], typeof(GameObject), true);
			}
		}
	}

	void DrawMinMaxValues(MapManager mapManager){
		//minWidth, minHeight
		GUI.color = (mapManager.minWidth > 0 && mapManager.minWidth < 100) ? Color.green : Color.red;
		int minWidth = EditorGUILayout.IntField (Strings.Param_Map_Min_Width, mapManager.minWidth);
		if (minWidth != mapManager.minWidth) {
			CommonEditorUi.RegisterUndo (Strings.Param_Map_Min_Width, mapManager);
			mapManager.minWidth = minWidth;
		}

		GUI.color = (mapManager.minHeight > 0 && mapManager.minHeight < 100) ? Color.green : Color.red;
		int minHeight = EditorGUILayout.IntField (Strings.Param_Map_Min_Height, mapManager.minHeight);
		if (minHeight != mapManager.minHeight) {
			CommonEditorUi.RegisterUndo (Strings.Param_Map_Min_Height, mapManager);
			mapManager.minHeight = minHeight;
		}

		//maxWidth, maxHeight
		GUI.color = (mapManager.maxWidth >= mapManager.minWidth && mapManager.maxWidth <= 100) ? Color.green : Color.red;
		int maxWidth = EditorGUILayout.IntField (Strings.Param_Map_Max_Width, mapManager.maxWidth);
		if (maxWidth != mapManager.maxWidth) {
			CommonEditorUi.RegisterUndo (Strings.Param_Map_Max_Width, mapManager);
			mapManager.maxWidth = maxWidth;
		}
		GUI.color = Color.white;

		GUI.color = (mapManager.maxHeight > 0 && mapManager.maxHeight <= 100) ? Color.green : Color.red;
		int maxHeight = EditorGUILayout.IntField (Strings.Param_Map_Max_Height, mapManager.maxHeight);
		if (maxHeight != mapManager.maxHeight) {
			CommonEditorUi.RegisterUndo (Strings.Param_Map_Max_Height, mapManager);
			mapManager.maxHeight = maxHeight;
		}
		GUI.color = Color.white;
	}

	void DrawCurrentMinMax(MapManager mapManger){
		GUI.color = (mapManger.currentWidth >= mapManger.minWidth && mapManger.currentWidth <= mapManger.maxWidth) ? Color.green : Color.red;
		int width = EditorGUILayout.IntField (Strings.Param_Current_Map_Width, mapManger.currentWidth);
		if (width != mapManger.currentWidth) {
			CommonEditorUi.RegisterUndo (Strings.Param_Current_Map_Width, mapManger);
			mapManger.currentWidth = width;
		}
		GUI.color = Color.white;

		GUI.color = (mapManger.currentHeight >= mapManger.minHeight && mapManger.currentHeight <= mapManger.maxHeight) ? Color.green : Color.red;
		int height = EditorGUILayout.IntField (Strings.Param_Current_Map_Height, mapManger.currentHeight);
		if (height != mapManger.currentHeight) {
			CommonEditorUi.RegisterUndo (Strings.Param_Current_Map_Height, mapManger);
			mapManger.currentHeight = height;
		}
		GUI.color = Color.white;
	}

	void DrawBasePrefabObject(MapManager mapManager){
		GUI.color = (mapManager.baseTilePrefab != null) ? Color.green : Color.red;
		GameObject baseTilePrefab = (GameObject)EditorGUILayout.ObjectField (Strings.Param_Map_Base_Tile, mapManager.baseTilePrefab, typeof(GameObject), false);

		if (baseTilePrefab != mapManager.baseTilePrefab) {
			CommonEditorUi.RegisterUndo (Strings.Param_Map_Base_Tile, mapManager);
			mapManager.baseTilePrefab = baseTilePrefab;
		}
		GUI.color = Color.white;

		GUI.color = (mapManager.baseItemPrefab != null) ? Color.green : Color.red;
		GameObject baseItemPrefab = (GameObject)EditorGUILayout.ObjectField (Strings.Param_Map_Base_Item, mapManager.baseItemPrefab, typeof(GameObject), false);

		if (baseItemPrefab != mapManager.baseItemPrefab) {
			CommonEditorUi.RegisterUndo (Strings.Param_Map_Base_Item, mapManager);
			mapManager.baseItemPrefab = baseItemPrefab;
		}
		GUI.color = Color.white;

		GUI.color = (mapManager.baseZoneRootPrefab != null) ? Color.green : Color.red;
		GameObject baseZoneRootPrefab = (GameObject)EditorGUILayout.ObjectField (Strings.Param_Map_Base_Zone_Root, mapManager.baseZoneRootPrefab, typeof(GameObject), false);

		if (baseZoneRootPrefab != mapManager.baseZoneRootPrefab) {
			CommonEditorUi.RegisterUndo (Strings.Param_Map_Base_Zone_Root, mapManager);
			mapManager.baseZoneRootPrefab = baseZoneRootPrefab;
		}
		GUI.color = Color.white;

		GUI.color = (mapManager.baseZoneMemberPrefab != null) ? Color.green : Color.red;
		GameObject baseZoneMemberPrefab = (GameObject)EditorGUILayout.ObjectField (Strings.Param_Map_Base_Zone_Member, mapManager.baseZoneMemberPrefab, typeof(GameObject), false);

		if (baseZoneMemberPrefab != mapManager.baseZoneMemberPrefab) {
			CommonEditorUi.RegisterUndo (Strings.Param_Map_Base_Zone_Member, mapManager);
			mapManager.baseZoneMemberPrefab = baseZoneMemberPrefab;
		}
		GUI.color = Color.white;
	}

	void DrawGenerateButton(MapManager mapManager){
		EditorGUILayout.BeginHorizontal ();

		if (GUILayout.Button (Strings.Param_Generate_Map_Data)) {
//			Debug.Log (Strings.Param_Generate_Map_Data);
			CreateTiles(mapManager);
		}

		if (GUILayout.Button (Strings.Param_Remove_Map_Data)) {
//			Debug.Log (Strings.Param_Remove_Map_Data);
			mapManager.RemoveAllTiles();
		}

		EditorGUILayout.EndHorizontal ();
	}

	void DrawMaterials(MapManager mapManager){
		serializedObject.Update ();
		EditorList.Show (serializedObject.FindProperty (Strings.Param_tileMaterials));
		EditorList.Show (serializedObject.FindProperty (Strings.Param_itemMaterials));
		serializedObject.ApplyModifiedProperties ();
	}

	void DrawSaveLoadButton(MapManager mapManager){
		string fileName = EditorGUILayout.TextField (Strings.Param_Map_File_Name, mapManager.fileName);
		if (fileName != mapManager.fileName) {
			CommonEditorUi.RegisterUndo (Strings.Param_Map_File_Name, mapManager);
			mapManager.fileName = fileName;
		}

		EditorGUILayout.BeginHorizontal ();

		if (GUILayout.Button (Strings.Param_Save_Map_Data)) {
			string title = Strings.Param_Save_Title;
			string msg = Strings.Param_Save_Msg;
			if (EditorUtility.DisplayDialog (title, msg, Strings.Param_yes, Strings.Param_no) == false)
				return;

			string dataPath = Application.dataPath;
			string fullPath = dataPath + Strings.Param_Save_Path + fileName + Strings.Param_Save_Extension;
			FileStream fs = new FileStream (fullPath, FileMode.Create);
			TextWriter textWriter = new StreamWriter (fs);
			int width = mapManager.currentWidth;
			int height = mapManager.currentHeight;
			textWriter.Write (Strings.Param_Save_width + width + Strings.Param_enter);
			textWriter.Write (Strings.Param_Save_height + height + Strings.Param_enter);

			for (int i = 0; i < width; ++i) {
				for (int j = 0; j < height; ++j) {
					Transform tile = mapManager.tiles [i, j].transform;
					textWriter.Write (tile.position + char.ToString(Strings.Param_tab));
					textWriter.Write (tile.eulerAngles + char.ToString(Strings.Param_tab));

					TileInfo tileInfo = tile.GetComponent<TileInfo> ();
					textWriter.Write ((int)tileInfo.currentTileStyle + Strings.Param_tab);
					textWriter.Write (Strings.Param_enter);
				}
			}

			// save house

			textWriter.Close ();

			// backup
			string backupPath = dataPath + Strings.Param_Save_Path + fileName + Strings.Param__backup + Strings.Param_Save_Extension;
			if (File.Exists (backupPath) == false)
				FileUtil.CopyFileOrDirectory (fullPath, backupPath);

			AssetDatabase.Refresh ();
		}
		if (GUILayout.Button (Strings.Param_Load_Map_Data)) {
			
		}

		EditorGUILayout.EndHorizontal ();
	}

	bool GetHit(Vector3 pos, out RaycastHit hit){
		Ray ray = new Ray (pos, Vector3.down);
		return Physics.Raycast (ray, out hit, 1000f);
	}

	public static class EditorList{
		public static void Show(SerializedProperty list){
			EditorGUILayout.PropertyField (list);
			EditorGUI.indentLevel += 1;
			if (list.isExpanded) {
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("Array.size"));
				for (int i = 0; i < list.arraySize; i++) {
					EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i));
				}
			}
			EditorGUI.indentLevel -= 1;
		}
	}

}
