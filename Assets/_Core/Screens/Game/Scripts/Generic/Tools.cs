using UnityEngine;

namespace Screens.Game
{
	public static class Tools
	{
		[System.Flags]
		public enum Direction
		{
			None	= 0,
			Up		= 1 << 0,
			Down	= 1 << 1,
			Left	= 1 << 2,
			Right	= 1 << 3,
			All		= Up | Down | Left | Right,
		}

		public static Vector2 ToVector(this Direction directions, bool normalized)
		{
			Vector2 vec = Vector2.zero;
			if(directions.HasFlag(Direction.Up))
			{
				vec += Vector2.up;
			}

			if(directions.HasFlag(Direction.Down))
			{
				vec += Vector2.down;
			}

			if(directions.HasFlag(Direction.Left))
			{
				vec += Vector2.left;
			}

			if(directions.HasFlag(Direction.Right))
			{
				vec += Vector2.right;
			}

			if(normalized)
			{
				vec = vec.normalized;
			}

			return vec;
		}
	}
}