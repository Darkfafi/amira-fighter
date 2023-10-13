using Assets.HeroEditor.Common.Scripts.Common;
using RaProgression;
using UnityEngine;

namespace Screens.Game
{
	public class DashAttackSkill : CharacterSkillBase
	{
		[field: SerializeField]
		public float Distance
		{
			get; private set;
		}

		[field: SerializeField]
		public float Duration
		{
			get; private set;
		}

		[SerializeField]
		private TrailRenderer _dashTrail = null;

		[Header("Audio")]
		[SerializeField]
		private AudioSource _combatSFXSource = null;

		[SerializeField]
		private GameSound[] _dashSounds = null;

		public override bool CanUse()
		{
			return base.CanUse() && Character.PushMovementController.CanPush() && Character.MovementController.IsMoving;
		}

		protected void Awake()
		{
			_dashTrail.emitting = false;
		}

		protected override void DoPerform(RaProgress progres)
		{
			Vector2 dir = new Vector2(Character.DirectionView.localScale.x, 0);

			if (Character.PushMovementController.Push(dir, Duration, Distance, (isComplete) =>
			{
				_dashTrail.emitting = false;
				progres.Complete();
			}))
			{
				_combatSFXSource.PlayRandomOneShot(_dashSounds);
				_dashTrail.sortingOrder = Character.OrthographicAgent.SortingOrder - 100;
				_dashTrail.emitting = true;
			}
		}
	}
}