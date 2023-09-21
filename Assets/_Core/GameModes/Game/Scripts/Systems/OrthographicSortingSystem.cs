using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GameModes.Game
{
	public class OrthographicSortingSystem : GameSystemBase
	{
		private HashSet<OrthographicAgent> _agents = new HashSet<OrthographicAgent>();
		private CancellationTokenSource _source = null;

		public void Register(OrthographicAgent agent)
		{
			if(IsInitialized)
			{
				_agents.Add(agent);
				agent.Refresh();
			}
		}

		public void Unregister(OrthographicAgent agent)
		{
			if(IsInitialized)
			{
				_agents.Remove(agent);
			}
		}

		private async Task SortingRoutine()
		{
			CancellationToken token = _source.Token;
			while(IsInitialized)
			{
				if(token.IsCancellationRequested)
				{
					break;
				}

				foreach(var agent in _agents)
				{
					agent.Refresh();
				}
				await Task.Delay(10);
			}
		}

		protected override void OnSetup()
		{
			_source = new CancellationTokenSource();
			Task.Run(() => SortingRoutine());
		}

		protected override void OnStart()
		{
		}

		protected override void OnEnd()
		{
			if(_source != null)
			{
				_source.Cancel();
				_source.Dispose();
				_source = null;
			}

			_agents.Clear();
		}
	}
}