using UnityEngine;

namespace Screens.Game
{
	public class GameEventFSMState : GameFSMStateBase
	{
		[field: SerializeField]
		public Sprite EventIcon
		{
			get; private set;
		}

		protected override void OnInit()
		{
			base.OnInit();
			GoFSMStateEvents.LastStateExitEvent.AddListener(OnLastStateExitEvent);
		}

		protected override void OnDeinit()
		{
			GoFSMStateEvents.LastStateExitEvent.RemoveListener(OnLastStateExitEvent);
			base.OnDeinit();
		}

		protected virtual void OnLastStateExitEvent()
		{
			FSM_GoToNextState();
		}
	}
}