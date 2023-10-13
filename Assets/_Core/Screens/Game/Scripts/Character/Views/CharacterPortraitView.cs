using Assets.HeroEditor.Common.Scripts.CharacterScripts;
using HeroEditor.Common;
using RaDataHolder;
using UnityEngine;

namespace Screens.Game
{
	public class CharacterPortraitView : RaMonoDataHolderBase<string>
	{
		[SerializeField]
		private CharacterBase _character = null;

		[SerializeField]
		private GameCharacterEntity _initGameCharacter = null;

		protected override void OnInitialization()
		{
			base.OnInitialization();
			_character.gameObject.SetActive(false);

			if(_initGameCharacter != null)
			{
				SetData(_initGameCharacter.CharacterView.ToJson());
			}
		}

		protected override void OnSetData()
		{
			_character.gameObject.SetActive(true);
			_character.FromJson(Data);
		}

		protected override void OnClearData()
		{
			_character.gameObject.SetActive(false);
		}
	}
}