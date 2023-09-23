using RaFSM;
using System.Collections;
using UnityEngine;

namespace Screens.Menu
{
	public class MenuSceneBootstrapper : SceneBootstrapperBase<MenuSceneModel>, IRaFSMCycler
	{
		[SerializeField]
		private RaGOFiniteStateMachine _menuFSM = null;

		[SerializeField]
		private Transform _statesRoot = null;

		[SerializeField]
		private float _minSkyHeight = 4000;

		[SerializeField]
		private RectTransform _bottomPartRectTransform = null;

		[field: SerializeField]
		public RectTransform WorldContainer
		{
			get; private set;
		}

		public RectTransform WorldBottomContainer => _bottomPartRectTransform;

		public void EditorOnPlayPressed()
		{
			if (_menuFSM.CurrentStateIndex == 0)
			{
				GoToNextState();
			}
		}

		protected override void OnSetData()
		{
			_menuFSM = new RaGOFiniteStateMachine(this, RaGOFiniteStateMachine.GetGOStates(_statesRoot));
		}

		protected override void OnSetDataResolved()
		{
			base.OnSetDataResolved();
			StartCoroutine(Setup());
		}

		protected override void OnClearData()
		{
			StopAllCoroutines();
			_menuFSM.Dispose();
		}

		private IEnumerator Setup()
		{
			yield return null;
			Canvas.ForceUpdateCanvases();
			Vector2 size = ((RectTransform)WorldContainer.parent.transform).rect.size;
			_bottomPartRectTransform.sizeDelta = size;
			size.y = Mathf.Max(_minSkyHeight, WorldContainer.sizeDelta.y + size.y * 2);
			WorldContainer.sizeDelta = size;
			_menuFSM.SwitchState(0);
		}

		public void GoToNextState()
		{
			_menuFSM.GoToNextState(wrap: false);
		}
	}
}