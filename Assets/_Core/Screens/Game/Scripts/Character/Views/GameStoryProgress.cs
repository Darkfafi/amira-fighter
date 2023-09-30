using RaFSM;
using RaProgression;
using System;
using System.Linq;
using UnityEngine;

namespace Screens.Game
{
	public class GameStoryProgress : IDisposable
	{
		public event Action<int, int> EventChangedEvent;

		public GameEventFSMState[] Events
		{
			get; private set;
		}

		public RaProgress Progress
		{
			get; private set;
		}

		public int CurrentEventIndex
		{
			get; private set;
		}

		private RaGOFiniteStateMachine _stateMachine = null;

		public GameStoryProgress(RaGOFiniteStateMachine stateMachine)
		{
			_stateMachine = stateMachine;
			Progress = new RaProgress(markAsStarted: false);
			_stateMachine.SwitchedStateEvent += OnSwitchedStateEvent;
			Events = _stateMachine.States.OfType<GameEventFSMState>().ToArray();
			CurrentEventIndex = -1;

			OnSwitchedStateEvent(_stateMachine.GetCurrentState(), null);
		}

		public void Dispose()
		{
			_stateMachine.SwitchedStateEvent -= OnSwitchedStateEvent;
			Events = null;
			Progress.Dispose();
			CurrentEventIndex = -1;
		}

		private void OnSwitchedStateEvent(RaStateBase<Component> newState, RaStateBase<Component> previousState)
		{
			if(newState is GameEventFSMState newEventState)
			{
				SetCurrentEvent(newEventState);
			}
			else if(previousState is GameEventFSMState)
			{
				SetCurrentEvent(null);
			}
		}

		private void SetCurrentEvent(GameEventFSMState gameEvent)
		{
			int oldIndex = CurrentEventIndex;

			if (gameEvent == null)
			{
				CurrentEventIndex = -1;
			}
			else
			{
				CurrentEventIndex = Array.IndexOf(Events, gameEvent);
			}

			if(CurrentEventIndex == oldIndex)
			{
				return;
			}

			if (CurrentEventIndex >= 0)
			{
				if(Progress.State == RaProgressState.None)
				{
					Progress.Start();
				}

				Progress.Evaluate((float)(CurrentEventIndex + 1) / Events.Length, throwIfNotValid: false);
			}
			else if(Progress.State == RaProgressState.InProgress)
			{
				Progress.Evaluate(1f);
				Progress.Complete();
			}
			else
			{
				Progress.Evaluate(0f, throwIfNotValid: false);
			}

			EventChangedEvent?.Invoke(CurrentEventIndex, oldIndex);
		}
	}
}