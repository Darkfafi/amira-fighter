using RaActions;
using RaCastedEvents;

namespace GameModes.Game
{
	public class CharacterActionsSystem : GameSystemBase
	{
		public RaCastedEvent MainActionEvent
		{
			get; private set;
		}

		public RaActionsProcessor Processor
		{
			get; private set;
		}

		protected override void OnSetup()
		{
			MainActionEvent = new RaCastedEvent();
			Processor = new RaActionsProcessor();
			Processor.ExecutedMainActionEvent += OnExecutedMainActionEvent;
		}

		protected override void OnStart()
		{

		}

		protected override void OnEnd()
		{
			Processor.Dispose();
			MainActionEvent.Dispose();
		}

		private void OnExecutedMainActionEvent(RaAction action)
		{
			MainActionEvent.Invoke(action);
		}
	}
}