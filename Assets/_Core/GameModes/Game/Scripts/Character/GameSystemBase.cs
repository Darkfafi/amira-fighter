namespace GameModes.Game
{
	public abstract class GameSystemBase<TSystem> : GameSystemBase<TSystem, GameSystemBase<TSystem>.VoidData>
		where TSystem : GameSystemBase<TSystem>
	{
		protected override void InjectData()
		{
			SetData(default);
		}

		public struct VoidData
		{
		
		}
	}
}