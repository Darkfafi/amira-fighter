using RaDataHolder;
using RaTweening;
using UnityEngine;

namespace Screens.Game
{
	public class GameCharacterUIView : RaMonoDataHolderBase<GameCharacterEntity>
	{
		[field: SerializeField]
		public HealthView Healthbar
		{
			get; private set;
		}

		[SerializeField]
		private CanvasGroup _healthbarCanvasGroup = null;

		protected override void OnInitialization()
		{
			base.OnInitialization();
			_healthbarCanvasGroup.alpha = 0f;
		}

		protected override void OnSetData()
		{
			Healthbar.SetData(Data.Health);
			Data.Health.HealthChangedEvent += OnHealthChangedEvent;
		}

		protected override void OnClearData()
		{
			Data.Health.HealthChangedEvent -= OnHealthChangedEvent;
			Healthbar.ClearData();
		}

		private void OnHealthChangedEvent(Health health)
		{
			RaTweenBase.StopGroup(_healthbarCanvasGroup);
			_healthbarCanvasGroup.alpha = 1;
			_healthbarCanvasGroup.TweenAlpha(0f, 0.5f).SetDelay(1f).SetGroup(_healthbarCanvasGroup);
		}
	}
}