using RaFSM;
using System;
using UnityEngine;

namespace Screens.Game
{
	public class DialogState : RaGOFSMState
	{
		[SerializeField]
		private DialogEntry[] _dialog;

		[SerializeField]
		private DialogView _dialogView = null;

		private int _currentIndex = 0;

		protected override void OnEnter()
		{
			_currentIndex = 0;
			ShowDialog();
		}

		protected override void OnExit(bool causedBySwitch)
		{
			if (_dialogView != null)
			{
				_dialogView.HideDialog();
			}
		}

		private void ShowDialog()
		{
			_dialogView.ShowDialog(_dialog[_currentIndex].CreateData(OnContinueDialog));
		}

		private void OnContinueDialog()
		{
			_currentIndex++;
			if (_currentIndex >= _dialog.Length)
			{
				GetDependency<IRaFSMCycler>().GoToNextState();
			}
			else
			{
				ShowDialog();
			}
		}

		[Serializable]
		private struct DialogEntry
		{
			public GameCharacterEntity Character;

			public string NameOverride;

			[TextArea]
			public string DialogText;

			public DialogView.DialogData CreateData(Action callback)
			{
				return DialogView.DialogData.Create(DialogText)
					.SetPortrait(Character.CharacterView)
					.SetName(string.IsNullOrEmpty(NameOverride) ? Character.CharacterName : NameOverride)
					.SetContinueCallback(callback);
			}
		}
	}
}