using RaFSM;
using UnityEngine;

namespace GameModes.Menu
{
	public class MenuSceneBootstrapper : SceneBootstrapperBase<MenuSceneModel>
	{
		[SerializeField]
		private RaGOFiniteStateMachine _menuFSM = null;

		[SerializeField]
		private Transform _statesRoot = null;

		protected override void OnSetData()
		{
			_menuFSM = new RaGOFiniteStateMachine(this, RaGOFiniteStateMachine.GetGOStates(_statesRoot));
		}

		protected override void OnSetDataResolved()
		{
			base.OnSetDataResolved();
			_menuFSM.SwitchState(0);
		}

		protected override void OnClearData()
		{
			_menuFSM.Dispose();
		}
	}
}