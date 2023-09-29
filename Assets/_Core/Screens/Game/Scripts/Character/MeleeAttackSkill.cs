using RaProgression;
using System;
using System.Collections;
using UnityEngine;

namespace Screens.Game
{
	public class MeleeAttackSkill : CharacterSkillBase
	{
		[field: SerializeField]
		public float AttackRadius
		{
			get; private set;

		} = 1.5f;


		[field: SerializeField]
		public float AttackDistance
		{
			get; private set;

		} = 0.5f;

		[SerializeField, Range(0f, 1f)]
		private float _momentOfImpact = 1f;

		[SerializeField]
		private float _attackDuration = 1f;

		[SerializeField]
		private float _knockbackDuration = 1f;

		[SerializeField]
		private float _knockbackStrength = 1f;

		private IEnumerator _attackRoutine = null;

		protected override void DoPerform(RaProgress progres)
		{
			StartCoroutine(_attackRoutine = AttackRoutine(progres));
		}

		private IEnumerator AttackRoutine(RaProgress progress)
		{
			// Animate Attack
			Character.CharacterView.Slash();

			// Wait until Impact
			float secondsUntilImpact = _attackDuration * _momentOfImpact;
			yield return new WaitForSeconds(secondsUntilImpact);

			// Perform Hit Logics
			Vector2 hitPos = transform.position;
			hitPos += new Vector2(Character.DirectionView.localScale.x, 0) * AttackDistance;
			var hits = Physics2D.OverlapCircleAll(hitPos, AttackRadius);

			Vector2 centerPos = Character.transform.position + ((new Vector3(hitPos.x, hitPos.y) - Character.transform.position) * 0.5f);

			// Sort on distance from hit position
			Array.Sort(hits, (a, b) =>
			{
				float distanceA = Vector2.Distance(a.transform.position, centerPos);
				float distanceB = Vector2.Distance(b.transform.position, centerPos);
				return (int)Mathf.Sign(distanceA - distanceB);
			});

			for (int i = 0, c = hits.Length; i < c; i++)
			{
				var hit = hits[i];

				// If we hit a body
				// And the body is attached to a GameCharacterEntity 
				// And it is not aligned with us (or us)
				// Perform Hit
				if (hit.tag == "MainCollider" &&
					hit.transform.root.TryGetComponent(out GameCharacterEntity entity) &&
					entity.tag != Character.transform.tag)
				{
					if (entity.Health.Damage(1))
					{
						Vector2 delta = entity.transform.position - Character.transform.position;

						entity.KnockbackController.Knockback(delta.normalized, _knockbackDuration, _knockbackStrength);
					}
				}
			}

			// Wait until End
			yield return new WaitForSeconds(_attackDuration - secondsUntilImpact);
			progress.Complete();
			EndAttackRoutine();
		}

		private void EndAttackRoutine()
		{
			if(_attackRoutine != null)
			{
				StopCoroutine(_attackRoutine);
				_attackRoutine = null;
			}
		}

		protected override void OnEnded()
		{
			base.OnEnded();
		}

		protected void OnDrawGizmos()
		{
			Gizmos.color = Color.red;
			if (Character != null && Character.DirectionView != null)
			{
				Gizmos.DrawWireSphere(transform.position + new Vector3(Character.DirectionView.localScale.x, 0, 0) * AttackDistance, AttackRadius);
			}
			else
			{
				Gizmos.DrawWireSphere(transform.position + Vector3.right * AttackDistance, AttackRadius);
			}
			Gizmos.color = Color.white;
		}
	}
}