using UnityEngine;
using static GameModes.Game.Tools;

namespace GameModes.Game
{
	public class CharacterInputController : MonoBehaviour
	{
		[SerializeField]
		private GameCharacterEntity _gameCharacter = null;

		private (KeyCode, Direction)[] _keycodeToDirectionMap = new (KeyCode, Direction)[]
		{
			(KeyCode.A, Direction.Left),
			(KeyCode.W, Direction.Up),
			(KeyCode.D, Direction.Right),
			(KeyCode.S, Direction.Down),
		};

		protected void Update()
		{
			for(int i = 0; i < _keycodeToDirectionMap.Length; i++)
			{
				(KeyCode key, Direction dir) = _keycodeToDirectionMap[i];

				if(Input.GetKeyDown(key))
				{
					_gameCharacter.SetDirectionFlag(dir, true).Execute(CharacterActionsSystem.Processor);
				}

				if(Input.GetKeyUp(key))
				{
					_gameCharacter.SetDirectionFlag(dir, false).Execute(CharacterActionsSystem.Processor);
				}
			}

			if(Input.GetKeyDown(KeyCode.K))
			{
				_gameCharacter.MainAttack().Execute(CharacterActionsSystem.Processor);
			}
		}
	}
}