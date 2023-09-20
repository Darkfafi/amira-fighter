using RaCollection;
using UnityEngine;

namespace GameModes.Game
{
	public class EnemyAILeaderSystem : GameSystemBase<EnemyAILeaderSystem>
	{
		private RaCollection<EnemyAIController> _spawnedEnemies;

		protected override void OnInitialization()
		{
			base.OnInitialization();
			_spawnedEnemies = new RaCollection<EnemyAIController>(OnEnemyAdded, OnEnemyRemoved);
		}

		protected override void OnSetData()
		{
			CharacterActionsSystem.MainActionEvent.RegisterMethod<CharacterCoreSystem.SpawnCharacterAction>(OnSpawnedCharacter);
			CharacterActionsSystem.MainActionEvent.RegisterMethod<CharacterCoreSystem.DespawnCharacterAction>(OnDespawnedCharacter);
		}

		protected override void OnClearData()
		{

			CharacterActionsSystem.MainActionEvent.UnregisterMethod<CharacterCoreSystem.DespawnCharacterAction>(OnDespawnedCharacter);
			CharacterActionsSystem.MainActionEvent.UnregisterMethod<CharacterCoreSystem.SpawnCharacterAction>(OnSpawnedCharacter);
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
