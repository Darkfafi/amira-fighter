using UnityEngine;
using UnityEngine.Rendering;

namespace Screens.Game
{
	[RequireComponent(typeof(SortingGroup))]
	public class OrthographicAgent : MonoBehaviour
	{
		[SerializeField]
		private SortingGroup _sortingGroup = null;

		[SerializeField]
		private Canvas _agentCanvas = null;

		[SerializeField]
		private OrthographicSortingSystem _system = null;

		public int SortingOrder
		{
			get; private set;
		}

		private void OnValidate()
		{
			if(_sortingGroup == null)
			{
				_sortingGroup = GetComponent<SortingGroup>();
			}
		}

		protected void Start()
		{
			_system.Register(this);
		}

		protected void OnDestroy()
		{
			_system.Unregister(this);
		}

		public void Refresh()
		{
			SortingOrder = Mathf.RoundToInt(-(transform.position.y) * 100);

			if (_sortingGroup != null)
			{
				_sortingGroup.sortingOrder = SortingOrder;
			}

			if (_agentCanvas != null)
			{
				_agentCanvas.sortingOrder = SortingOrder + 1;
			}
		}
	}
}