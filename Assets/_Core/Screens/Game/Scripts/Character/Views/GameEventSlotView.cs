using RaDataHolder;
using UnityCommands;
using UnityEngine;
using UnityEngine.UI;

namespace Screens.Game
{
	public class GameEventSlotView : RaMonoDataHolderBase<GameEventFSMState>
	{
		[SerializeField]
		private Image _icon = null;

		[Header("State Commands")]
		[SerializeField]
		private UnityCommand _noneStateCommand = null;
		[SerializeField]
		private UnityCommand _isActiveStateCommand = null;
		[SerializeField]
		private UnityCommand _completedStateCommand = null;

		protected override void OnInitialization()
		{
			base.OnInitialization();
			SetActiveState(State.None, force: true);
		}

		protected override void OnSetData()
		{
			_icon.sprite = Data.EventIcon;
		}

		protected override void OnClearData()
		{
			_icon.sprite = null;
		}

		public void SetActiveState(State state, bool force = false)
		{
			UnityCommand.SwitchToCommand(new UnityCommand[] 
			{ 
				_noneStateCommand,
				_isActiveStateCommand,
				_completedStateCommand
			}, default, (int)state, force: force);
		}

		public enum State
		{
			None = 0,
			IsActive = 1,
			Completed = 2,
		}
	}
}