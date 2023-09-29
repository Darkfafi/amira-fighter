using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Screens.Game
{
	public class KnockbackController : MonoBehaviour
	{
		[field: SerializeField]
		public GameCharacterEntity Character
		{
			get; private set;
		}

		private IEnumerator _knockbackRoutine = null;
		private Action _onCompleteInternal = null;
		private readonly object _knockbackLocker = new object();

		public void Knockback(Vector2 direction, float knockbackDuration, float knockbackStrength)
		{
			if(Character.CharacterLockedTracker.Flags.Count > 1)
			{
				return;
			}

			if(	Character.CharacterLockedTracker.Flags.Count == 1 && 
				!Character.CharacterLockedTracker.HasFlag(_knockbackLocker))
			{
				return;
			}

			StopKnockback();

			Character.MovementController.MovementBlockers.Register(_knockbackLocker);
			Character.CharacterLockedTracker.Register(_knockbackLocker);

			_onCompleteInternal = () => 
			{
				Character.CharacterLockedTracker.Unregister(_knockbackLocker);
				Character.MovementController.MovementBlockers.Unregister(_knockbackLocker);
			};

			StartCoroutine(_knockbackRoutine = KnockbackRoutine(direction, knockbackDuration, knockbackStrength));
		}

		private IEnumerator KnockbackRoutine(Vector3 direction, float knockbackDuration, float knockbackStrength)
		{
			float timer = 0;

			direction = direction.normalized;

			if (knockbackDuration > 0)
			{
				float knockbackStep = knockbackStrength / knockbackDuration;

				while (timer < knockbackDuration)
				{
					timer += Time.deltaTime;
					Vector3 desiredPosition = Character.transform.position + direction * knockbackStep * Time.deltaTime;

					Character.MovementController.Agent.Warp(desiredPosition);

					yield return null;
				}
			}

			StopKnockback();
		}

		private void StopKnockback()
		{
			if(_knockbackRoutine != null)
			{
				StopCoroutine(_knockbackRoutine);
				_knockbackRoutine = null;

				_onCompleteInternal?.Invoke();
				_onCompleteInternal = null;
			}
		}
	}
}