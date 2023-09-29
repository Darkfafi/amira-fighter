using RaCollection;
using RaDataHolder;
using TMPro;
using UnityEngine;

namespace Screens.Game
{
	public class CharacterHUDView : RaMonoDataHolderBase<GameCharacterEntity>
	{
		[SerializeField]
		private CharacterPortraitView _characterPortraitView = null;

		[SerializeField]
		private HealthView _healthView = null;

		[SerializeField]
		private TMP_Text _nameLabel = null;

		[SerializeField]
		private GameObject _container = null;

		[SerializeField]
		private SkillView[] _skillViews = null; 

		protected override void OnInitialization()
		{
			base.OnInitialization();
			_container.SetActive(false);
		}

		protected override void OnSetData()
		{
			_container.SetActive(true);
			_nameLabel.text = Data.CharacterName;
			_healthView.SetData(Data.Health);
			_characterPortraitView.SetData(Data.CharacterView.ToJson());

			for(int i = 0; i < _skillViews.Length; i++)
			{
				CharacterSkillBase skill = ((i <= Data.AllSkills.Length - 1) ? Data.AllSkills[i] : null);
				_skillViews[i].ReplaceData(skill);
			}
		}

		protected override void OnClearData()
		{
			_nameLabel.text = string.Empty;
			_skillViews.ForEachReverse(x => x.ClearData());
			_healthView.ClearData();
			_characterPortraitView.ClearData();
			_container.SetActive(false);
		}
	}
}