namespace GameModes.Game
{
	public class AI_IdleState : EnemyAIStateBase
	{
		protected override void OnInit()
		{
		}

		protected override void OnEnter()
		{
			Dependency.Character.SetDirectionFlag(Tools.Direction.None, GameCharacterEntity.SetDirectionFlagAction.WriteType.Override).Execute(CharacterActionsSystem.Processor);
		}

		protected override void OnExit(bool isSwitch)
		{
		}

		protected override void OnDeinit()
		{
		}
	}
}