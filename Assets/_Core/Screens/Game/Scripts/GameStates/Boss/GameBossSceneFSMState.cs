using UnityEngine;

namespace Screens.Game
{
	public class GameBossSceneFSMState : GameEventFSMState, IDialogCharacterContainer
	{
		[SerializeField]
		private GameCharacterEntity _bossPrefab  = null;

		[SerializeField]
		private ParticleSystem _hideShowFXPrefab = null;

		[SerializeField]
		private Transform _bossSpawnPoint = null;

		public GameCharacterEntity BossInstance
		{
			get; private set;
		}

		public GameCharacterEntity PlayerInstance => Dependency.PlayerCharacter;

		private readonly object _hideLock = new object();

		protected override void OnInit()
		{
			base.OnInit();
		}

		protected override void OnEnter()
		{
			Dependency.GameSystems.CharacterActionsSystem.MainActionEvent.RegisterMethod<CharacterCoreSystem.DespawnCharacterAction>(OnUnitDespawned);

			object setupLockFlag = new object();

			if (Dependency.GameSystems.CharacterCoreSystem.SpawnCharacter(_bossPrefab, _bossSpawnPoint.position, lookDirection: -1f, lockFlag: setupLockFlag)
				.Execute(Dependency.GameSystems.ActionsProcessor, out var tricksterSpawnResult))
			{
				BossInstance = tricksterSpawnResult.CreatedCharacter;
				BossInstance.AddTag(nameof(GameBossSceneFSMState));
				BossInstance.AddTag(nameof(BossInstance));
			}

			LockStateElements(setupLockFlag);
			
			base.OnEnter();

			UnlockStateElements(setupLockFlag);
		}

		public void LockStateElements(object flag)
		{
			LockBoss(flag);
			LockPlayer(flag);
		}

		public void UnlockStateElements(object flag)
		{
			UnlockPlayer(flag);
			UnlockBoss(flag);
		}

		public void HealPlayer()
		{
			if(PlayerInstance != null)
			{
				PlayerInstance.Health.Heal(9999);
			}
		}

		public void LockBoss(object flag)
		{
			if(BossInstance != null)
			{
				BossInstance.CharacterLockedTracker.Register(flag);
			}
		}

		public void UnlockBoss(object flag)
		{
			if(BossInstance != null)
			{
				BossInstance.CharacterLockedTracker.Unregister(flag);
			}
		}

		public void LockPlayer(object flag)
		{
			if (Dependency.PlayerCharacter != null)
			{
				Dependency.PlayerCharacter.CharacterLockedTracker.Register(flag);
			}
		}

		public void UnlockPlayer(object flag)
		{
			if (Dependency.PlayerCharacter != null)
			{
				Dependency.PlayerCharacter.CharacterLockedTracker.Unregister(flag);
			}
		}

		public void ShowBoss()
		{
			if (BossInstance != null)
			{
				BossInstance.gameObject.SetActive(true);
			}
			UnlockBoss(_hideLock);
		}

		public void HideBoss()
		{
			if(BossInstance != null)
			{
				BossInstance.gameObject.SetActive(false);
			}
			LockBoss(_hideLock);
		}

		public void ShowBossWithEffects()
		{
			if(BossInstance != null && !BossInstance.gameObject.activeSelf)
			{
				ShowBoss();
				Instantiate(_hideShowFXPrefab, BossInstance.transform.position + Vector3.up * 0.5f, Quaternion.identity);
			}
		}

		public void HideBossWithEffects()
		{
			if(BossInstance != null && BossInstance.gameObject.activeSelf)
			{
				HideBoss();
				Instantiate(_hideShowFXPrefab, BossInstance.transform.position + Vector3.up * 0.5f, Quaternion.identity);
			}
		}

		public void ReturnBossToSpawn()
		{
			if(BossInstance != null)
			{
				object resetFlag = new object();
				BossInstance.CharacterLockedTracker.Register(resetFlag);
				{
					BossInstance.transform.position = _bossSpawnPoint.position;
					BossInstance.SetLookDirection(-1f);
				}
				BossInstance.CharacterLockedTracker.Unregister(resetFlag);
			}
		}

		private void OnUnitDespawned(CharacterCoreSystem.DespawnCharacterAction action)
		{
			if (action.Result.Success &&
				action.Parameters.CharacterToDespawn.HasTag(nameof(GameBossSceneFSMState)))
			{
				if (action.Parameters.CharacterToDespawn.HasTag(nameof(BossInstance)))
				{
					BossInstance = null;
				}

				if (BossInstance == null)
				{
					GoToNextState();
				}
			}
		}

		protected override void OnExit(bool isSwitch)
		{			
			Dependency.GameSystems.CharacterActionsSystem.MainActionEvent.UnregisterMethod<CharacterCoreSystem.DespawnCharacterAction>(OnUnitDespawned);

			if (BossInstance != null)
			{
				Dependency.GameSystems.CharacterCoreSystem.DespawnCharacter(BossInstance).Execute(Dependency.GameSystems.ActionsProcessor);
				BossInstance = null;
			}

			base.OnExit(isSwitch); 
		}

		protected override void OnLastStateExitEvent()
		{

		}

		public GameCharacterEntity GetCharacter(int index)
		{
			if(index < 0)
			{
				return Dependency.PlayerCharacter;
			}

			if(index == 0)
			{
				return BossInstance;
			}

			return null;
		}
	}
}