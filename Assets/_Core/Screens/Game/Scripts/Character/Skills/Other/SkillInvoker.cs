using System.Collections;
using UnityEngine;

namespace Screens.Game
{
	public class SkillInvoker : MonoBehaviour
	{
		[SerializeField]
		private CharacterSkillBase _characterSkill;

		[SerializeField]
		private Vector2 _invokeRange = Vector2.one;

		private IEnumerator _routine = null;

		protected void OnEnable()
		{
			StartCoroutine(_routine = SkillRoutine());
		}

		private void OnDisable()
		{
			if(_routine != null)
			{
				StopCoroutine(_routine);
				_routine = null;
			}
		}

		private IEnumerator SkillRoutine()
		{
			while(true)
			{
				yield return new WaitForSeconds(Random.Range(_invokeRange.x, _invokeRange.y));

				if(_characterSkill.Character.IsCharacterLocked)
				{
					continue;
				}

				_characterSkill.Character.CoreSystem.UseSkill(_characterSkill)
					.Execute(_characterSkill.Character.CoreSystem.Processor);
			}
		}
	}
}