using UnityEngine;

namespace Screens.Game
{
	public class SpawnPoint : MonoBehaviour
	{
		[SerializeField]
		private float _radius = 1f;

		[SerializeField]
		private Color _color = Color.white;

		public static Vector2 GetSpawnPosition(Vector3 center, float radius)
		{
			float angle = Mathf.Deg2Rad * Random.Range(0, 360);
			Vector2 returnValue = center;
			returnValue += new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized * Random.Range(0f, radius);
			return returnValue;
		}

		public Vector2 GetSpawnPosition()
		{
			return GetSpawnPosition(transform.position, _radius);
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = _color;
			Gizmos.DrawWireSphere(transform.position, _radius);
			Gizmos.color = Color.white;
		}
	}
}