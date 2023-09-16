using RaCollection;
using RaFSM;
using System;
using UnityEngine;


namespace GameModes.Game
{
	public class UnitsSpawnState : GameFSMState
	{
		[SerializeField]
		private WeightedCharacter[] _charactersPool = null;

		[SerializeField]
		private int _amountToSpawn = 1;

		private RaElementCollection<GameCharacterEntity> _spawnedCharacters;

		protected override void OnInit()
		{
			_spawnedCharacters = new RaElementCollection<GameCharacterEntity>(onAddItem: OnCharacterAdded, onRemoveItem: OnCharacterRemoved);
		}

		protected override void OnEnter()
		{
			CharacterActionsSystem.MainActionEvent.RegisterMethod<CharacterFactoryController.DespawnCharacterAction>(OnUnitDespawned);
			
			for (int i = 0; i < _amountToSpawn; i++)
			{
				if(Dependency.CharacterFactoryController.SpawnCharacter(_charactersPool.GetRandomItem().Character, Vector3.zero).Execute(CharacterActionsSystem.Processor, out var result))
				{
					_spawnedCharacters.Add(result.CreatedCharacter);
				}
			}
		}

		private void OnUnitDespawned(CharacterFactoryController.DespawnCharacterAction action)
		{
			if(	action.Result.Success && 
				action.Parameters.CharacterToDespawn.HasTag(nameof(UnitsSpawnState)) && 
				action.Parameters.CharacterToDespawn.HasTag(GetInstanceID().ToString()))
			{
				_spawnedCharacters.Remove(action.Parameters.CharacterToDespawn);
			}
		}

		protected override void OnExit(bool isSwitch)
		{
			if (CharacterActionsSystem.IsAvailable)
			{
				CharacterActionsSystem.MainActionEvent.UnregisterMethod<CharacterFactoryController.DespawnCharacterAction>(OnUnitDespawned);

				_spawnedCharacters.ForEach(character =>
				{
					Dependency.CharacterFactoryController.DespawnCharacter(character).Execute(CharacterActionsSystem.Processor);
				});
			}

			_spawnedCharacters.Clear();
		}

		protected override void OnDeinit()
		{
			_spawnedCharacters.Dispose();
			_spawnedCharacters = null;
		}

		private void OnCharacterAdded(GameCharacterEntity item, int index)
		{
			item.AddTag(nameof(UnitsSpawnState));
			item.AddTag(GetInstanceID().ToString());
		}

		private void OnCharacterRemoved(GameCharacterEntity item, int index)
		{
			if (IsCurrentState)
			{
				if (_spawnedCharacters.Count == 0)
				{
					GetDependency<IRaFSMCycler>().GoToNextState();
				}
			}
		}

		[Serializable]
		private class WeightedCharacter : IRaCollectionWeightedItem
		{
			[field: SerializeField]
			public int RaItemWeight
			{
				get; private set;
			} = 1;

			[field: SerializeField]
			public GameCharacterEntity Character
			{
				get; private set;
			}
		}
	}
}