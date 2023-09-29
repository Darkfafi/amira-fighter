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

		public override bool CanUse()
		{
			return base.CanUse() && Character.PushMovementController.CanPush() && Character.MovementController.IsMoving;
		}

		protected override void DoPerform(RaProgress progres)
		{
			Vector2 dir = new Vector2(Character.DirectionView.localScale.x, 0);

			if (Character.PushMovementController.Push(dir, Duration, Distance, (isComplete) =>
			{
				progres.Complete();
			}))
			{

			}
		}
	}
}