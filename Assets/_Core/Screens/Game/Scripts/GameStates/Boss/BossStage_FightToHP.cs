using RaFSM;
using System;
using UnityEngine;

namespace Screens.Game
{
	public class BossStage_FightToHP : RaGOStateBase<GameBossSceneFSMState>
	{
		[SerializeField, Range(0f, 1f)]
		private float _hpPercentage = 0f; 

		protected override void OnInit()
		{

		}

		protected override void OnEnter()
		{
			Dependency.BossInstance.Health.HealthChangedEvent += OnHealthChangedEvent;
			OnHealthChangedEvent(Dependency.BossInstance.Health);
		}

		protected override void OnExit(bool isSwitch)
		{
			Dependency.BossInstance.Health.HealthChangedEvent -= OnHealthChangedEvent;
		}

		protected override void OnDeinit()
		{

		}

		private void OnHealthChangedEvent(Health obj)
		{
			if(obj.NormalizedValue <= _hpPercentage)
			{
				FSM_GoToNextState();
			}
		}
	}
}