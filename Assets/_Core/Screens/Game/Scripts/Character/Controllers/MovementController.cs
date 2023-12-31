﻿using RaFlags;
using UnityEngine;
using UnityEngine.AI;

namespace Screens.Game
{
	public sealed class MovementController : MonoBehaviour
	{
		[field: SerializeField]
		public NavMeshAgent Agent
		{
			get; private set;
		}

		[field: SerializeField]
		public Vector2 SpeedRange
		{
			get; private set;
		}


		private RaFlagsTracker _movementBlockers; 
		private RaFlagsTracker _agentDisablers;

		public Vector2? Destination
		{
			get => Agent.isStopped ? null : Agent.destination;
			set
			{
				if (Agent && Agent.isActiveAndEnabled)
				{
					if (!(Agent.isStopped = !value.HasValue))
					{
						Vector3 destination = value.Value;
						destination.z = Agent.transform.position.z;
						Agent.SetDestination(destination);
					}
				}
			}
		}

		public bool IsMoving => !Mathf.Approximately(Velocity.magnitude, 0f);

		public Vector2 Velocity => 
			(Agent == null || 
			!Agent.isActiveAndEnabled ||
			Agent.isStopped) ? Vector2.zero : Agent.velocity;

		public RaFlagsTracker MovementBlockers
		{
			get
			{
				if(_movementBlockers == null)
				{
					_movementBlockers = new RaFlagsTracker(OnBlockStateChangedEvent);
				}
				return _movementBlockers;
			}
		}

		public RaFlagsTracker AgentDisablers
		{
			get
			{
				if(_agentDisablers == null)
				{
					_agentDisablers = new RaFlagsTracker(OnAgentDisabledStateChangedEvent);
				}
				return _agentDisablers;
			}
		}

		public float Speed
		{
			get; private set;
		}

		private void Awake()
		{
			Setup();
		}

		public void Setup()
		{
			Agent.updateRotation = false;
			Agent.updateUpAxis = false;
			Agent.speed = Speed = Random.Range(SpeedRange.x, SpeedRange.y);
			Destination = null;
		}

		private void OnBlockStateChangedEvent(bool isEmpty, RaFlagsTracker tracker)
		{
			if(!isEmpty)
			{
				Destination = null;
			}
		}

		private void OnAgentDisabledStateChangedEvent(bool isEmpty, RaFlagsTracker tracker)
		{
			Agent.enabled = isEmpty;
		}
	}
}