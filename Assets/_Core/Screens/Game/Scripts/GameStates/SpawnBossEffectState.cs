using Cinemachine;
using RaFSM;
using System.Collections;
using UnityEngine;

namespace Screens.Game
{
	public class SpawnBossEffectState : RaGOStateBase<GameBossSceneFSMState>
	{
		[SerializeField]
		private ParticleSystem _effect = null;

		[SerializeField]
		private float _secondsUntilEffectStart = 0.5f;

		[SerializeField]
		private float _secondsUntilEffectStartToSpawn = 0.5f;

		[SerializeField]
		private float _secondsSpawnToEffectStop = 0.5f;

		[SerializeField]
		private float _secondsUntilEffectStopToEnd = 1f;

		[Header("Audio")]
		[SerializeField]
		private AudioSource _sfxSource = null;

		[SerializeField]
		private AudioClip _showPortalSound = null;

		private IEnumerator _routine = null;

		protected override void OnInit()
		{

		}

		protected override void OnEnter()
		{
			Dependency.HideBoss();
			StartCoroutine(_routine = CinematicRoutine());
		}

		protected override void OnExit(bool isSwitch)
		{
			_effect.Stop();

			if (_routine != null)
			{
				StopCoroutine(_routine);
				_routine = null;
			}
		}

		protected override void OnDeinit()
		{
		}

		private IEnumerator CinematicRoutine()
		{
			yield return new WaitForSeconds(_secondsUntilEffectStart);
			
			_effect.Play();
			_sfxSource.PlayOneShot(_showPortalSound);

			yield return new WaitForSeconds(_secondsUntilEffectStartToSpawn);

			Dependency.ShowBoss();

			yield return new WaitForSeconds(_secondsSpawnToEffectStop);

			_effect.Stop();

			yield return new WaitForSeconds(_secondsUntilEffectStopToEnd);

			FSM_GoToNextState();
		}
	}
}