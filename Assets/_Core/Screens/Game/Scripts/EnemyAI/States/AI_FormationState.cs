using System.Collections;
using UnityEngine;

namespace Screens.Game
{
	public class AI_FormationState : EnemyAIStateBase
	{
		private IEnumerator _formationRoutine = null;

		protected override void OnInit()
		{
		}

		protected override void OnEnter()
		{
			StartCoroutine(_formationRoutine = FormationRoutine());
		}

		protected override void OnExit(bool isSwitch)
		{
			StopCoroutine(_formationRoutine);
			_formationRoutine = null;

			Dependency.Character.MovementController.Destination = null;
		}

		protected override void OnDeinit()
		{
		}

		private IEnumerator FormationRoutine()
		{
			while (IsCurrentState)
			{
				if (Dependency.CurrentFormationPoint != null)
				{
					Dependency.Character.MovementController.Destination = Dependency.CurrentFormationPoint.position;

					if(!Dependency.Character.MovementController.IsMoving)
					{
						Vector3 delta = Dependency.CurrentFormationPoint.parent.position - Dependency.Character.transform.position;
						Dependency.Character.SetLookDirection(delta.x);
					}
				}
				else
				{
					Dependency.Character.MovementController.Destination = null;
				}

				yield return new WaitForSeconds(Random.Range(0.25f, 0.5f));
			}
		}
	}
}