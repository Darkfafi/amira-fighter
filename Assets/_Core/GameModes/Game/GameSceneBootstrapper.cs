namespace GameModes.Game
{
	public class GameSceneBootstrapper : SceneBootstrapperBase<GameSceneModel>
	{
		public GameCharacter PlayerCharacter
		{
			get; private set;
		}

		protected override void OnSetData()
		{
			PlayerCharacter = Instantiate(Data.PlayerCharacterPrefab);
		}

		protected override void OnClearData()
		{
			if(PlayerCharacter != null)
			{
				Destroy(PlayerCharacter.gameObject);
			}
		}
	}
}