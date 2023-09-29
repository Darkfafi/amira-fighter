using RaFSM;
using System;
using UnityEngine;

namespace Screens.Game
{
	public class EnemyAIController : MonoBehaviour
	{
		public event Action<EnemyAIController> EnemyKilledEvent;
		public event Action<EnemyAIController, bool> EnemyLockedStateChangedEvent;

		public const int IDLE_INDEX = 0;
		public const int FORMATION_INDEX = 1;
		public const int ATTACK_INDEX = 2;

		[field: SerializeField]
		public GameCharacterEntity Character
		{
			get; private set;
		}

		[SerializeField]
		private Transform _statesRoot = null;

		[field: Header("ReadOnly")]
		[field: SerializeField]
		public Transform CurrentFormationPoint
		{
			get; private set;
		}

		[field: SerializeField]
		public GameCharacterEntity Target
		{
			get; private set;
		}

		private RaGOFiniteStateMachine _aiFSM = null;
		public int CurrentStateIndex => _aiFSM.CurrentStateIndex;

		protected void Awake()
		{
			_aiFSM = new RaGOFiniteStateMachine(this, RaGOFiniteStateMachine.GetGOStates(_statesRoot));
			Character.CharacterLockedStateChangedEvent += OnCharacterLockedStateChangedEvent;
			Character.CharacterKilledEvent += OnCharacterKilledEvent;
		}

		protected void Start()
		{
			if(!_aiFSM.TryGetCurrentState(out _))
			{
				GoToIdleState();
			}
		}

		public void SetCurrentFormationPoint(GameCharacterEntity target, Transform formationPoint)
		{
			Target = target;
			CurrentFormationPoint = formationPoint;
		}

		public void ClearCurrentFormationPoint()
		{
			Target = null;
			CurrentFormationPoint = null;
		}

		protected void OnDestroy()
		{
			if(Character != null)
			{
				Character.CharacterKilledEvent -= OnCharacterKilledEvent;
				Character.CharacterLockedStateChangedEvent -= OnCharacterLockedStateChangedEvent;
			}

			_aiFSM.Dispose();
			_aiFSM = null;
		}

		public void GoToIdleState()
		{
			if(Character.IsCharacterLocked)
			{
				return;
			}

			_aiFSM.SwitchState(IDLE_INDEX);
		}

		public void GoToFormationState()
		{
			if (Character.IsCharacterLocked)
			{
				return;
			}

			_aiFSM.SwitchState(FORMATION_INDEX);
		}

		public void GoToAttackState()
		{
			if(Character.IsCharacterLocked)
			{
				return;
			}

			_aiFSM.SwitchState(ATTACK_INDEX);
		}

		private void OnCharacterLockedStateChangedEvent(bool isLocked)
		{
			if (isLocked)
			{
				_aiFSM.SwitchState(null);
			}
			else
			{
				GoToIdleState();
			}

			EnemyLockedStateChangedEvent?.Invoke(this, isLocked);
		}

		private void OnCharacterKilledEvent()
		{
			EnemyKilledEvent?.Invoke(this);
		}
	}
}