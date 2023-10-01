using RaCollection;
using UnityEngine;

namespace Screens.Game
{

	public class GameIntroSceneFSMState : GameEventFSMState, IDialogCharacterContainer
	{
		[Header("Spawning")]
		[SerializeField]
		private GameCharacterEntity _tricksterPrefab = null;
		[SerializeField]
		private GameCharacterEntity[] _enemyPrefabs = null;

		[SerializeField]
		private Transform _trickterSpawn = null;

		[SerializeField]
		private Transform[] _enemiesSpawns = null;

		private RaCollection<GameCharacterEntity> _enemies = new RaCollection<GameCharacterEntity>();

		public GameCharacterEntity TricksterInstance
		{
			get; private set;
		}

		public IReadOnlyRaCollection<GameCharacterEntity> Enemies => _enemies;

		protected override void OnEnter()
		{
			object setupLockFlag = new object();

			Dependency.GameSystems.CharacterActionsSystem.MainActionEvent.RegisterMethod<CharacterCoreSystem.DespawnCharacterAction>(OnUnitDespawned);

			if(Dependency.GameSystems.CharacterCoreSystem.SpawnCharacter(_tricksterPrefab, _trickterSpawn.position, lookDirection: -1f, lockFlag: setupLockFlag)
				.Execute(Dependency.GameSystems.ActionsProcessor, out var tricksterSpawnResult))
			{
				TricksterInstance = tricksterSpawnResult.CreatedCharacter;
				TricksterInstance.AddTag(nameof(GameIntroSceneFSMState));
				TricksterInstance.AddTag(nameof(TricksterInstance));
			}

			for(int i = 0; i < _enemiesSpawns.Length; i++)
			{
				GameCharacterEntity enemyPrefab = _enemyPrefabs[Random.Range(0, _enemyPrefabs.Length)];
				if (Dependency.GameSystems.CharacterCoreSystem.SpawnCharacter(enemyPrefab, _enemiesSpawns[i].position, lookDirection: 1f, lockFlag: setupLockFlag)
					.Execute(Dependency.GameSystems.ActionsProcessor, out var enemySpawnResult))
				{
					enemySpawnResult.CreatedCharacter.AddTag(nameof(GameIntroSceneFSMState));
					enemySpawnResult.CreatedCharacter.AddTag(nameof(Enemies));
					_enemies.Add(enemySpawnResult.CreatedCharacter);
				}
			}

			LockIntroElements(setupLockFlag);

			base.OnEnter();

			UnlockIntroElements(setupLockFlag);
		}

		private void OnUnitDespawned(CharacterCoreSystem.DespawnCharacterAction action)
		{
			if (action.Result.Success &&
				action.Parameters.CharacterToDespawn.HasTag(nameof(GameIntroSceneFSMState)))
			{
				if(action.Parameters.CharacterToDespawn.HasTag(nameof(Enemies)))
				{
					_enemies.Remove(action.Parameters.CharacterToDespawn);
				}
				else if(action.Parameters.CharacterToDespawn.HasTag(nameof(TricksterInstance)))
				{
					TricksterInstance = null;
				}

				if(TricksterInstance == null && _enemies.Count == 0)
				{
					GoToNextState();
				}
			}
		}

		protected override void OnExit(bool isSwitch)
		{
			Dependency.GameSystems.CharacterActionsSystem.MainActionEvent.UnregisterMethod<CharacterCoreSystem.DespawnCharacterAction>(OnUnitDespawned);

			if(TricksterInstance != null)
			{
				Dependency.GameSystems.CharacterCoreSystem.DespawnCharacter(TricksterInstance).Execute(Dependency.GameSystems.ActionsProcessor);
				TricksterInstance = null;
			}

			_enemies.ForEach(character =>
			{
				Dependency.GameSystems.CharacterCoreSystem.DespawnCharacter(character).Execute(Dependency.GameSystems.ActionsProcessor);
			});

			_enemies.Clear();
			base.OnExit(isSwitch);
		}

		protected override void OnDeinit()
		{
			_enemies.Dispose();
			_enemies = null;
			TricksterInstance = null;
			base.OnDeinit();
		}

		public void LockEnemyCharacters(object flag)
		{
			if (TricksterInstance != null)
			{
				TricksterInstance.CharacterLockedTracker.Register(flag);
			}

			Enemies.ForEachReadOnly(x => x.CharacterLockedTracker.Register(flag));
		}

		public void LockHUD(object flag)
		{
			Dependency.GameHUDView.HideFlags.Register(flag);
		}

		public void LockIntroElements(object flag, bool includeHUDHide = true)
		{
			if(Dependency.PlayerCharacter != null)
			{
				Dependency.PlayerCharacter.CharacterLockedTracker.Register(flag);
			}

			LockEnemyCharacters(flag);

			if (includeHUDHide)
			{
				LockHUD(flag);
			}
		}

		public void UnlockIntroElements(object flag)
		{
			Dependency.GameHUDView.HideFlags.Unregister(flag);

			Enemies.ForEachReadOnly(x => x.CharacterLockedTracker.Unregister(flag));

			if (TricksterInstance != null)
			{
				TricksterInstance.CharacterLockedTracker.Unregister(flag);
			}

			if (Dependency.PlayerCharacter != null)
			{
				Dependency.PlayerCharacter.CharacterLockedTracker.Unregister(flag);
			}
		}

		public GameCharacterEntity GetCharacter(int index)
		{
			int offset = 0;

			if(index == -1)
			{
				return Dependency.PlayerCharacter;
			}

			if(TricksterInstance != null)
			{
				offset = 1;

				if (index == 0)
				{
					return TricksterInstance;
				}
			}

			return Enemies[index - offset];
		}
	}
}