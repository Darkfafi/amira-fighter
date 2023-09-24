using System;
using UnityEngine;

namespace Screens.Game
{
	public class EnemyFormation : MonoBehaviour
	{
		[field: SerializeField]
		public Transform[] Points
		{
			get; private set;
		}

		public int GetIndex(Transform point)
		{
			return Array.IndexOf(Points, point);
		}
	}
}
