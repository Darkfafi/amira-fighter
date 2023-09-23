using RaFSM;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Screens.Game
{
	public class EnemyAIController : MonoBehaviour
	{
		public const int IDLE_INDEX = 0;
		public const int CHASE_INDEX = 1;
		public const int ATTACK_INDEX = 2;

		[field: SerializeField]
		public GameCharacterEntity Character
		{
			get; private set;
		}

		[SerializeField]
		private float _lookRadius = 2f;

		[SerializeField]
		private Transform _statesRoot = null;

		[Header("ReadOnly")]
		[SerializeField]
		private List<EnemyAIController> _aliesInLookRadius = new List<EnemyAIController>();

		[SerializeField]
		private CharacterInputController _targetInLookRadius = null;

		private RaGOFiniteStateMachine _aiFSM = null;
		private IEnumerator _stateSwitchingRoutine = null;

		public EnemyAIController[] Alies => _aliesInLookRadius.ToArray();
		public CharacterInputController Target => _targetInLookRadius;
		public int CurrentStateIndex => _aiFSM.CurrentStateIndex;

		protected void Awake()
		{
			_aiFSM = new RaGOFiniteStateMachine(this, RaGOFiniteStateMachine.GetGOStates(_statesRoot));
		}

		protected void Start()
		{
			_aiFSM.SwitchState(IDLE_INDEX);
			StartCoroutine(_stateSwitchingRoutine = StateSwitchingRoutine());
		}

		private IEnumerator StateSwitchingRoutine()
		{
			while (_stateSwitchingRoutine != null)
			{
				// Refresh Sight
				_targetInLookRadius = null;
				CharacterInputController target = FindObjectOfType<CharacterInputController>(includeInactive: false);
				Vector2 deltaToEnemy = target.transform.position - Character.transform.position;
				if (deltaToEnemy.magnitude <= _lookRadius)
				{
					_targetInLookRadius = target;
				}

				_aliesInLookRadius.Clear();
				EnemyAIController[] alies = FindObjectsByType<EnemyAIController>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID);
				for (int i = 0; i < alies.Length; i++)
				{
					EnemyAIController ally = alies[i];
					Vector2 delta = ally.transform.position - Character.transform.position;
					if (delta.magnitude <= _lookRadius && ally != this)
					{
						_aliesInLookRadius.Add(ally);
					}
				}

				switch(_aiFSM.CurrentStateIndex)
				{
					// When Idle, Try to start Chase
					case IDLE_INDEX:
						if (!TryAttack())
						{
							// When we have a target, try to Chase them
							if (_targetInLookRadius != null)
							{
								_aiFSM.SwitchState(CHASE_INDEX);
							}
							// Try to find an Ally which is chasing the Enemy
							else
							{
								for (int i = _aliesInLookRadius.Count - 1; i >= 0; i--)
								{
									EnemyAIController ally = _aliesInLookRadius[i];
									if (ally.CurrentStateIndex > IDLE_INDEX)
									{
										_aiFSM.SwitchState(CHASE_INDEX);
										break;
									}
								}
							}
						}
						break;
					case CHASE_INDEX:
						TryAttack();
						break;
				}

				yield return new WaitForSeconds(Random.Range(0.45f, 1f));
			}
			StopStateSwitchingRoutine();
		}

		protected void OnDestroy()
		{
			StopStateSwitchingRoutine();

			_aiFSM.Dispose();
			_aiFSM = null;
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawWireSphere(transform.position, _lookRadius);
			Gizmos.color = Color.white;
		}

		private void StopStateSwitchingRoutine()
		{
			if (_stateSwitchingRoutine != null)
			{
				StopCoroutine(_stateSwitchingRoutine);
				_stateSwitchingRoutine = null;
			}
		}

		private bool TryAttack()
		{
			if (Target != null && Vector2.Distance(Target.transform.position, Character.transform.position) <= Character.AttackRadius)
			{
				_aiFSM.SwitchState(ATTACK_INDEX);
				return true;
			}
			return false;
		}

		public void GoToIdleState()
		{
			_aiFSM.SwitchState(IDLE_INDEX);
		}
	}
}