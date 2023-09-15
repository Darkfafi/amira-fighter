using UnityEngine;
using RaActions;

namespace GameModes.Game
{
	public class CharacterActionsSystem : MonoBehaviour
	{
		public static CharacterActionsSystem Instance
		{
			get
			{
				if(_instance == null)
				{
					_instance = new GameObject("<CharacterController>").AddComponent<CharacterActionsSystem>();
				}
				return _instance;
			}
		}

		public static RaActionsProcessor Processor => Instance._processor;

		private static CharacterActionsSystem _instance = null;

		private RaActionsProcessor _processor = new RaActionsProcessor();
	}
}