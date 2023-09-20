namespace GameModes.Game
{
	public class AI_AttackState : EnemyAIStateBase
	{
		protected override void OnInit()
		{
		}

		protected override void OnEnter()
		{
			CharacterCoreSystem.Instance.MainAttack(Dependency.Character)
				.Execute(CharacterActionsSystem.Processor);
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