using UnityEngine;


namespace GameModes.Game
{
	public class GameLevel : MonoBehaviour
	{
		[field: SerializeField]
		public Transform PlayerSpawn
		{
			get; private set;
		}
	}
}