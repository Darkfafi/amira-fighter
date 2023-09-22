using Assets.HeroEditor.Common.Scripts.CharacterScripts;
using RaActions;
using RaCollection;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static GameModes.Game.Tools;

namespace GameModes.Game
{
	public class GameCharacterEntity : MonoBehaviour, IRaCollectionElement
	{
		public string Id => GetInstanceID().ToString();

		[SerializeField]
		private List<string> _tags = new List<string>();

		[SerializeField]
		private NavMeshAgent _agent = null;

		[SerializeField]
		private Vector2 _speedRange = Vector2.one;

		[SerializeField]
		private int _hp = 5;

		[SerializeField]
		private float _attackRadius = 1f;

		[SerializeField]
		private float _runSpeedThreshold = 4f;

		public float AttackRadius => _attackRadius;

		[field: SerializeField]
		public Character CharacterView
		{
			get; private set;
		}

		[field: Header("ReadOnly")]
		[field: SerializeField]
		public Direction CurrentDirections
		{
			get; private set;
		}

		[field: SerializeField]
		public Vector2 CurrentDirVector
		{
			get; private set;
		}

		[field: SerializeField]
		public float Speed
		{
			get; private set;
		}

		public Health Health
		{
			get; private set;
		}

		protected void OnValidate()
		{
			if(CharacterView == null)
			{
				CharacterView = GetComponentInChildren<Character>();
			}
		}

		protected void Awake()
		{
			_agent.updateRotation = false;
			_agent.updateUpAxis = false;

			_agent.speed = Speed = Random.Range(_speedRange.x, _speedRange.y);

			Health = new Health(_hp);
			SetDirectionFlag(Direction.None, SetDirectionFlagAction.WriteType.Override).Execute(CharacterActionsSystem.Processor);
		}

		protected void Update()
		{
			if(CurrentDirections != Direction.None)
			{
				_agent.isStopped = false;
				_agent.SetDestination(transform.position + new Vector3(CurrentDirVector.x, CurrentDirVector.y));
			}
			else
			{
				_agent.isStopped = true;
			}
		}

		protected void OnDrawGizmos()
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(transform.position, _attackRadius);
			Gizmos.color = Color.white;
		}

		public bool AddTag(string tag)
		{
			if(HasTag(tag))
			{
				return false;
			}

			_tags.Add(tag);
			return true;
		}

		public bool RemoveTag(string tag)
		{
			return _tags.Remove(tag);
		}

		public bool HasTag(string tag)
		{
			return _tags.Contains(tag);
		}

		#region Attacks

		public MainAttackAction MainAttack()
		{
			return new MainAttackAction((parameters) =>
			{
				Vector3 centerOfCrowd = Vector3.zero;
				RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, _attackRadius, Vector2.zero);
				for(int i = 0; i < hits.Length; i++)
				{
					RaycastHit2D hit = hits[i];
					if(hit.collider.TryGetComponent(out GameCharacterEntity entity))
					{
						centerOfCrowd += entity.transform.position;
						if (entity.tag != tag)
						{
							entity.Health.Damage(1);
						}
					}
				}

				CharacterView.Slash();
				return new MainAttackAction.ActionResult()
				{
					Success = true
				};
			}, new MainAttackAction.ActionParams() 
			{ 
				Character = this 
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

		public SetDirectionFlagAction SetDirectionFlag(Direction direction, SetDirectionFlagAction.WriteType writeType)
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
				if (parameters.Character.CurrentDirections != Direction.None)
				{
					CharacterState movementState = parameters.Character.Speed >= _runSpeedThreshold ? CharacterState.Run : CharacterState.Walk;

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
				Character = this,
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
	}
}