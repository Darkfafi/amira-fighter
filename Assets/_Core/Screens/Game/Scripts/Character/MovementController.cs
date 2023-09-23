using RaTweening;
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

		public Vector2 Velocity => Agent.isStopped ? Vector2.zero : Agent.velocity;


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
	}
}