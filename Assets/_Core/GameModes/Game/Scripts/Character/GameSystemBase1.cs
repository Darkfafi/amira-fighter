using RaDataHolder;

namespace GameModes.Game
{
	public abstract class GameSystemBase<TSystem, TData> : RaMonoDataHolderBase<TData>
		where TSystem : GameSystemBase<TSystem, TData>
	{
		public static TSystem Instance
		{
			get
			{
				TryInit();
				return _instance;
			}
		}

		public static bool IsAvailable => Instance != null;

		private static TSystem _instance = null;

		protected override void OnInitialization()
		{
			base.OnInitialization();
			TryInit();
		}

		protected abstract void InjectData();

		private static void TryInit()
		{
			if(_instance == null)
			{
				_instance = FindObjectOfType<TSystem>();

				if(_instance != null)
				{
					_instance.InjectData();
				}
			}
		}
	}
}