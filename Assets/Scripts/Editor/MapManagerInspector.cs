using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using System.IO;
using Constant;

[CustomEditor(typeof(MapManager))]
public class MapManagerInspector : Editor
{
	PreviewRenderUtility previewRenderUtil = null;
	MapManager mapManager;
	Texture texture = null;
	GameObject preview = null;
	bool isShowTileFold = false;
	bool isShowZoneFold = false;

	void OnEnable()
	{
		mapManager = target as MapManager;

		//		var flags = BindingFlags.Static | BindingFlags.NonPublic;
		//		var propInfo = typeof(Camera).GetProperty (Strings.Param_PreviewCullingLayer, flags);
		//		int previewLayer = (int)propInfo.GetValue (null, new object[0]);

		switch (mapManager.editMode)
		{
			case MapEditMode.tile_mode:
				if (texture == null)
					texture = mapManager.tileMaterials[(int)mapManager.editTileStyle].mainTexture;
				break;
			case MapEditMode.item_mode:
				if (preview == null)
				{
					preview = mapManager.itemModels[(int)mapManager.editItemStyle];
					//				preview = Instantiate (mapManager.itemModels [(int)mapManager.editItemStyle]);
					//				preview.hideFlags = HideFlags.HideAndDontSave;
					//				preview.SetActive (false);
					//				preview.layer = previewLayer;
					//				foreach (Transform t in preview.transform) {
					//					t.gameObject.layer = previewLayer;
					//				}
				}
				break;
			case MapEditMode.zone_mode:
				break;
			default:
				break;
		}

		if (previewRenderUtil == null)
		{
			previewRenderUtil = new PreviewRenderUtility(true);
			previewRenderUtil.m_CameraFieldOfView = 30f;
			previewRenderUtil.m_Camera.nearClipPlane = 0.3f;
			previewRenderUtil.m_Camera.farClipPlane = 1000f;
			//			previewRenderUtil.m_Camera.cullingMask = 1 << previewLayer;
		}
	}

	void OnDisable()
	{
		previewRenderUtil.Cleanup();
		previewRenderUtil = null;
		//		if (preview)
		//			MapManager.DestroyGameObject (preview);
		preview = null;
	}

	public override bool HasPreviewGUI()
	{
		if (preview)
			return true;
		return false;
	}

	public override GUIContent GetPreviewTitle()
	{
		return new GUIContent(Strings.Param_preview + char.ToString(Strings.Param__) + preview.name);
	}

	public override void OnPreviewGUI(Rect r, GUIStyle background)
	{
		previewRenderUtil.BeginPreview(r, background);

		var previewCam = previewRenderUtil.m_Camera;
		previewCam.transform.position = preview.transform.position + new Vector3(0, 2.5f, 0);
		previewCam.transform.LookAt(preview.transform);
		previewCam.Render();

		previewRenderUtil.EndAndDrawPreview(r);
	}

	//	public override void OnInteractivePreviewGUI (Rect r, GUIStyle background)
	//	{
	//		previewRenderUtil.BeginPreview (r, background);
	//
	////		preview.SetActive (true);
	//		previewRenderUtil.m_Camera.Render ();
	////		preview.SetActive (false);
	//
	//		previewRenderUtil.EndAndDrawPreview (r);
	//	}

	public override void OnInspectorGUI()
	{
		CommonEditorUi.DrawSeparator(Color.cyan);
		DrawMinMaxValues(mapManager);
		CommonEditorUi.DrawSeparator(Color.cyan);
		DrawBasePrefabObject(mapManager);
		DrawCurrentMinMax(mapManager);
		DrawGenerateButton(mapManager);
		CommonEditorUi.DrawSeparator(Color.cyan);
		DrawMaterials(mapManager);
		CommonEditorUi.DrawSeparator(Color.cyan);
		DrawSaveLoadButton(mapManager);
		CommonEditorUi.DrawSeparator(Color.cyan);
		ShowTiles(mapManager);
		CommonEditorUi.DrawSeparator(Color.cyan);
		ShowZones(mapManager);

		if (GUI.changed)
			EditorUtility.SetDirty(target);
	}

