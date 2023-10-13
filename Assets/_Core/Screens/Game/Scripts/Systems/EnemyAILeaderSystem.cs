using RaCollection;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Screens.Game
{
	public class EnemyAILeaderSystem : GameSystemBase
	{
		[SerializeField]
		private Vector2 _attackDelayRange = new Vector2(1f, 3f);

		[SerializeField]
		private EnemyFormation _formationPrefab = null;

		// Trackers
		private RaCollection<EnemyAIController> _spawnedEnemies = null;
		private GameCharacterEntity _targetEntity = null;
		private EnemyFormation _targetFormation = null;
		private RaCollection<EnemyAIController> _enemiesInSubsitution = null;

		// Formation Maps
		private Dictionary<EnemyAIController, Transform> _enemyToFormationPointMap = new Dictionary<EnemyAIController, Transform>();
		private Dictionary<Transform, EnemyAIController> _formationPointToEnemyMap = new Dictionary<Transform, EnemyAIController>();
		private bool[] _pointsOccupationStatus = null;

		// Systems
		private CharacterActionsSystem _characterActionsSystem = null;

		// Look-ups
		public GameCharacterEntity TargetEntity => _targetEntity;

		private CancellationTokenSource _cancellationTokenSource = null;

		protected override void OnSetup()
		{
			_spawnedEnemies = new RaCollection<EnemyAIController>(OnEnemyAdded, OnEnemyRemoved);
			_enemiesInSubsitution = new RaCollection<EnemyAIController>();

			_characterActionsSystem = GetDependency<CharacterActionsSystem>();

			_characterActionsSystem.MainActionEvent.RegisterMethod<CharacterCoreSystem.SpawnCharacterAction>(OnSpawnedCharacter);
			_characterActionsSystem.MainActionEvent.RegisterMethod<CharacterCoreSystem.DespawnCharacterAction>(OnDespawnedCharacter);
		}

		protected override void OnStart()
		{
			SelectAttackerRoutine();
		}

		protected override void OnEnd()
		{
			if(_cancellationTokenSource != null)
			{
				_cancellationTokenSource.Cancel();
				_cancellationTokenSource.Dispose();
				_cancellationTokenSource = null;
			}

			if (_spawnedEnemies != null)
			{
				_spawnedEnemies.Dispose();

				_characterActionsSystem.MainActionEvent.UnregisterMethod<CharacterCoreSystem.DespawnCharacterAction>(OnDespawnedCharacter);
				_characterActionsSystem.MainActionEvent.UnregisterMethod<CharacterCoreSystem.SpawnCharacterAction>(OnSpawnedCharacter);
				_spawnedEnemies = null;
			}

			if (_enemiesInSubsitution != null)
			{
				_enemiesInSubsitution.Dispose();
				_enemiesInSubsitution = null;
			}


			if (_targetFormation != null)
			{
				Destroy(_targetFormation.gameObject);
			}

			_enemyToFormationPointMap.Clear();
			_formationPointToEnemyMap.Clear();

			_pointsOccupationStatus = null;
		}

		private void OnEnemyAdded(EnemyAIController enemy, int index)
		{
			enemy.EnemyLockedStateChangedEvent += OnEnemyLockedStateChangedEvent;

			// Register, if not locked
			if (!enemy.Character.IsCharacterLocked)
			{
				if (TryGetFirstAvailablePoint(out Transform point))
				{
					RegisterEnemyToPoint(enemy, point);
				}
				else
				{
					_enemiesInSubsitution.Add(enemy);
				}
			}
		}

		private void OnEnemyRemoved(EnemyAIController enemy, int index)
		{
			enemy.EnemyLockedStateChangedEvent -= OnEnemyLockedStateChangedEvent;

			// Unregister
			UnregisterEnemyPoint(enemy);
			_enemiesInSubsitution.Remove(enemy);

			// Try Assign unassigned enemy to any free points
			if (_enemiesInSubsitution.Count > 0 && TryGetFirstAvailablePoint(out Transform point))
			{
				RegisterEnemyToPoint(_enemiesInSubsitution.Dequeue(), point);
			}
		}

		private void OnSpawnedCharacter(CharacterCoreSystem.SpawnCharacterAction spawnAction)
		{
			if (spawnAction.Result.Success)
			{
				if (spawnAction.Result.CreatedCharacter.TryGetComponent(out EnemyAIController enemyAI))
				{
					_spawnedEnemies.Add(enemyAI);
				}
				else if (_targetEntity == null && spawnAction.Result.CreatedCharacter.TryGetComponent<CharacterInputController>(out _))
				{
					_targetEntity = spawnAction.Result.CreatedCharacter;
					_targetFormation = Instantiate(_formationPrefab, _targetEntity.transform, worldPositionStays: false);
					_targetFormation.transform.localPosition = Vector3.zero;
					_pointsOccupationStatus = new bool[_targetFormation.Points.Length];
				}
			}
		}

		private void OnDespawnedCharacter(CharacterCoreSystem.DespawnCharacterAction despawnedAction)
		{
			if (despawnedAction.Result.Success)
			{
				if (despawnedAction.Parameters.CharacterToDespawn.TryGetComponent(out EnemyAIController enemyAI))
				{
					_spawnedEnemies.Remove(enemyAI);
				}
				else if (despawnedAction.Parameters.CharacterToDespawn == _targetEntity)
				{
					_targetEntity = null;
					Destroy(_targetFormation.gameObject);
					_pointsOccupationStatus = null;
				}
			}
		}

		private void RegisterEnemyToPoint(EnemyAIController enemy, Transform point)
		{
			UnregisterEnemyPoint(point);
			enemy.SetCurrentFormationPoint(TargetEntity, point);
			_enemyToFormationPointMap[enemy] = point;
			_formationPointToEnemyMap[point] = enemy;
			SetOccupationStatus(point, true);

			// Actually apply the Formation State 
			enemy.GoToFormationState();
		}

		private void UnregisterEnemyPoint(EnemyAIController enemy)
		{
			if (_enemyToFormationPointMap.TryGetValue(enemy, out Transform point))
			{
				_enemyToFormationPointMap.Remove(enemy);
				_formationPointToEnemyMap.Remove(point);
				SetOccupationStatus(point, false);
				enemy.ClearCurrentFormationPoint();
				enemy.GoToIdleState();
			}
		}

		private void UnregisterEnemyPoint(Transform point)
		{
			if (_formationPointToEnemyMap.TryGetValue(point, out EnemyAIController enemy))
			{
				_enemyToFormationPointMap.Remove(enemy);
				_formationPointToEnemyMap.Remove(point);
				SetOccupationStatus(point, false);
				enemy.ClearCurrentFormationPoint();
				enemy.GoToIdleState();
			}
		}

		private void SetOccupationStatus(Transform point, bool status)
		{
			if (_targetFormation != null)
			{
				_pointsOccupationStatus[_targetFormation.GetIndex(point)] = status;
			}
		}

		private int GetFirstAvailablePointIndex()
		{
			if (_pointsOccupationStatus != null)
			{
				for (int i = 0, c = _pointsOccupationStatus.Length; i < c; i++)
				{
					if (!_pointsOccupationStatus[i])
					{
						return i;
					}
				}
			}
			return -1;
		}

		private bool TryGetFirstAvailablePoint(out Transform point)
		{
			int index = GetFirstAvailablePointIndex();
			if (index >= 0)
			{
				point = _targetFormation.Points[index];
				return true;
			}
			point = default;
			return false;
		}

		private void OnEnemyLockedStateChangedEvent(EnemyAIController enemy, bool isLocked)
		{
			EnemyAIController enemyToAssignPoint;

			// When Locked, Remove from Formation and from Subsitution.
			// Then try to assign a different enemy to a spot in the formation if one was made free.
			if (isLocked)
			{
				UnregisterEnemyPoint(enemy);
				_enemiesInSubsitution.Remove(enemy);
				enemyToAssignPoint = _enemiesInSubsitution.Count > 0 ? _enemiesInSubsitution[0] : null;
			}
			// Else, when it is unlocked, add it to subsitution and then try to give IT a spot in the formation
			else
			{
				_enemiesInSubsitution.Add(enemy);
				enemyToAssignPoint = enemy;
			}

			if (enemyToAssignPoint != null && TryGetFirstAvailablePoint(out Transform point))
			{
				_enemiesInSubsitution.Remove(enemyToAssignPoint);
				RegisterEnemyToPoint(enemyToAssignPoint, point);
			}
		}

		private async void SelectAttackerRoutine()
		{
			_cancellationTokenSource = new CancellationTokenSource();
			CancellationToken token = _cancellationTokenSource.Token;

			while (IsInitialized)
			{
				if(token.IsCancellationRequested)
				{
					Debug.Log("End Leader Attacker Routine");
					break;
				}

				try
				{
					await Task.Delay(Mathf.RoundToInt(Random.Range(_attackDelayRange.x, _attackDelayRange.y) * 1000), cancellationToken: token);
				}
				catch
				{
					Debug.Log("End Leader Attacker Routine");
					break;
				}

				if (_enemyToFormationPointMap.Count == 0)

				{
					continue;
				}

				EnemyAIController[] formationEnemies = _enemyToFormationPointMap.Keys.ToArray();
				formationEnemies.Shuffle();

				EnemyAIController enemyToAttack = null;

				for (int i = 0; i < formationEnemies.Length; i++)
				{
					EnemyAIController formationEnemy = formationEnemies[i];

					if(formationEnemy.CurrentFormationPoint == null)
					{
						continue;
					}

					if(formationEnemy.CurrentStateIndex != EnemyAIController.FORMATION_INDEX)
					{
						continue;
					}

					Vector2 formationPointDeltaFromCenter = formationEnemy.CurrentFormationPoint.transform.position - formationEnemy.CurrentFormationPoint.parent.transform.position;
					float maxAllowedDistanceFromTarget = formationPointDeltaFromCenter.magnitude + 1f;

					if(Vector2.Distance(formationEnemy.Character.transform.position, TargetEntity.transform.position) > maxAllowedDistanceFromTarget)
					{
						continue;
					}

					enemyToAttack = formationEnemy;
					break;
				}

				if (enemyToAttack != null)
				{
					enemyToAttack.GoToAttackState();
				}
			}
		}
	}
}
