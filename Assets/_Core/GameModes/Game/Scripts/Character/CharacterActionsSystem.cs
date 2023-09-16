using RaActions;
using RaCastedEvents;
using RaDataHolder;
using UnityEngine;

namespace GameModes.Game
{
	public class CharacterActionsSystem : RaMonoDataHolderBase<RaActionsProcessor>
	{
		public static CharacterActionsSystem Instance
		{
			get
			{
				if(_instance == null)
				{
					_instance = FindObjectOfType<CharacterActionsSystem>();
					if (_instance != null)
					{
						_instance.SetData(new RaActionsProcessor());
					}
				}
				return _instance;
			}
		}

		public static bool IsAvailable => Instance != null;

		public static RaCastedEvent MainActionEvent => Instance._mainActionEvent;
		public static RaActionsProcessor Processor => Instance.Data;

		private static CharacterActionsSystem _instance = null;

		private RaCastedEvent _mainActionEvent = new RaCastedEvent();

		protected override void OnSetData()
		{
			Data.ExecutedMainActionEvent += OnExecutedMainActionEvent;
		}

		protected override void OnClearData()
		{
			Data.Dispose();
		}

		private void OnExecutedMainActionEvent(RaAction action)
		{
			_mainActionEvent.Invoke(action);
		}
	}
}