	public void OnSceneGUI()
	{
		if (Application.isPlaying)
			return;

		MapManager mapManager = target as MapManager;

		Handles.BeginGUI();

		if (GUI.Button(new Rect(10, 10, 100, 30), Strings.Label_Tile_Mode))
		{
			mapManager.editMode = MapEditMode.tile_mode;
			mapManager.editMapObjType = MapObjType.floor;
		}
		if (GUI.Button(new Rect(10, 50, 100, 30), Strings.Label_Item_Mode))
		{
			mapManager.editMode = MapEditMode.item_mode;
			mapManager.editMapObjType = MapObjType.item;
		}
		if (GUI.Button(new Rect(10, 90, 100, 30), Strings.Label_Zone_Mode))
		{
			mapManager.editMode = MapEditMode.zone_mode;
			mapManager.editMapObjType = MapObjType.zone_portal;
		}

		GUI.color = Color.green;
		GUI.Label(new Rect(120, 10, 500, 30), Strings.Label_Edit_Mode + mapManager.editMode);
		GUI.Label(new Rect(120, 50, 500, 30), Strings.Label_MapObjType + mapManager.editMapObjType);
		GUI.color = Color.white;

		if (texture)
		{
			GUI.Box(new Rect(Screen.width - 200, Screen.height - 250, 150, 150), texture);
		}

		//		DrawZone (mapManager);

		Handles.EndGUI();

		int controlId = GUIUtility.GetControlID(FocusType.Passive);
		HandleUtility.AddDefaultControl(controlId);

		Event e = Event.current;
		if (e.isKey)
		{
			if (e.character == '3')
			{
				mapManager.editMode = MapEditMode.tile_mode;
				mapManager.editMapObjType = MapObjType.floor;
			}
			if (e.character == '4')
			{
				mapManager.editMode = MapEditMode.item_mode;
				mapManager.editMapObjType = MapObjType.item;
			}
			if (e.character == '5')
			{
				mapManager.editMode = MapEditMode.zone_mode;
				mapManager.editMapObjType = MapObjType.zone_portal;
			}

			int delta = 0;
			if (e.character == 'a' || e.character == 'A')
				delta = -1;
			if (e.character == 's' || e.character == 'S')
				delta = 1;

			if (delta != 0)
			{
				if (e.shift)
				{
					if (!((mapManager.editMapObjType <= 0 && delta < 0) || (mapManager.editMapObjType + 1 >= MapObjType.max_value && delta > 0)))
						mapManager.editMapObjType += delta;

				}
				else
				{
					switch (mapManager.editMode)
					{
						case MapEditMode.tile_mode:
							if ((mapManager.editTileStyle <= 0 && delta < 0) || (mapManager.editTileStyle + 1 >= TileStyle.max_value && delta > 0))
								break;
							mapManager.editTileStyle += delta;
							texture = mapManager.tileMaterials[(int)mapManager.editTileStyle].mainTexture;
							break;
						case MapEditMode.item_mode:
							if ((mapManager.editItemStyle <= 0 && delta < 0) || (mapManager.editItemStyle + 1 >= ItemStyle.max_value && delta > 0))
								break;
							mapManager.editItemStyle += delta;
							preview = mapManager.itemModels[(int)mapManager.editItemStyle];
							//						if (preview) {
							//							MapManager.DestroyGameObject (preview);
							//						}
							//						preview = Instantiate (mapManager.itemModels [(int)mapManager.editItemStyle]);
							//						preview.hideFlags = HideFlags.HideAndDontSave;
							//						preview.SetActive (false);
							//
							//						var flags = BindingFlags.Static | BindingFlags.NonPublic;
							//						var propInfo = typeof(Camera).GetProperty (Strings.Param_PreviewCullingLayer, flags);
							//						int previewLayer = (int)propInfo.GetValue (null, new object[0]);
							//
							//						preview.layer = previewLayer;
							//						foreach (Transform t in preview.transform) {
							//							t.gameObject.layer = previewLayer;
							//						}
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

		if (e.type == EventType.mouseDown || e.type == EventType.mouseDrag)
		{
			Vector2 mousePosition = e.mousePosition;
			Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);
			RaycastHit hit;

			if (Physics.Raycast(ray, out hit, 1000f))
			{
				TileInfo tile = hit.transform.GetComponent<TileInfo>();
				//				Debug.Log ("tile: " + tile);
				if (tile == null)
				{
					return;
				}

				if (e.button == 0)
				{ // left click button
					if (e.shift)
					{ // while shift key pressed
					  // change MapObjType
						switch (mapManager.editMode)
						{
							case MapEditMode.tile_mode:
								tile.UpdateType(mapManager.editMapObjType);
								break;
							case MapEditMode.item_mode:
								if (tile.item)
									tile.item.UpdateType(mapManager.editMapObjType);
								break;
							case MapEditMode.zone_mode:
								if (tile.IsZoneMember())
								{
									ZoneInfo zone = tile.transform.parent.GetComponent<ZoneInfo>();
									zone.UpdateType(mapManager.editMapObjType);
								}
								break;
							default:
								break;
						}

					}
					else
					{
						// add or change Style
						switch (mapManager.editMode)
						{
							case MapEditMode.tile_mode:
								tile.currentTileStyle = mapManager.editTileStyle;
								tile.UpdateMaterial(mapManager.tileMaterials);
								break;
							case MapEditMode.item_mode:
								mapManager.AddOrEditItem(tile);
								break;
							case MapEditMode.zone_mode:
								if (!tile.IsZoneMember())
								{
									mapManager.AddZone(tile.triggerCollider);
								}
								break;
							default:
								break;
						}
					}

				}
				else if (e.button == 1)
				{ // right click button
					if (e.shift)
					{ // while shift key pressed

					}
					else
					{
						// remove object
						switch (mapManager.editMode)
						{
							case MapEditMode.item_mode:
								mapManager.RemoveItem(tile);
								break;
							case MapEditMode.zone_mode:
								if (tile.IsZoneMember())
									mapManager.RemoveZone(tile.triggerCollider);
								break;
							default:
								break;
						}
					}
				}
			}
		}

	}


	public void ShowTiles(MapManager mapManger)
	{
		if (mapManger.tiles == null)
		{
			if (mapManger.LoadFromHierarchy() == false)
			{
				GUI.color = Color.red;
				GUILayout.TextField("=== no tiles ===");
				GUI.color = Color.white;
				return;
			}
		}

		isShowTileFold = EditorGUILayout.Foldout(isShowTileFold, Strings.Label_Tiles_List);
		if (isShowTileFold == false)
			return;

		int width = mapManger.tiles.GetLength(0);
		int height = mapManger.tiles.GetLength(1);
		EditorGUILayout.LabelField(Strings.Label_Tiles_Width + width);
		EditorGUILayout.LabelField(Strings.Label_Tiles_Height + height);

		for (int i = 0; i < width; ++i)
		{
			for (int j = 0; j < height; ++j)
			{
				string text = string.Format(Strings.Format_Tile_Index, i, j);
				EditorGUILayout.ObjectField(text, mapManger.tiles[i, j], typeof(GameObject), true);
			}
		}
	}

	void DrawMinMaxValues(MapManager mapManager)
	{
		//minWidth, minHeight
		GUI.color = (mapManager.minWidth > 0 && mapManager.minWidth < 100) ? Color.green : Color.red;
		int minWidth = EditorGUILayout.IntField(Strings.Param_Map_Min_Width, mapManager.minWidth);
		if (minWidth != mapManager.minWidth)
		{
			CommonEditorUi.RegisterUndo(Strings.Param_Map_Min_Width, mapManager);
			mapManager.minWidth = minWidth;
		}

		GUI.color = (mapManager.minHeight > 0 && mapManager.minHeight < 100) ? Color.green : Color.red;
		int minHeight = EditorGUILayout.IntField(Strings.Param_Map_Min_Height, mapManager.minHeight);
		if (minHeight != mapManager.minHeight)
		{
			CommonEditorUi.RegisterUndo(Strings.Param_Map_Min_Height, mapManager);
			mapManager.minHeight = minHeight;
		}

		//maxWidth, maxHeight
		GUI.color = (mapManager.maxWidth >= mapManager.minWidth && mapManager.maxWidth <= 100) ? Color.green : Color.red;
		int maxWidth = EditorGUILayout.IntField(Strings.Param_Map_Max_Width, mapManager.maxWidth);
		if (maxWidth != mapManager.maxWidth)
		{
			CommonEditorUi.RegisterUndo(Strings.Param_Map_Max_Width, mapManager);
			mapManager.maxWidth = maxWidth;
		}
		GUI.color = Color.white;

		GUI.color = (mapManager.maxHeight > 0 && mapManager.maxHeight <= 100) ? Color.green : Color.red;
		int maxHeight = EditorGUILayout.IntField(Strings.Param_Map_Max_Height, mapManager.maxHeight);
		if (maxHeight != mapManager.maxHeight)
		{
			CommonEditorUi.RegisterUndo(Strings.Param_Map_Max_Height, mapManager);
			mapManager.maxHeight = maxHeight;
		}
		GUI.color = Color.white;
	}

	void DrawCurrentMinMax(MapManager mapManger)
	{
		GUI.color = (mapManger.currentWidth >= mapManger.minWidth && mapManger.currentWidth <= mapManger.maxWidth) ? Color.green : Color.red;
		int width = EditorGUILayout.IntField(Strings.Param_Current_Map_Width, mapManger.currentWidth);
		if (width != mapManger.currentWidth)
		{
			CommonEditorUi.RegisterUndo(Strings.Param_Current_Map_Width, mapManger);
			mapManger.currentWidth = width;
		}
		GUI.color = Color.white;

		GUI.color = (mapManger.currentHeight >= mapManger.minHeight && mapManger.currentHeight <= mapManger.maxHeight) ? Color.green : Color.red;
		int height = EditorGUILayout.IntField(Strings.Param_Current_Map_Height, mapManger.currentHeight);
		if (height != mapManger.currentHeight)
		{
			CommonEditorUi.RegisterUndo(Strings.Param_Current_Map_Height, mapManger);
			mapManger.currentHeight = height;
		}
		GUI.color = Color.white;
	}

	void DrawBasePrefabObject(MapManager mapManager)
	{
		GUI.color = (mapManager.baseTilePrefab != null) ? Color.green : Color.red;
		GameObject baseTilePrefab = (GameObject)EditorGUILayout.ObjectField(Strings.Param_Map_Base_Tile, mapManager.baseTilePrefab, typeof(GameObject), false);

		if (baseTilePrefab != mapManager.baseTilePrefab)
		{
			CommonEditorUi.RegisterUndo(Strings.Param_Map_Base_Tile, mapManager);
			mapManager.baseTilePrefab = baseTilePrefab;
		}
		GUI.color = Color.white;

		GUI.color = (mapManager.baseItemPrefab != null) ? Color.green : Color.red;
		GameObject baseItemPrefab = (GameObject)EditorGUILayout.ObjectField(Strings.Param_Map_Base_Item, mapManager.baseItemPrefab, typeof(GameObject), false);

		if (baseItemPrefab != mapManager.baseItemPrefab)
		{
			CommonEditorUi.RegisterUndo(Strings.Param_Map_Base_Item, mapManager);
			mapManager.baseItemPrefab = baseItemPrefab;
		}
		GUI.color = Color.white;

		GUI.color = (mapManager.baseZonePrefab != null) ? Color.green : Color.red;
		GameObject baseZonePrefab = (GameObject)EditorGUILayout.ObjectField(Strings.Param_Map_Base_Zone, mapManager.baseZonePrefab, typeof(GameObject), false);

		if (baseZonePrefab != mapManager.baseZonePrefab)
		{
			CommonEditorUi.RegisterUndo(Strings.Param_Map_Base_Zone, mapManager);
			mapManager.baseZonePrefab = baseZonePrefab;
		}
		GUI.color = Color.white;

	}

	void DrawGenerateButton(MapManager mapManager)
	{
		EditorGUILayout.BeginHorizontal();

		if (GUILayout.Button(Strings.Param_Generate_Map_Data))
		{
			//			Debug.Log (Strings.Param_Generate_Map_Data);
			mapManager.CreateTiles();
		}

		if (GUILayout.Button(Strings.Param_Remove_Map_Data))
		{
			//			Debug.Log (Strings.Param_Remove_Map_Data);
			mapManager.RemoveAllTiles();
		}

		EditorGUILayout.EndHorizontal();
	}

	void DrawSaveLoadButton(MapManager mapManager)
	{
		string fileName = EditorGUILayout.TextField(Strings.Param_Map_File_Name, mapManager.fileName);
		if (fileName != mapManager.fileName)
		{
			CommonEditorUi.RegisterUndo(Strings.Param_Map_File_Name, mapManager);
			mapManager.fileName = fileName;
		}

		EditorGUILayout.BeginHorizontal();

		if (GUILayout.Button(Strings.Param_Save_Map_Data))
		{
			string title = Strings.Param_Save_Title;
			string msg = Strings.Param_Save_Msg;
			if (EditorUtility.DisplayDialog(title, msg, Strings.Param_yes, Strings.Param_no) == false)
				return;

			string dataPath = Application.dataPath;
			string fullPath = dataPath + Strings.Param_Save_Path + fileName + Strings.Param_Save_Extension;
			FileStream fs = new FileStream(fullPath, FileMode.Create);
			TextWriter textWriter = new StreamWriter(fs);
			int width = mapManager.currentWidth;
			int height = mapManager.currentHeight;
			textWriter.Write(Strings.Param_Save_width + width + Strings.Param_enter);
			textWriter.Write(Strings.Param_Save_height + height + Strings.Param_enter);

			//save tiles, items
			for (int i = 0; i < width; ++i)
			{
				for (int j = 0; j < height; ++j)
				{
					Transform tile_transform = mapManager.tiles[i, j].transform;
					textWriter.Write(Strings.GetThousandthsVector3ToString(tile_transform.localPosition) + char.ToString(Strings.Param_tab));
					textWriter.Write(Strings.GetThousandthsVector3ToString(tile_transform.eulerAngles) + char.ToString(Strings.Param_tab));
					textWriter.Write(tile_transform.tag + char.ToString(Strings.Param_tab));

					TileInfo tile = mapManager.tiles[i, j].GetComponent<TileInfo>();
					textWriter.Write((int)tile.type + char.ToString(Strings.Param_tab));
					textWriter.Write((int)tile.currentTileStyle + char.ToString(Strings.Param_tab));

					if (tile.item)
					{
						textWriter.Write(tile.item.name + char.ToString(Strings.Param_tab));
						textWriter.Write((int)tile.item.type + char.ToString(Strings.Param_tab));
						textWriter.Write((int)tile.item.currentItemStyle + char.ToString(Strings.Param_tab));

					}
					else
					{
						textWriter.Write(Strings.Param_null);
					}

					textWriter.Write(Strings.Param_enter);
				}
			}

			int colliders_Count;
			int zones_Count = mapManager.zones.Count;
			textWriter.Write(Strings.Param_Save_zones_Count + zones_Count + Strings.Param_enter);

			// save zones
			for (int k = 0; k < zones_Count; k++)
			{
				textWriter.Write((int)mapManager.zones[k].type + char.ToString(Strings.Param_tab));
				colliders_Count = mapManager.zones[k].colliders.Count;
				textWriter.Write(colliders_Count + char.ToString(Strings.Param_tab));
				for (int l = 0; l < colliders_Count; l++)
				{
					textWriter.Write(mapManager.zones[k].colliders[l].name + char.ToString(Strings.Param_tab));
				}
				textWriter.Write(Strings.Param_enter);
			}

			textWriter.Close();

			// backup
			string backupPath = dataPath + Strings.Param_Save_Path + fileName + Strings.Param__backup + Strings.Param_Save_Extension;
			if (File.Exists(backupPath) == false)
				FileUtil.CopyFileOrDirectory(fullPath, backupPath);

			AssetDatabase.Refresh();
		}
		if (GUILayout.Button(Strings.Param_Load_Map_Data))
		{
			mapManager.LoadMapDataFromFile();
		}

		EditorGUILayout.EndHorizontal();
	}

	void DrawMaterials(MapManager mapManager)
	{
		serializedObject.Update();
		EditorList.ShowSingle(serializedObject.FindProperty(Strings.Param_tileMaterials));
		EditorList.ShowSingle(serializedObject.FindProperty(Strings.Param_itemModels));
		serializedObject.ApplyModifiedProperties();
	}

	void DrawZone(MapManager mapManager)
	{
		//		Vector3[] points = new Vector3[4];
		Transform t;

		foreach (ZoneInfo z in mapManager.zones)
		{
			switch (z.type)
			{
				case MapObjType.zone_portal:
					Handles.color = Color.cyan;
					break;
				case MapObjType.zone_autosave:
					Handles.color = Color.magenta;
					break;
				case MapObjType.zone_event:
					Handles.color = Color.yellow;
					break;
				default:
					Handles.color = Random.ColorHSV();
					break;
			}

			for (int i = 0; i < z.colliders.Count; i++)
			{
				t = z.colliders[i].transform;
				//				points [0] = new Vector3 (t.position.x - 0.5f, t.position.y + 0.5f, t.position.z - 0.5f);
				//				points [1] = new Vector3 (t.position.x - 0.5f, t.position.y + 0.5f, t.position.z + 0.5f);
				//				points [2] = new Vector3 (t.position.x + 0.5f, t.position.y + 0.5f, t.position.z - 0.5f);
				//				points [3] = new Vector3 (t.position.x + 0.5f, t.position.y + 0.5f, t.position.z + 0.5f);
				//
				//				Handles.DrawSolidRectangleWithOutline (points, Handles.color, Color.blue);
			}

			Handles.color = Color.white;
		}
	}

	void ShowZones(MapManager mapManager)
	{
		ZoneInfo z;

		if (GUILayout.Button(Strings.Param_Remove_All_Zone_Data))
		{
			while (mapManager.zones.Count > 0)
			{
				z = mapManager.zones[0];
				mapManager.zones.Remove(z);
				z.RemoveAllZoneMembers();
				MapManager.DestroyGameObject(z.gameObject);
			}
		}

		if (mapManager.zones.Count == 0)
		{
			GUI.color = Color.red;
			GUILayout.TextField("=== no zones ===");
			GUI.color = Color.white;
			return;
		}

		isShowZoneFold = EditorGUILayout.Foldout(isShowZoneFold, Strings.Label_Zones_List);
		if (isShowZoneFold == false)
			return;

		string text;
		int locate_x, locate_z;
		int size = mapManager.zones.Count;
		EditorGUILayout.LabelField(Strings.Label_Zones_Size + size);

		for (int i = 0; i < size; i++)
		{
			z = mapManager.zones[i];
			EditorGUILayout.Foldout(true, z.name + Strings.Param__ + z.gameObject.tag);

			for (int j = 0; j < z.colliders.Count; j++)
			{
				locate_x = (int)z.colliders[j].transform.position.x;
				locate_z = (int)z.colliders[j].transform.position.z;
				text = string.Format(Strings.Format_Zone_Index, locate_x, locate_z);
				EditorGUILayout.ObjectField(text, z.colliders[j], typeof(Collider), true);
			}
		}
	}

	public static class EditorList
	{
		public static void ShowSingle(SerializedProperty list)
		{
			EditorGUILayout.PropertyField(list);
			EditorGUI.indentLevel += 1;
			if (list.isExpanded)
			{
				EditorGUILayout.PropertyField(list.FindPropertyRelative("Array.size"));
				for (int i = 0; i < list.arraySize; i++)
				{
					EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i));
				}
			}
			EditorGUI.indentLevel -= 1;
		}

	}

}
