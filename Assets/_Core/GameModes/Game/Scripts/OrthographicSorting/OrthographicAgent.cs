using UnityEngine;
using UnityEngine.Rendering;

namespace GameModes.Game
{
	[RequireComponent(typeof(SortingGroup))]
	public class OrthographicAgent : MonoBehaviour
	{
		[SerializeField]
		private SortingGroup _sortingGroup = null;

		private void OnValidate()
		{
			if(_sortingGroup == null)
			{
				_sortingGroup = GetComponent<SortingGroup>();
			}
		}

		protected void OnEnable()
		{
			OrthographicSortingSystem.Register(this);
		}

		protected void OnDisable()
		{
			OrthographicSortingSystem.Unregister(this);
		}

		protected void OnDestroy()
		{
			OrthographicSortingSystem.Unregister(this);
		}

		public void Refresh()
		{
			_sortingGroup.sortingOrder = Mathf.RoundToInt(-(transform.position.y) * 100);
		}
	}
}