namespace Screens.Game
{
	public class AI_AttackState : EnemyAIStateBase
	{
		protected override void OnInit()
		{
		}

		protected override void OnEnter()
		{
			Dependency.Character.CoreSystem.MainSkill(Dependency.Character)
				.Execute(Dependency.Character.CoreSystem.Processor);
			Dependency.GoToIdleState();
		}

		protected override void OnExit(bool isSwitch)
		{
		}

		protected override void OnDeinit()
		{
		}
	}
}