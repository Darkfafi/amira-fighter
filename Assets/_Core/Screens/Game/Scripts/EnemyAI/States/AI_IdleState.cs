namespace Screens.Game
{
	public class AI_IdleState : EnemyAIStateBase
	{
		protected override void OnInit()
		{
		}

		protected override void OnEnter()
		{
			Dependency.Character.MovementController.Destination = null;
		}

		protected override void OnExit(bool isSwitch)
		{
		}

		protected override void OnDeinit()
		{
		}
	}
}