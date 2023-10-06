using RaFSM;
using System.Collections;
using UnityEngine;

namespace Screens.Game
{
	public class BossStage_Rain : RaGOStateBase<GameBossSceneFSMState>
	{
		[SerializeField]
		private GameObject _objectToRain = null;

		[SerializeField]
		private float _spawnRadius = 0.5f;

		[SerializeField]
		private Vector2 _spawnDelayRangeInSeconds = Vector2.one;

		[SerializeField]
		private int _spawnAmount = 1;

		[SerializeField]
		private float _secondsUntilEndAfterLastSpawn = 2;

		private IEnumerator _routine = null;

		protected override void OnInit()
		{

		}

		protected override void OnEnter()
		{
			StartCoroutine(_routine = SpawnRoutine());
		}

		protected override void OnExit(bool isSwitch)
		{
			if(_routine != null)
			{
				StopCoroutine(_routine);
				_routine = null;
			}
		}

		protected override void OnDeinit()
		{

		}

		private IEnumerator SpawnRoutine()
		{
			int count = 0;
			while(count < _spawnAmount)
			{
				yield return new WaitForSeconds(Random.Range(_spawnDelayRangeInSeconds.x, _spawnDelayRangeInSeconds.y));
				Vector2 spawnPosition = SpawnPoint.GetSpawnPosition(Dependency.PlayerInstance.transform.position, _spawnRadius);
				Instantiate(_objectToRain, spawnPosition, Quaternion.identity);
				count++;
			}

			yield return new WaitForSeconds(_secondsUntilEndAfterLastSpawn);

			FSM_GoToNextState();

			_routine = null;
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(transform.position, _spawnRadius);
			Gizmos.color = Color.white;
		}
	}
}