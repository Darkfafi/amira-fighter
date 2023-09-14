using RaModelsSO;
using UnityEngine;
namespace GameModes.Game
{
	public class GameSceneModel : RaModelSOBase
	{
		[SerializeField]
		private GameCharacter _defaultCharacterPrefab = null;

		public GameCharacter PlayerCharacterPrefab
		{
			get; private set;
		}

		protected override void OnInit()
		{
			if(PlayerCharacterPrefab == null)
			{
				PlayerCharacterPrefab = _defaultCharacterPrefab;
			}
		}

		protected override void OnDeinit()
		{
			PlayerCharacterPrefab = null;
		}
	}
}