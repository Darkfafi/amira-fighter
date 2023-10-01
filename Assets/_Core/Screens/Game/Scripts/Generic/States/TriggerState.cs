using RaFSM;
using UnityEngine;

namespace Screens.Game
{
	public class TriggerState : RaGOStateBase
	{
		public RaGOStateEvent TriggeredEvent;

		protected override void OnDeinit()
		{

		}

		protected override void OnEnter()
		{

		}

		protected void OnTriggerEnter2D(Collider2D collision)
		{
			if (IsCurrentState)
			{
				TriggeredEvent?.Invoke(this);
			}
		}

		protected override void OnExit(bool isSwitch)
		{

		}

		protected override void OnInit()
		{

		}
	}
}