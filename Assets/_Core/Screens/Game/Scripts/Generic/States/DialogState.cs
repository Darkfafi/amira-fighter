using RaFSM;
using System;
using UnityEngine;

namespace Screens.Game
{
	public interface IDialogCharacterContainer
	{
		GameCharacterEntity GetCharacter(int index);
	}

	public class DialogState : RaGOStateBase
	{
		[SerializeField]
		private DialogEntry[] _dialog;

		[SerializeField]
		private DialogView _dialogView = null;

		private int _currentIndex = 0;

		protected override void OnInit()
		{

		}

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
			DialogEntry entry = _dialog[_currentIndex];
			GameCharacterEntity overrideCharacterEntity = null;

			if (entry.EnableCharacterLookupByIndex)
			{
				overrideCharacterEntity = GetDependency<IDialogCharacterContainer>().GetCharacter(entry.CharacterIndex);
			}

			_dialogView.ShowDialog(_dialog[_currentIndex].CreateData(OnContinueDialog, overrideCharacterEntity));
		}

		protected override void OnDeinit()
		{

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
			[Header("IDialogCharacterContainer")]
			public int CharacterIndex;
			public bool EnableCharacterLookupByIndex;

			[Header("Dialog Options")]
			public GameCharacterEntity Character;

			public bool IsUnskippable;
			public float Duration;

			[TextArea]
			public string DialogText;

			public DialogView.DialogData CreateData(Action callback, GameCharacterEntity overrideCharacter)
			{
				GameCharacterEntity selectedCharacter = Character;

				if(overrideCharacter != null)
				{
					selectedCharacter = overrideCharacter;
				}

				return DialogView.DialogData.Create(DialogText)
					.SetPortrait(selectedCharacter.CharacterView)
					.SetName(selectedCharacter.CharacterName)
					.SetDuration(Duration)
					.SetClickability(!IsUnskippable)
					.SetContinueCallback(callback);
			}
		}
	}
}