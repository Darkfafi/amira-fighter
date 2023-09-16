using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameModes.Game
{
	public class OrthographicSortingSystem : MonoBehaviour
	{
		public static bool IsAvailable => Instance != null;

		public static OrthographicSortingSystem Instance
		{
			get
			{
				if(_instance == null)
				{
					_instance = FindObjectOfType<OrthographicSortingSystem>();
				}
				return _instance;
			}
		}

		private HashSet<OrthographicAgent> _agents = new HashSet<OrthographicAgent>();

		public static void Register(OrthographicAgent agent)
		{
			if(IsAvailable)
			{
				Instance._agents.Add(agent);
				Instance.name = $"{nameof(OrthographicSortingSystem)} [{Instance._agents.Count}]";
				agent.Refresh();
			}
		}

		public static void Unregister(OrthographicAgent agent)
		{
			if (IsAvailable)
			{
				Instance._agents.Remove(agent);
				Instance.name = $"{nameof(OrthographicSortingSystem)} [{Instance._agents.Count}]";
			}
		}

		private static OrthographicSortingSystem _instance = null;

		protected void OnEnable()
		{
			StartCoroutine(SortingRoutine());
		}

		protected void OnDisable()
		{
			StopAllCoroutines();
		}

		protected void OnDestroy()
		{
			StopAllCoroutines();
			_agents.Clear();
		}

		private IEnumerator SortingRoutine()
		{
			while(true)
			{
				foreach(var agent in _agents)
				{
					agent.Refresh();
				}
				yield return new WaitForSeconds(0.1f);
			}
		}
	}
}