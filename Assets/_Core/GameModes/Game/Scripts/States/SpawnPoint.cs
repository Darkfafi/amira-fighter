using UnityEngine;

namespace GameModes.Game
{
	public class SpawnPoint : MonoBehaviour
	{
		[SerializeField]
		private float _radius = 1f;

		[SerializeField]
		private Color _color = Color.white;

		public Vector2 GetSpawnPosition()
		{
			float angle = Mathf.Deg2Rad * Random.Range(0, 360);

			Vector2 returnValue =  transform.position;
			returnValue += new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized * Random.Range(0f, _radius);
			return returnValue;
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = _color;
			Gizmos.DrawWireSphere(transform.position, _radius);
			Gizmos.color = Color.white;
		}
	}
}