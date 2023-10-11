using System;
using System.Collections;
using UnityEngine;

namespace Screens.Game
{
	public class PushMovementController : MonoBehaviour
	{
		[field: SerializeField]
		public GameCharacterEntity Character
		{
			get; private set;
		}

		private IEnumerator _effectRoutine = null;
		private Action _onComplete = null;
		private Action<bool> _onEnd = null;
		private readonly object _effectLockFlag = new object();

		protected void OnDestroy()
		{
			StopPush(isComplete: false);
		}

		public bool CanPush()
		{
			if (Character.CharacterLockedTracker.Flags.Count > 1)
			{
				return false;
			}

			if (Character.CharacterLockedTracker.Flags.Count == 1 &&
				!Character.CharacterLockedTracker.HasFlag(_effectLockFlag))
			{
				return false;
			}

			return true;
		}

		public bool Push(Vector2 direction, float duration, float distance, Action<bool> onEnd = null)
		{
			if(!CanPush())
			{
				return false;
			}

			StopPush(isComplete: false);

			Character.MovementController.MovementBlockers.Register(_effectLockFlag);
			Character.CharacterLockedTracker.Register(_effectLockFlag);

			_onComplete = () => 
			{
				if (Character != null)
				{
					Character.CharacterLockedTracker.Unregister(_effectLockFlag);
					
					if (Character.MovementController != null)
					{
						Character.MovementController.MovementBlockers.Unregister(_effectLockFlag);
					}
				}
			};

			_onEnd = onEnd;

			StartCoroutine(_effectRoutine = PushRoutine(direction, duration, distance));
			return true;
		}

		private IEnumerator PushRoutine(Vector3 direction, float duration, float distance)
		{
			float timer = 0;

			direction = direction.normalized;

			if (duration > 0)
			{
				float step = distance / duration;

				while (timer < duration)
				{
					timer += Time.deltaTime;
					Vector3 desiredPosition = Character.transform.position + direction * step * Time.deltaTime;

					Character.MovementController.Agent.Warp(desiredPosition);

					yield return null;
				}
			}

			StopPush(isComplete: true);
		}

		private void StopPush(bool isComplete)
		{
			if(_effectRoutine != null)
			{
				StopCoroutine(_effectRoutine);
				_effectRoutine = null;

				_onComplete?.Invoke();
				_onComplete = null;

				var onEnd = _onEnd;
				_onEnd = null;
				onEnd?.Invoke(isComplete);
			}
		}
	}
}