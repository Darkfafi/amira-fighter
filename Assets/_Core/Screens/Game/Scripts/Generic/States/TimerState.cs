using RaFSM;
using System.Collections;
using UnityEngine;

namespace Screens.Game
{
	public class TimerState : RaGOStateBase
	{
		public RaGOStateEvent TimerCompletedEvent;

		[SerializeField]
		private float _duration = 1f;

		private IEnumerator _timerRoutine = null;

		protected override void OnInit()
		{

		}

		protected override void OnEnter()
		{
			StartCoroutine(_timerRoutine = RunTimer());
		}

		protected override void OnExit(bool isSwitch)
		{
			StopCoroutine(_timerRoutine);
			_timerRoutine = null;
		}

		protected override void OnDeinit()
		{

		}

		private IEnumerator RunTimer()
		{
			yield return new WaitForSeconds(_duration);
			TimerCompletedEvent?.Invoke(this);
		}
	}
}