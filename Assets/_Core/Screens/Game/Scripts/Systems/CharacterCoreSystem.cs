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

		#region Attacks

		public MainAttackAction MainAttack(GameCharacterEntity entity)
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

		#region Movement

		public SetDirectionFlagAction SetDirectionFlag(GameCharacterEntity entity, Direction direction, SetDirectionFlagAction.WriteType writeType)
		{
			return new SetDirectionFlagAction((parameters) =>
			{
				// Logics
				switch(writeType)
				{
					case SetDirectionFlagAction.WriteType.Add:
						parameters.Character.CurrentDirections |= parameters.Direction;
						break;
					case SetDirectionFlagAction.WriteType.Remove:
						parameters.Character.CurrentDirections &= ~parameters.Direction;
						break;
					case SetDirectionFlagAction.WriteType.Override:
						parameters.Character.CurrentDirections = parameters.Direction;
						break;
				}

				parameters.Character.CurrentDirVector = parameters.Character.CurrentDirections.ToVector(normalized: true);

				// Visuals
				if(parameters.Character.CurrentDirections != Direction.None)
				{
					CharacterState movementState = parameters.Character.Speed >= parameters.Character.RunSpeedThreshold ? CharacterState.Run : CharacterState.Walk;

					parameters.Character.CharacterView.SetState(movementState);
					parameters.Character.CharacterView.transform.localScale = new Vector3(Mathf.Sign(parameters.Character.CurrentDirVector.x), 1f, 1f);
				}
				else
				{
					parameters.Character.CharacterView.SetState(CharacterState.Idle);
				}

				return new SetDirectionFlagAction.ActionResult()
				{
					Success = true
				};
			}, new SetDirectionFlagAction.ActionParams()
			{
				Character = entity,
				Direction = direction,
				WriteType = writeType,
			});
		}

		public class SetDirectionFlagAction : RaAction<SetDirectionFlagAction.ActionParams, SetDirectionFlagAction.ActionResult>
		{
			public SetDirectionFlagAction(MainHandler executeMethod, ActionParams parameters)
				: base(executeMethod, parameters)
			{
			}

			public struct ActionParams
			{
				public GameCharacterEntity Character;
				public Direction Direction;
				public WriteType WriteType;
			}

			public struct ActionResult : IRaActionResult
			{
				public bool Success
				{
					get; set;
				}
			}

			public enum WriteType
			{
				Override = 0,
				Add = 1,
				Remove = 2
			}
		}

		#endregion

		public SpawnCharacterAction SpawnCharacter(GameCharacterEntity characterPrefab, Vector3 position)
		{
			return new SpawnCharacterAction((parameters) => 
			{
				if (parameters.CharacterPrefab)
				{
					GameCharacterEntity instance = Instantiate(parameters.CharacterPrefab, parameters.Position, Quaternion.identity);
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