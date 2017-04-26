﻿using System;

namespace Constant
{
	public struct Strings
	{
		//game object's name
		public static readonly string GameObject_equip_position = "equip_position";
		public static readonly string GameObject_drop_position = "drop_position";
		public static readonly string GameObject__root = "_root";
		public static readonly string GameObject_center = "center";

		//input
		public static readonly string Input_Horizontal = "Horizontal";
		public static readonly string Input_Vertical = "Vertical";
		public static readonly string Input_Mouse_X = "Mouse X";
		public static readonly string Input_Mouse_ScrollWheel = "Mouse ScrollWheel";

		//parameters
		public static readonly string Param_state = "state";
		public static readonly string Param_Map_Min_Width = "Map Min Width";
		public static readonly string Param_Map_Min_Height = "Map Min Height";
		public static readonly string Param_Map_Max_Width = "Map Max Width";
		public static readonly string Param_Map_Max_Height = "Map Max Height";
		public static readonly string Param_Map_Base_Tile = "Map Base Tile";
		public static readonly string Param_Map_Base_Item = "Map Base Item";
		public static readonly string Param_Map_Base_Zone_Root = "Map Base Zone Root";
		public static readonly string Param_Map_Base_Zone_Member = "Map Base Zone Member";
		public static readonly string Param_Current_Map_Width = "Current Map Width";
		public static readonly string Param_Current_Map_Height = "Current Map Height";
		public static readonly string Param_Generate_Map_Data = "Generate Map Data";
		public static readonly string Param_Remove_Map_Data = "Remove Map Data";
		public static readonly string Param_Map_File_Name = "Map File Name";
		public static readonly string Param_Save_Map_Data = "Save Map Data";
		public static readonly string Param_Load_Map_Data = "Load Map Data";
		public static readonly string Param_Save_Title = "Save Map File";
		public static readonly string Param_Save_Msg = "Do you want to save map data?";
		public static readonly string Param_yes = "yes";
		public static readonly string Param_no = "no";
		public static readonly string Param_Save_Path = "/Resources/MapData/";
		public static readonly string Param_Save_Extension = ".txt";
		public static readonly string Param_Save_width = "width  ";
		public static readonly string Param_Save_height = "height  ";
//			public static readonly string Param_enter = "\n";
//			public static readonly string Param_tab = "\t";
		public static readonly string Param__ = "_";
		public static readonly string Param__backup = "_backup";
		public static readonly string Param_Load_Path = "MapData/";
		public static readonly string Param_tileMaterials = "tileMaterials";
		public static readonly string Param_itemMaterials = "itemMaterials";
		public static readonly char Param_tab = '\t';
		public static readonly char Param_enter = '\n';

		//label
		public static readonly string Label_Tiles_List = "Tiles List";
		public static readonly string Label_Tiles_Width = "Tiles Width : ";
		public static readonly string Label_Tiles_Height = "Tiles Height : ";
		public static readonly string Label_Brown_Stony = "Brown Stony";
		public static readonly string Label_Brown_Stony_Light = "Brown Stony Light";
		public static readonly string Label_Grass = "Grass";
		public static readonly string Label_Grey_Stones = "Grey Stones";
		public static readonly string Label_Sandy = "Sandy";
		public static readonly string Label_Sandy_Orange = "Sandy Orange";
		public static readonly string Label_Water_Deep_Blue = "Water Deep Blue";
		public static readonly string Label_Water_Light_Blue = "Water Light Blue";
		public static readonly string Label_Edit_Mode = "Edit Mode : ";
		public static readonly string Label_Tile_Mode = "Tile Mode";
		public static readonly string Label_Item_Mode = "Item Mode";
		public static readonly string Label_Zone_Mode = "Zone Mode";
		public static readonly string Label_MapObjType = "MapObjType : ";

		//tag
		public static readonly string Tag_Item = "Item";

		//format
		public static readonly string Format_Tile_Index = "Tile ( {0}, {1} )";
	}

	namespace Enums{
		//character's animation parameter state
		public enum CharState
		{
			idle = 0,
			walk,
			get_item,
			pick_up,
			change_item,
			walk_during_up_item,
			put_down,
		}

		public enum MapEditMode{
			tile_mode = 1,
			item_mode,
			zone_mode,
		}

		//map object functional type
		public enum MapObjType{
			//tile
			floor = 0,
			wall,
			half_wall,
			water,

			//item
			item,
			decoration,
			gettable_item,
			obstacle,

			//district
			zone,
			max_value,
		}

		public enum TileStyle{
			brown_stony=0,
			brown_stony_light,
			grass,
			grey_stones,
			sandy,
			sandy_orange,
			water_deep_blue,
			water_light_blue,
			max_value,
		}

		public enum ItemStyle{
			max_value,
		}
	}
}
