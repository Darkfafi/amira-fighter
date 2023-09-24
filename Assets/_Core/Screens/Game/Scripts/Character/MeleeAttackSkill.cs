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

		[SerializeField, Range(0f, 1f)]
		private float _momentOfImpact = 1f;

		[SerializeField]
		private float _attackDuration = 1f;

		[SerializeField]
		private bool _hitSingleCharacter = true;

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
			Debug.Log("Attack - Wait For Impact");
			float secondsUntilImpact = _attackDuration * _momentOfImpact;
			yield return new WaitForSeconds(secondsUntilImpact);

			// Perform Hit Logics
			var hits = Physics2D.CircleCastAll(Character.transform.position, AttackRadius, new Vector2(Character.CharacterView.transform.localScale.x, 0));

			Vector2 hitPos = Character.transform.position;
			hitPos += new Vector2(Character.CharacterView.transform.localScale.x, 0) * 0.15f;

			// Sort on distance from hit position
			Array.Sort(hits, (a, b) =>
			{
				float distanceA = Vector2.Distance(a.transform.position, hitPos);
				float distanceB = Vector2.Distance(b.transform.position, hitPos);
				return (int)Mathf.Sign(distanceA - distanceB);
			});

			Debug.Log("Attack - Impact");
			for (int i = 0, c = hits.Length; i < c; i++)
			{
				var hit = hits[i];

				// If we hit a body
				// And the body is attached to a GameCharacterEntity 
				// And it is not aligned with us (or us)
				// Perform Hit
				if (hit.collider.tag == "Body" &&
					hit.transform.TryGetComponent(out GameCharacterEntity entity) &&
					entity.tag != Character.transform.tag)
				{
					entity.Health.Damage(1);
					if (_hitSingleCharacter)
					{
						break;
					}
				}
			}

			// Wait until End
			Debug.Log("Attack - Wait for animation to end");
			yield return new WaitForSeconds(_attackDuration - secondsUntilImpact);
			Debug.Log("Attack - End");
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
			Gizmos.DrawWireSphere(transform.position, AttackRadius);
			Gizmos.color = Color.white;
		}
	}
}