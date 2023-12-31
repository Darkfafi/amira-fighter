﻿using RaActions;
using RaProgression;
using UnityEngine;

namespace Screens.Game
{
	public class CharacterCoreSystem : GameSystemBase
	{
		public RaActionsProcessor Processor => GetDependency<CharacterActionsSystem>().Processor;

		protected override void OnSetup()
		{

		}

		protected override void OnStart()
		{

		}

		protected override void OnEnd()
		{

		}

		#region Skills

		public UseSkillAction UseSkill(CharacterSkillBase characterSkill)
		{
			return new UseSkillAction((parameters) =>
			{
				bool success = parameters.CharacterSkill.Use();

				return new UseSkillAction.ActionResult()
				{
					SkillProgress = parameters.CharacterSkill.SkillUseProgress,
					Success = success
				};
			}, new UseSkillAction.ActionParams()
			{
				CharacterSkill = characterSkill
			});
		}

		public class UseSkillAction : RaAction<UseSkillAction.ActionParams, UseSkillAction.ActionResult>
		{
			public UseSkillAction(MainHandler executeMethod, ActionParams parameters)
				: base(executeMethod, parameters)
			{
			}

			public struct ActionParams
			{
				public CharacterSkillBase CharacterSkill;
			}

			public struct ActionResult : IRaActionResult
			{
				public RaProgress SkillProgress
				{
					get; set;
				}

				public bool Success
				{
					get; set;
				}

			}
		}

		#endregion

		public SpawnCharacterAction SpawnCharacter(GameCharacterEntity characterPrefab, Vector3 position, float lookDirection = 0f, object lockFlag = null)
		{
			return new SpawnCharacterAction((parameters) => 
			{
				if (parameters.CharacterPrefab)
				{
					GameCharacterEntity instance = Instantiate(parameters.CharacterPrefab, parameters.Position, Quaternion.identity);
					instance.SetLookDirection(parameters.LookDirection);
					
					if (parameters.LockFlag != null)
					{
						instance.CharacterLockedTracker.Register(parameters.LockFlag);
					}

					return new SpawnCharacterAction.ActionResult()
					{
						CreatedCharacter = instance,
						Success = instance != null,
					};
				}
				return default;
			}, new SpawnCharacterAction.ActionParams()
			{
				CharacterPrefab = characterPrefab,
				LookDirection = lookDirection,
				LockFlag = lockFlag,
				Position = position,
			});
		}

		public class SpawnCharacterAction : RaAction<SpawnCharacterAction.ActionParams, SpawnCharacterAction.ActionResult>
		{
			public SpawnCharacterAction(MainHandler executeMethod, ActionParams parameters)
				: base(executeMethod, parameters)
			{
			}

			public struct ActionParams
			{
				public GameCharacterEntity CharacterPrefab;
				public Vector3 Position;
				public float LookDirection;
				public object LockFlag;
			}

			public struct ActionResult : IRaActionResult
			{
				public GameCharacterEntity CreatedCharacter;

				public bool Success
				{
					get; set;
				}
			}
		}

		public DespawnCharacterAction DespawnCharacter(GameCharacterEntity characterToDespawn)
		{
			return new DespawnCharacterAction((parameters) =>
			{
				if (parameters.CharacterToDespawn)
				{
					Destroy(parameters.CharacterToDespawn.gameObject);
					return new DespawnCharacterAction.ActionResult()
					{
						Success = true,
					};
				}
				return default;
			}, new DespawnCharacterAction.ActionParams()
			{
				CharacterToDespawn = characterToDespawn,
			});
		}

		public class DespawnCharacterAction : RaAction<DespawnCharacterAction.ActionParams, DespawnCharacterAction.ActionResult>
		{
			public DespawnCharacterAction(MainHandler executeMethod, ActionParams parameters)
				: base(executeMethod, parameters)
			{
			}

			public struct ActionParams
			{
				public GameCharacterEntity CharacterToDespawn;
			}

			public struct ActionResult : IRaActionResult
			{
				public bool Success
				{
					get; set;
				}
			}
		}
	}
}