using Assets.HeroEditor.Common.Scripts.CharacterScripts;
using RaActions;
using UnityEngine;
using static Screens.Game.Tools;

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

		public MainAttackAction MainSkill(GameCharacterEntity entity)
		{
			return new MainAttackAction((parameters) =>
			{
				Vector3 centerOfCrowd = Vector3.zero;
				RaycastHit2D[] hits = Physics2D.CircleCastAll(parameters.Character.transform.position, parameters.Character.AttackRadius, Vector2.zero);
				for(int i = 0; i < hits.Length; i++)
				{
					RaycastHit2D hit = hits[i];
					if(hit.collider.TryGetComponent(out GameCharacterEntity otherEntity))
					{
						centerOfCrowd += otherEntity.transform.position;
						if(otherEntity.tag != parameters.Character.tag)
						{
							otherEntity.Health.Damage(1);
						}
					}
				}

				entity.CharacterView.Slash();
				return new MainAttackAction.ActionResult()
				{
					Success = true
				};
			}, new MainAttackAction.ActionParams()
			{
				Character = entity
			});
		}

		public class MainAttackAction : RaAction<MainAttackAction.ActionParams, MainAttackAction.ActionResult>
		{
			public MainAttackAction(MainHandler executeMethod, ActionParams parameters)
				: base(executeMethod, parameters)
			{
			}

			public struct ActionParams
			{
				public GameCharacterEntity Character;
			}

			public struct ActionResult : IRaActionResult
			{
				public bool Success
				{
					get; set;
				}

			}
		}

		#endregion

		public SpawnCharacterAction SpawnCharacter(GameCharacterEntity characterPrefab, Vector3 position, float lookDirection = 0f)
		{
			return new SpawnCharacterAction((parameters) => 
			{
				if (parameters.CharacterPrefab)
				{
					GameCharacterEntity instance = Instantiate(parameters.CharacterPrefab, parameters.Position, Quaternion.identity);
					instance.SetLookDirection(parameters.LookDirection);
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