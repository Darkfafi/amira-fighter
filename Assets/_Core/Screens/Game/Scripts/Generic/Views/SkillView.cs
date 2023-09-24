using RaDataHolder;
using UnityEngine;
using UnityEngine.UI;

namespace Screens.Game
{
	public class SkillView : RaMonoDataHolderBase<CharacterSkillBase>
	{
		[SerializeField]
		private GameObject _container = null;

		[SerializeField]
		private RectTransform _cooldownFill = null;

		[SerializeField]
		private Image _icon = null;

		protected override void OnInitialization()
		{
			base.OnInitialization();
			_container.SetActive(false);
		}

		protected override void OnSetData()
		{
			_container.SetActive(true);
			_icon.sprite = Data.IconSprite;
		}

		protected void Update()
		{
			if(HasData)
			{
				float value = 0f;
				if (!Data.CanUse())
				{
					if(Data.SkillCooldownProgress.State == RaProgression.RaProgressState.InProgress)
					{
						value = 1f - Data.SkillCooldownProgress.NormalizedValue;
					}
					else
					{
						value = 1f;
					}
				}

				Vector3 scale = _cooldownFill.localScale;
				scale.y = value;
				_cooldownFill.localScale = scale;
			}
		}

		protected override void OnClearData()
		{
			_container.SetActive(false);
		}
	}
}