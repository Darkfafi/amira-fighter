using RaFSM;
using UnityEngine;
using Cinemachine;

namespace Screens.Game
{
	public class GameIntroSpeachState : RaGOFSMState<GameIntroSceneFSMState>
	{
		[SerializeField]
		private CinemachineVirtualCamera _speachVirtualCamera = null;

		protected override void OnInit()
		{
			base.OnInit();
			_speachVirtualCamera.Priority = 0;
		}

		protected override void OnEnter()
		{
			base.OnEnter();
			_speachVirtualCamera.Priority = int.MaxValue;
		}

		protected override void OnExit(bool isSwitch)
		{
			base.OnExit(isSwitch);
			_speachVirtualCamera.Priority = 0;
		}

		protected override void OnDeinit()
		{
			base.OnDeinit();
		}
	}
}