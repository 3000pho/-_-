using System;

namespace AssemblyCSharp
{
	namespace Constant
	{
		public struct Strings
		{
			//game object's name
			public static readonly string equip_position = "equip_position";
			public static readonly string drop_position = "drop_position";
			public static readonly string _root = "_root";
			public static readonly string center = "center";

			//input
			public static readonly string Horizontal = "Horizontal";
			public static readonly string Vertical = "Vertical";
			public static readonly string Mouse_X = "Mouse X";
			public static readonly string Mouse_ScrollWheel = "Mouse ScrollWheel";

			//animator controller parameters
			public static readonly string state = "state";

			//tag
			public static readonly string Item = "Item";
		}

		namespace Enums{
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
		}
	}
}

