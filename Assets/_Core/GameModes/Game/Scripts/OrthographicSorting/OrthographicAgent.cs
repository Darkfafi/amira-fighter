﻿using UnityEngine;
using UnityEngine.Rendering;

namespace GameModes.Game
{
	[RequireComponent(typeof(SortingGroup))]
	public class OrthographicAgent : MonoBehaviour
	{
		[SerializeField]
		private SortingGroup _sortingGroup = null;

		[SerializeField]
		private OrthographicSortingSystem _system = null;

		private void OnValidate()
		{
			if(_sortingGroup == null)
			{
				_sortingGroup = GetComponent<SortingGroup>();
			}
		}

		protected void OnEnable()
		{
			_system.Register(this);
		}

		protected void OnDisable()
		{
			_system.Unregister(this);
		}

		protected void OnDestroy()
		{
			_system.Unregister(this);
		}

		public void Refresh()
		{
			_sortingGroup.sortingOrder = Mathf.RoundToInt(-(transform.position.y) * 100);
		}
	}
}