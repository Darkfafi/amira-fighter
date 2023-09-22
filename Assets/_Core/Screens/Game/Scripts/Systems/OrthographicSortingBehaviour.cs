using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameModes.Game
{
	public class OrthographicSortingBehaviour : MonoBehaviour
	{
		private HashSet<OrthographicAgent> _agents = new HashSet<OrthographicAgent>();

		protected void OnEnable()
		{
			StartCoroutine(SortingRoutine());
		}

		protected void OnDisable()
		{
			StopAllCoroutines();
		}

		public void Register(OrthographicAgent agent)
		{
			_agents.Add(agent);
			agent.Refresh();
		}

		public void Unregister(OrthographicAgent agent)
		{
			_agents.Remove(agent);
		}

		protected IEnumerator SortingRoutine()
		{
			while (true)
			{
				foreach (var agent in _agents)
				{
					agent.Refresh();
				}
				yield return new WaitForSeconds(0.1f);
			}
		}
	}
}