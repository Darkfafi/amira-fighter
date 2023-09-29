using UnityEngine;
using static Screens.Game.Tools;

namespace Screens.Game
{
	public class CharacterInputController : MonoBehaviour
	{
		[field: SerializeField]
		public GameCharacterEntity Character
		{
			get; private set;
		}

		[SerializeField]
		private float _directionRange = 0.5f;

		[SerializeField]
		private bool _isInputLocked = false;

		private Direction _direction = Direction.None;

		private (KeyCode, Direction)[] _keycodeToDirectionMap = new (KeyCode, Direction)[]
		{
			(KeyCode.A, Direction.Left),
			(KeyCode.W, Direction.Up),
			(KeyCode.D, Direction.Right),
			(KeyCode.S, Direction.Down),
		};

		private KeyCode[] _skillKeycodes = new KeyCode[]
		{
			KeyCode.J,
			KeyCode.K
		};

		protected void Awake()
		{
			Character.CharacterLockedStateChangedEvent += OnCharacterLockedStateChangedEvent;
			OnCharacterLockedStateChangedEvent(Character.IsCharacterLocked);
		}

		protected void OnDestroy()
		{
			if(Character)
			{
				Character.CharacterLockedStateChangedEvent -= OnCharacterLockedStateChangedEvent;
			}
		}

		protected void Update()
		{
			for (int i = 0; i < _keycodeToDirectionMap.Length; i++)
			{
				(KeyCode key, Direction dir) = _keycodeToDirectionMap[i];

				if (Input.GetKeyDown(key))
				{
					_direction |= dir;
				}

				if(Input.GetKeyUp(key))
				{
					_direction &= ~dir;
				}
			}

			if (_isInputLocked)
			{
				return;
			}

			if (_direction != Direction.None)
			{
				Vector2 destination = Character.transform.position;
				destination += _direction.ToVector(normalized: true) * _directionRange;
				Character.MovementController.Destination = destination;
			}
			else
			{
				Character.MovementController.Destination = null;
			}

			for(int i = 0; i < _skillKeycodes.Length; i++)
			{
				KeyCode keyCode = _skillKeycodes[i];
				if(Input.GetKeyDown(keyCode) && i <= Character.AllSkills.Length - 1)
				{
					Character.CoreSystem.UseSkill(Character.AllSkills[i])
						.Execute(Character.CoreSystem.Processor);
				}
			}
		}

		private void OnCharacterLockedStateChangedEvent(bool isLocked)
		{
			if (_isInputLocked = isLocked)
			{
				if (_direction != Direction.None)
				{
					Character.MovementController.Destination = null;
				}
			}
		}
	}
}