using UnityEngine;
using RaActions;

namespace GameModes.Game
{
	public class CharacterFactoryController : MonoBehaviour
	{
		[SerializeField]
		private Transform _charactersParent = null;

		public SpawnCharacterAction SpawnCharacter(GameCharacterEntity characterPrefab, Vector3 position)
		{
			return new SpawnCharacterAction((parameters) => 
			{
				if (parameters.CharacterPrefab)
				{
					GameCharacterEntity instance = Instantiate(parameters.CharacterPrefab, parameters.Position, Quaternion.identity, _charactersParent);
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