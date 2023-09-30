using RaDataHolder;
using UnityEngine;

namespace Screens.Game
{
	public class GameHUDView : RaMonoDataHolderBase<GameSceneBootstrapper>
	{
		[field: SerializeField]
		public CharacterHUDView CharacterHUDView
		{
			get; private set;
		}

		[field: SerializeField]
		public GameProgressHUDView GameProgressHUDView
		{
			get; private set;
		}

		protected override void OnSetData()
		{
			CharacterHUDView.SetData(Data.PlayerCharacter);
			GameProgressHUDView.SetData(new GameProgressHUDView.CoreData()
			{
				GameCharacter = Data.PlayerCharacter,
				StoryProgress = Data.StoryProgress
			});
		}

		protected override void OnClearData()
		{
			GameProgressHUDView.ClearData();
			CharacterHUDView.ClearData();
		}
	}
}