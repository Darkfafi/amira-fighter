using RaCollection;
using UnityEngine;

namespace GameModes.Game
{
	public class EnemyAILeaderSystem : GameSystemBase
	{
		private RaCollection<EnemyAIController> _spawnedEnemies;

		private CharacterActionsSystem _characterActionsSystem = null;

		protected override void OnSetup()
		{
			_spawnedEnemies = new RaCollection<EnemyAIController>(OnEnemyAdded, OnEnemyRemoved);

			_characterActionsSystem = GetDependency<CharacterActionsSystem>();

			_characterActionsSystem.MainActionEvent.RegisterMethod<CharacterCoreSystem.SpawnCharacterAction>(OnSpawnedCharacter);
			_characterActionsSystem.MainActionEvent.RegisterMethod<CharacterCoreSystem.DespawnCharacterAction>(OnDespawnedCharacter);
		}

		protected override void OnStart()
		{

		}

		protected override void OnEnd()
		{
			_spawnedEnemies.Dispose();

			_characterActionsSystem.MainActionEvent.UnregisterMethod<CharacterCoreSystem.DespawnCharacterAction>(OnDespawnedCharacter);
			_characterActionsSystem.MainActionEvent.UnregisterMethod<CharacterCoreSystem.SpawnCharacterAction>(OnSpawnedCharacter);
		}

		private void OnEnemyAdded(EnemyAIController item, int index)
		{
			Debug.Log("Spawned: " + item);
		}

		private void OnEnemyRemoved(EnemyAIController item, int index)
		{
			Debug.Log("Despawned: " + item);
		}

		private void OnSpawnedCharacter(CharacterCoreSystem.SpawnCharacterAction spawnAction)
		{
			if(spawnAction.Result.Success && spawnAction.Result.CreatedCharacter.TryGetComponent(out EnemyAIController enemyAI))
			{
				_spawnedEnemies.Add(enemyAI);
			}
		}

		private void OnDespawnedCharacter(CharacterCoreSystem.DespawnCharacterAction despawnedAction)
		{
			if(despawnedAction.Result.Success && despawnedAction.Parameters.CharacterToDespawn.TryGetComponent(out EnemyAIController enemyAI))
			{
				_spawnedEnemies.Remove(enemyAI);
			}
		}
	}
}
