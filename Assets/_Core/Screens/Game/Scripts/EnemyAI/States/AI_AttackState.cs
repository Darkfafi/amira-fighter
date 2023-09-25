using RaProgression;
using System.Collections;
using UnityEngine;

namespace Screens.Game
{
	public class AI_AttackState : EnemyAIStateBase
	{
		private IEnumerator _attackRoutine = null;

		protected override void OnInit()
		{
		}

		protected override void OnEnter()
		{
			StartCoroutine(_attackRoutine = AttackRoutine());
		}

		protected override void OnExit(bool isSwitch)
		{
			EndAttackState();
		}

		protected override void OnDeinit()
		{
		}

		private IEnumerator AttackRoutine()
		{
			while (IsCurrentState)
			{
				yield return null;

				if(Dependency.Target == null)
				{
					continue;
				}

				if(!Dependency.Character.MeleeAttackSkill.CanUse())
				{
					continue;
				}

				Dependency.Character.MovementController.Destination = Dependency.Target.transform.position;

				if(Vector2.Distance(Dependency.Character.transform.position, Dependency.Target.transform.position) < Dependency.Character.MeleeAttackSkill.AttackRadius)
				{
					Dependency.Character.MovementController.Destination = null;
					if (Dependency.Character.CoreSystem.MeleeAttackSkill(Dependency.Character).Execute(Dependency.Character.CoreSystem.Processor, out var result))
					{
						RaProgress attackProgress = result.SkillProgress;
						yield return new WaitUntil(() => attackProgress.HasEnded);
						attackProgress.Dispose();
						EndAttackState();
					}
					else
					{
						EndAttackState();
					}
				}

				yield return new WaitForSeconds(Random.Range(0.1f, 0.15f));
			}
			EndAttackState();
		}

		private void EndAttackState()
		{
			if(_attackRoutine != null)
			{
				StopCoroutine(_attackRoutine);
				_attackRoutine = null;
			}

			if (IsCurrentState)
			{
				Dependency.GoToFormationState();
			}
		}
	}
}