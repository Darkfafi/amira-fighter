using UnityEngine;
using static GameModes.Game.Tools;

namespace GameModes.Game
{
	public class CharacterInputController : MonoBehaviour
	{
		[field: SerializeField]
		public GameCharacterEntity Character
		{
			get; private set;
		}

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
					CharacterCoreSystem.Instance.SetDirectionFlag(Character, dir, CharacterCoreSystem.SetDirectionFlagAction.WriteType.Add)
						.Execute(CharacterActionsSystem.Processor);
				}

				if(Input.GetKeyUp(key))
				{
					CharacterCoreSystem.Instance.SetDirectionFlag(Character, dir, CharacterCoreSystem.SetDirectionFlagAction.WriteType.Remove)
						.Execute(CharacterActionsSystem.Processor);
				}
			}

			if(Input.GetKeyDown(KeyCode.K))
			{
				CharacterCoreSystem.Instance.MainAttack(Character)
					.Execute(CharacterActionsSystem.Processor);
			}
		}
	}
}