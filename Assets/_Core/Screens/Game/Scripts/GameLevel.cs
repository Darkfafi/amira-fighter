using System.Collections.Generic;
using UnityEngine;


namespace Screens.Game
{
	public class GameLevel : MonoBehaviour
	{
		[field: SerializeField]
		public SpawnPoint PlayerSpawn
		{
			get; private set;
		}

		[field: SerializeField]
		public SpawnPoint[] EnemySpawnsLeft
		{
			get; private set;
		}

		[field: SerializeField]
		public SpawnPoint[] EnemySpawnsRight
		{
			get; private set;
		}

		public SpawnPoint GetEnemySpawnPoint(int dir = 0)
		{
			var spawnPoints = GetEnemySpawnPoints(dir);
			return spawnPoints[Random.Range(0, spawnPoints.Count)];
		}

		public List<SpawnPoint> GetEnemySpawnPoints(int dir = 0)
		{
			List<SpawnPoint> spawnPoints = new List<SpawnPoint>();

			if(dir <= 0)
			{
				spawnPoints.AddRange(EnemySpawnsLeft);
			}

			if(dir >= 0)
			{
				spawnPoints.AddRange(EnemySpawnsRight);
			}

			return spawnPoints;
		}
	}
}