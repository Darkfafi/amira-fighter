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
					Character.CoreSystem.SetDirectionFlag(Character, dir, CharacterCoreSystem.SetDirectionFlagAction.WriteType.Add)
						.Execute(Character.CoreSystem.Processor);
				}

				if(Input.GetKeyUp(key))
				{
					Character.CoreSystem.SetDirectionFlag(Character, dir, CharacterCoreSystem.SetDirectionFlagAction.WriteType.Remove)
						.Execute(Character.CoreSystem.Processor);
				}
			}

			if(Input.GetKeyDown(KeyCode.K))
			{
				Character.CoreSystem.MainAttack(Character)
					.Execute(Character.CoreSystem.Processor);
			}
		}
	}
}