using RaActions;
using RaCastedEvents;

namespace GameModes.Game
{
	public class CharacterActionsSystem : GameSystemBase<CharacterActionsSystem, RaActionsProcessor>
	{
		public static RaCastedEvent MainActionEvent => Instance._mainActionEvent;
		public static RaActionsProcessor Processor => Instance.Data;

		private RaCastedEvent _mainActionEvent = new RaCastedEvent();

		protected override void InjectData()
		{
			SetData(new RaActionsProcessor());
		}

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