using RaProgression;
using System;
using UnityEngine;

namespace Screens.Game
{
	public abstract class CharacterSkillBase : MonoBehaviour
	{
		[field: SerializeField]
		public Sprite IconSprite
		{
			get; private set;
		}

		[field: SerializeField]
		public GameCharacterEntity Character
		{
			get; private set;
		}

		[field: SerializeField]
		public float CooldownTimeInSeconds
		{
			get; private set;
		}

		public RaProgress SkillCooldownProgress
		{
			get; private set;
		} = new RaProgress(markAsStarted: false);

		public RaProgress SkillUseProgress
		{
			get; private set;
		} = new RaProgress(markAsStarted: false);

		public float CooldownTimeProgressed
		{
			get; private set;
		}

		public virtual bool CanUse() => SkillCooldownProgress.State != RaProgressState.InProgress && SkillUseProgress.State != RaProgressState.InProgress;

		protected void Update()
		{
			if(SkillCooldownProgress.State == RaProgressState.InProgress)
			{
				CooldownTimeProgressed += Time.deltaTime;
				SkillCooldownProgress.Evaluate(CooldownTimeProgressed / CooldownTimeInSeconds);

				if(Mathf.Approximately(1f, SkillCooldownProgress.NormalizedValue))
				{
					SkillCooldownProgress.Complete();
				}
			}
		}

		public bool Use()
		{
			if(!CanUse())
			{
				return false;
			}

			SkillUseProgress.Reset(throwIfNotValid: false);
			SkillUseProgress.Start(throwIfNotValid: false);
			
			SkillUseProgress.ProgressCompletedEvent += OnCompletedEvent;
			SkillUseProgress.ProgressCancelledEvent += OnCancelledEvent;

			DoPerform(SkillUseProgress);
			return true;
		}

		protected abstract void DoPerform(RaProgress progres);

		private void OnCancelledEvent(IRaProgress progress)
		{
			SkillUseProgress.ProgressCompletedEvent -= OnCompletedEvent;
			SkillUseProgress.ProgressCancelledEvent -= OnCancelledEvent;
			OnCancelled();
			OnEnded();
		}

		private void OnCompletedEvent(IRaProgress progress)
		{
			SkillUseProgress.ProgressCompletedEvent -= OnCompletedEvent;
			SkillUseProgress.ProgressCancelledEvent -= OnCancelledEvent;
			StartCooldown();
			OnCompleted();
			OnEnded();
		}

		private void StartCooldown()
		{
			SkillCooldownProgress.Reset(throwIfNotValid: false);
			SkillCooldownProgress.Start(throwIfNotValid: false);
			SkillCooldownProgress.ProgressCompletedEvent += OnCooldownCompletedEvent;
			CooldownTimeProgressed = 0f;
			OnStartCooldown();
		}

		private void OnCooldownCompletedEvent(IRaProgress progress)
		{
			SkillCooldownProgress.ProgressCompletedEvent -= OnCooldownCompletedEvent;
			CooldownTimeProgressed = 0f;
			OnCompletedCooldown();
		}

		protected virtual void OnStartCooldown()
		{
		}

		protected virtual void OnCompletedCooldown()
		{
		}

		protected virtual void OnCompleted()
		{

		}

		protected virtual void OnCancelled()
		{

		}

		protected virtual void OnEnded()
		{

		}
	}
}