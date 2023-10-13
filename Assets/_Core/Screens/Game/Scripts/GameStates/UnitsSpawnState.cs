using RaCollection;
using RaFSM;
using System;
using UnityEngine;

namespace Screens.Game
{
	public class UnitsSpawnState : GameFSMStateBase
	{
		[SerializeField]
		private GameCharacterEntity[] _mendatoryCharacters = null;

		[SerializeField]
		private WeightedCharacter[] _charactersPool = null;

		[SerializeField]
		private int _amountToSpawn = 1;

		private RaElementCollection<GameCharacterEntity> _spawnedCharacters;

		protected override void OnInit()
		{
			base.OnInit();
			_spawnedCharacters = new RaElementCollection<GameCharacterEntity>(onAddItem: OnCharacterAdded, onRemoveItem: OnCharacterRemoved);
		}

		protected override void OnEnter()
		{
			base.OnEnter();
			Dependency.GameSystems.CharacterActionsSystem.MainActionEvent.RegisterMethod<CharacterCoreSystem.DespawnCharacterAction>(OnUnitDespawned);

			for (int i = 0; i < _mendatoryCharacters.Length; i++)
			{
				Vector3 spawnPoint = Dependency.Level.GetEnemySpawnPoint(0).GetSpawnPosition();
				if (Dependency.GameSystems.CharacterCoreSystem.SpawnCharacter(_mendatoryCharacters[i], spawnPoint, UnityEngine.Random.Range(-1f, 1f))
					.Execute(Dependency.GameSystems.CharacterCoreSystem.Processor, out var result))
				{
					_spawnedCharacters.Add(result.CreatedCharacter);
				}
			}

			for (int i = 0; i < _amountToSpawn; i++)
			{
				Vector3 spawnPoint = Dependency.Level.GetEnemySpawnPoint(0).GetSpawnPosition();
				if(Dependency.GameSystems.CharacterCoreSystem.SpawnCharacter(_charactersPool.GetRandomItem().Character, spawnPoint, UnityEngine.Random.Range(-1f, 1f))
					.Execute(Dependency.GameSystems.CharacterCoreSystem.Processor, out var result))
				{
					_spawnedCharacters.Add(result.CreatedCharacter);
				}
			}
		}

		private void OnUnitDespawned(CharacterCoreSystem.DespawnCharacterAction action)
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
			Dependency.GameSystems.CharacterActionsSystem.MainActionEvent.UnregisterMethod<CharacterCoreSystem.DespawnCharacterAction>(OnUnitDespawned);
			_spawnedCharacters.Clear();
			base.OnExit(isSwitch);
		}

		protected override void OnDeinit()
		{
			_spawnedCharacters.Dispose();
			_spawnedCharacters = null;
			base.OnDeinit();
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