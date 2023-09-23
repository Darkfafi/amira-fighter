﻿using System.Collections;
using UnityEngine;

namespace Screens.Game
{
	public class AI_ChaseState : EnemyAIStateBase
	{
		[SerializeField]
		private float _separationRadius = 0.25f;

		private IEnumerator _chaseRoutine = null;

		[field: SerializeField]
		public bool IsChasing
		{
			get; private set;
		}

		protected override void OnInit()
		{
			IsChasing = false;
		}

		protected override void OnEnter()
		{
			IsChasing = true;
			StartCoroutine(_chaseRoutine = ChaseRoutine());
		}

		protected override void OnExit(bool isSwitch)
		{
			StopCoroutine(_chaseRoutine);
			_chaseRoutine = null;

			IsChasing = false;

			Dependency.Character.MovementController.Destination = null;
		}

		protected override void OnDeinit()
		{
			IsChasing = false;
		}

		private IEnumerator ChaseRoutine()
		{
			while (IsCurrentState)
			{
				// I want to walk towards the target, or follow somebody who sees the target
				// When I see the Target, set his position as the target position
				// When I see an ally that sees the target, set his position as the target position

				// When near allies, stop if my line of direction collides with them

				// Boids Behaviour. Get All Within Radius
				EnemyAIController[] alies = Dependency.Alies;
				CharacterInputController target = Dependency.Target;

				// To Target
				Vector3? targetPosition = null;

				if (target != null)
				{
					targetPosition = target.transform.position;
				}
				else
				{
					for (int i = 0; i < alies.Length; i++)
					{
						EnemyAIController ally = alies[i];
						if (ally.Target != null)
						{
							targetPosition = ally.transform.position;
							break;
						}
					}
				}
				
				if(targetPosition.HasValue)
				{
					RaycastHit2D[] hits = Physics2D.RaycastAll(Dependency.transform.position, targetPosition.Value, float.MaxValue, LayerMask.GetMask("Play Area", "Enemy - Play Area", "Player - Play Area"));
					for (int i = 0; i < hits.Length; i++)
					{
						RaycastHit2D hit = hits[i];
						
						if(hit.collider.tag != "Blocker")
						{
							continue;
						}
						
						if (hit.transform.root == Dependency.transform.root)
						{
							continue;
						}

						Vector3 delta = hit.collider.gameObject.transform.position - Dependency.transform.position;
						targetPosition = hit.transform.position - (delta.normalized * _separationRadius);
						break;
					}
				}

				IsChasing = targetPosition.HasValue;

				if (IsChasing)
				{
					Dependency.Character.MovementController.Destination = targetPosition.Value;
				}
				else
				{
					Dependency.Character.MovementController.Destination = null;
					Dependency.GoToIdleState();
				}

				yield return new WaitForSeconds(Random.Range(0.25f, 0.5f));
			}
		}

		private void OnDrawGizmos()
		{
			Gizmos.DrawWireSphere(transform.position, _separationRadius);
		}
	}
}