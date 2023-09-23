using RaFSM;
using RaTweening;
using UnityEngine;

namespace Screens.Menu
{
	public class MenuToGameTransitionState : RaGOStateBase<MenuSceneBootstrapper>
	{
		[SerializeField]
		private float _transitionSpeed = 10f;

		[SerializeField]
		private AnimationCurve _transitionEase = AnimationCurve.Linear(0, 0, 1f, 1f);

		protected override void OnInit()
		{

		}

		protected override void OnEnter()
		{
			float destinationY = Dependency.WorldContainer.sizeDelta.y - Dependency.WorldBottomContainer.sizeDelta.y;
			float distance = Mathf.Abs(destinationY - Dependency.WorldContainer.anchoredPosition.y);
			float duration = Mathf.Max(1f , distance) / _transitionSpeed;
			Dependency.WorldContainer.TweenAnchorPosY(destinationY, duration).SetEasing(_transitionEase).OnComplete(() => 
			{
				GetDependency<IRaFSMCycler>().GoToNextState();
			});
		}

		protected override void OnExit(bool isSwitch)
		{

		}

		protected override void OnDeinit()
		{

		}
	}
}