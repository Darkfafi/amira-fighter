namespace GameModes.Game
{
	public class AI_IdleState : EnemyAIStateBase
	{
		protected override void OnInit()
		{
		}

		protected override void OnEnter()
		{
			Dependency.Character.CoreSystem.SetDirectionFlag(Dependency.Character, Tools.Direction.None, CharacterCoreSystem.SetDirectionFlagAction.WriteType.Override)
				.Execute(Dependency.Character.CoreSystem.Processor);
		}

		protected override void OnExit(bool isSwitch)
		{
		}

		protected override void OnDeinit()
		{
		}
	}
}