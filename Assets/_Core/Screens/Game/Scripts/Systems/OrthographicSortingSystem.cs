using UnityEngine;

namespace Screens.Game
{
	public class OrthographicSortingSystem : GameSystemBase
	{
		private OrthographicSortingBehaviour _behaviour;

		public void Register(OrthographicAgent agent)
		{
			if(_behaviour != null)
			{
				_behaviour.Register(agent);
			}
		}

		public void Unregister(OrthographicAgent agent)
		{
			if(_behaviour != null)
			{
				_behaviour.Unregister(agent);
			}
		}

		protected override void OnSetup()
		{
			_behaviour = new GameObject("<Behaviour>").AddComponent<OrthographicSortingBehaviour>();
			DontDestroyOnLoad(_behaviour);
		}

		protected override void OnStart()
		{

		}

		protected override void OnEnd()
		{
			if (_behaviour != null)
			{
				Destroy(_behaviour.gameObject);
				_behaviour = null;
			}
		}
	}
}