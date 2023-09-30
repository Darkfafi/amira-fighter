using RaCollection;
using RaDataHolder;
using RaFlags;
using RaTweening;
using UnityEngine;

namespace Screens.Game
{
	public class GameHUDView : RaMonoDataHolderBase<GameSceneBootstrapper>
	{
		[field: SerializeField]
		public CharacterHUDView CharacterHUDView
		{
			get; private set;
		}

		[field: SerializeField]
		public GameProgressHUDView GameProgressHUDView
		{
			get; private set;
		}

		public RaFlagsTracker HideFlags
		{
			get; private set;
		}

		[SerializeField]
		private CanvasGroup _canvasGroup = null;

		[SerializeField]
		private NestedFadeGroup.NestedFadeGroup[] _nestedFadeGroups = null;

		public bool IsShowing
		{
			get; private set;
		}

		protected void OnValidate()
		{
			_nestedFadeGroups = GetComponentsInChildren<NestedFadeGroup.NestedFadeGroup>();
		}

		protected override void OnInitialization()
		{
			base.OnInitialization();
			HideFlags = new RaFlagsTracker(OnHideFlagsChangedEvent);
			IsShowing = false;
			SetAlpha(0f);
		}

		protected override void OnSetData()
		{
			CharacterHUDView.SetData(Data.PlayerCharacter);
			
			GameProgressHUDView.SetData(new GameProgressHUDView.CoreData()
			{
				GameCharacter = Data.PlayerCharacter,
				StoryProgress = Data.StoryProgress
			});
		}

		protected override void OnSetDataResolved()
		{
			base.OnSetDataResolved();
			OnHideFlagsChangedEvent(HideFlags.IsEmpty(), HideFlags);
		}

		protected override void OnClearData()
		{
			GameProgressHUDView.ClearData();
			CharacterHUDView.ClearData();
		}

		private void OnHideFlagsChangedEvent(bool isEmpty, RaFlagsTracker tracker)
		{
			if(isEmpty == IsShowing)
			{
				return;
			}

			IsShowing = isEmpty;

			RaTweenLambda.TweenFloat(IsShowing ? 0f : 1f, IsShowing ? 1f : 0f, 0.5f, (v, n) =>
			{
				SetAlpha(v);
			});
		}

		private void SetAlpha(float v)
		{
			_canvasGroup.alpha = v;
			_nestedFadeGroups.ForEach(g => g.AlphaSelf = v);
		}
	}
}