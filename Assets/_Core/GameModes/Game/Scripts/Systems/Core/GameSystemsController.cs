using RaActions;
using RaBehaviourSO;

namespace GameModes.Game
{
	public sealed class GameSystemsController : RaBehaviourSOController<GameSystemBase>
	{
		public CharacterActionsSystem CharacterActionsSystem
		{
			get; private set;
		}

		public CharacterCoreSystem CharacterCoreSystem
		{
			get; private set;
		}

		public EnemyAILeaderSystem EnemyAILeaderSystem
		{
			get; private set;
		}

		public RaActionsProcessor ActionsProcessor => CharacterActionsSystem.Processor;

		public GameSystemsController(GameSystemBase[] behaviours, BehaviourHandler onInit = null, BehaviourHandler onDeinit = null) 
			: base(behaviours, onInit, onDeinit)
		{
			if(TryGetBehaviour(out CharacterActionsSystem actionsSystem)) CharacterActionsSystem = actionsSystem;
			if(TryGetBehaviour(out CharacterCoreSystem coreSystem)) CharacterCoreSystem = coreSystem;
			if(TryGetBehaviour(out EnemyAILeaderSystem enemyAILeaderSystem)) EnemyAILeaderSystem = enemyAILeaderSystem;
		}
	}
}