﻿using RaDataHolder;
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
		}

		protected override void OnClearData()
		{
			_nameLabel.text = string.Empty;
			_healthView.ClearData();
			_characterPortraitView.ClearData();
			_container.SetActive(false);
		}
	}
}