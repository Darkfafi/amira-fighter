using RaCollection;
using RaDataHolder;
using RaTweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Screens.Game
{
	public class GameProgressHUDView : RaMonoDataHolderBase<GameProgressHUDView.CoreData>
	{
		[SerializeField]
		private Transform _eventSlotsContainer = null;

		[SerializeField]
		private GameEventSlotView _slotPrefab = null;

		[SerializeField]
		private Image _progressFill = null;

		[SerializeField]
		private Transform _characterSlot = null;

		[SerializeField]
		private CharacterPortraitView _characterPortraitView = null;

		private List<GameEventSlotView> _slotViews = new List<GameEventSlotView>();

		protected override void OnInitialization()
		{
			base.OnInitialization();
			_progressFill.transform.localScale = new Vector3(0f, 1f, 1f);
		}

		protected override void OnSetData()
		{
			_characterPortraitView.SetData(Data.GameCharacter.CharacterView.ToJson());

			Data.StoryProgress.EventChangedEvent += OnEventChangedEvent;
			for(int i = 0; i < Data.StoryProgress.Events.Length; i++)
			{
				var gameEvent = Data.StoryProgress.Events[i];
				var slot = Instantiate(_slotPrefab, _eventSlotsContainer);
				_slotViews.Add(slot);
				slot.SetData(gameEvent);
			}
			OnEventChangedEvent(Data.StoryProgress.CurrentEventIndex, -1);
		}

		protected void Update()
		{
			if (HasData)
			{
				_characterPortraitView.transform.position = _characterSlot.transform.position;
			}
		}

		protected override void OnClearData()
		{
			_slotViews.ForEachReverse(x => Destroy(x.gameObject));
			_slotViews.Clear();

			_slotViews.Clear();

			_characterPortraitView.ClearData();

			Data.StoryProgress.EventChangedEvent -= OnEventChangedEvent;
		}

		private void OnEventChangedEvent(int newEventIndex, int oldEventIndex)
		{
			for(int i = 0; i <= oldEventIndex; i++)
			{
				_slotViews[i].SetActiveState(GameEventSlotView.State.Completed);
			}

			oldEventIndex = oldEventIndex + 1;

			for (int i = oldEventIndex; i <= newEventIndex; i++)
			{
				_slotViews[i].SetActiveState(GameEventSlotView.State.IsActive);
			}

			_progressFill.transform.TweenScaleX(Data.StoryProgress.Progress.NormalizedValue, 0.45f);
		}

		public struct CoreData
		{
			public GameStoryProgress StoryProgress;
			public GameCharacterEntity GameCharacter;
		}
	}
}