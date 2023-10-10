using UnityEngine;

namespace Screens.Game
{
	public class DamageBox : MonoBehaviour
	{
		[SerializeField]
		private Rect _damageRect = new Rect(0, 0, 1, 1);

		[SerializeField]
		private float _knockbackDuration = 0.15f;

		[SerializeField]
		private float _knockbackStrength = 1.15f;

		[SerializeField]
		private bool _hitOnEnable = true;

		private Vector2 GetPosition() => new Vector2(transform.position.x, transform.position.y) + _damageRect.position;

		protected void OnEnable()
		{
			if (_hitOnEnable)
			{
				DoDamage();
			}
		}

		public void DoDamage()
		{
			Collider2D[] hits = Physics2D.OverlapBoxAll(GetPosition(), _damageRect.size, 0f);

			for (int i = 0, c = hits.Length; i < c; i++)
			{
				var hit = hits[i];

				if (hit.tag == "MainCollider" &&
					hit.transform.root.TryGetComponent(out GameCharacterEntity entity))
				{
					if (entity.Health.Damage(1))
					{
						Vector2 delta = entity.transform.position - transform.position;
						entity.PushMovementController.Push(delta.normalized, _knockbackDuration, _knockbackStrength);
					}
				}
			}
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireCube(GetPosition(), _damageRect.size);
			Gizmos.color = Color.white;
		}
	}
}