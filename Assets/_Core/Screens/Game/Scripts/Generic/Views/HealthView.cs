using RaDataHolder;
using UnityEngine;

namespace Screens.Game
{
	public class HealthView : RaMonoDataHolderBase<Health>
	{
		[SerializeField]
		protected Transform _hpFill = null;

		protected override void OnInitialization()
		{
			base.OnInitialization();
			SetFill(0);
		}

		protected override void OnSetData()
		{
			Data.HealthChangedEvent += OnHealthChangedEvent;
			SetFill(Data.NormalizedValue);
		}

		protected override void OnClearData()
		{
			Data.HealthChangedEvent -= OnHealthChangedEvent;
			SetFill(0);
		}

		private void OnHealthChangedEvent(Health health)
		{
			SetFill(health.NormalizedValue);
		}

		private void SetFill(float normalizedValue)
		{
			Vector3 fillScale = _hpFill.localScale;
			fillScale.x = normalizedValue;
			_hpFill.localScale = fillScale;
		}

	}
}