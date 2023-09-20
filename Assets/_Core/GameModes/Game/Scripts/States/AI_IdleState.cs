namespace GameModes.Game
{
	public class AI_IdleState : EnemyAIStateBase
	{
		protected override void OnInit()
		{
		}

		protected override void OnEnter()
		{
			CharacterCoreSystem.Instance.SetDirectionFlag(Dependency.Character, Tools.Direction.None, CharacterCoreSystem.SetDirectionFlagAction.WriteType.Override)
				.Execute(CharacterActionsSystem.Processor);
		}

		protected override void OnExit(bool isSwitch)
		{
		}

		protected override void OnDeinit()
		{
		}
	}
}