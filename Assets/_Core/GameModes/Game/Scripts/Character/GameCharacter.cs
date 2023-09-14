using Assets.HeroEditor.Common.Scripts.CharacterScripts;
using UnityEngine;
using RaActions;
using static GameModes.Game.Tools;

namespace GameModes.Game
{
	public class GameCharacter : MonoBehaviour
	{
		[SerializeField]
		private Rigidbody2D _rigidBody2D = null;

		[SerializeField]
		private float _speed = 1f;

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

		protected void Awake()
		{
			SetDirectionFlag(Direction.None, true).Execute(CharacterActionsSystem.Processor);
		}

		protected void FixedUpdate()
		{
			if(CurrentDirections != Direction.None)
			{
				_rigidBody2D.MovePosition(_rigidBody2D.position + CurrentDirVector * _speed * Time.fixedDeltaTime);
			}
		}

		public MainAttackAction MainAttack()
		{
			return new MainAttackAction((parameters) =>
			{
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

		public SetDirectionFlagAction SetDirectionFlag(Direction direction, bool enabled)
		{
			return new SetDirectionFlagAction((parameters) => 
			{
				// Logics
				if(enabled)
				{
					parameters.Character.CurrentDirections |= parameters.Direction;
				}
				else
				{
					parameters.Character.CurrentDirections &= ~parameters.Direction;
				}

				parameters.Character.CurrentDirVector = parameters.Character.CurrentDirections.ToVector(normalized: true);

				// Visuals
				if(parameters.Character.CurrentDirections != Direction.None)
				{
					parameters.Character.CharacterView.SetState(CharacterState.Run);
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
				Enabled = enabled,
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
				public GameCharacter Character;
			}

			public struct ActionResult : IRaActionResult
			{
				public bool Success
				{
					get; set;
				}

			}
		}

		public class SetDirectionFlagAction : RaAction<SetDirectionFlagAction.ActionParams, SetDirectionFlagAction.ActionResult>
		{
			public SetDirectionFlagAction(MainHandler executeMethod, ActionParams parameters) 
				: base(executeMethod, parameters)
			{
			}

			public struct ActionParams
			{
				public GameCharacter Character;
				public Direction Direction;
				public bool Enabled;
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