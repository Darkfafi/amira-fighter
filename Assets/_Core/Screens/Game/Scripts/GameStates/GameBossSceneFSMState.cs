﻿using Assets.HeroEditor.Common.Scripts.Common;
using UnityEngine;

namespace Screens.Game
{
	public class GameBossSceneFSMState : GameEventFSMState, IDialogCharacterContainer
	{
		[SerializeField]
		private GameCharacterEntity _bossPrefab  = null;

		[field: SerializeField]
		public CharacterHUDView BossCharacterHUDView
		{
			get; private set;
		}

		[SerializeField]
		private Transform _bossSpawnPoint = null;

		public GameCharacterEntity BossInstance
		{
			get; private set;
		}

		protected override void OnInit()
		{
			base.OnInit();
			BossCharacterHUDView.SetActive(false);
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

				BossCharacterHUDView.SetData(BossInstance);
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
		}

		public void HideBoss()
		{
			if(BossInstance != null)
			{
				BossInstance.gameObject.SetActive(false);
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