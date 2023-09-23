using HeroEditor.Common;
using RaDataHolder;
using UnityEngine;

namespace Screens.Game
{
	public class CharacterPortraitView : RaMonoDataHolderBase<string>
	{
		[SerializeField]
		private CharacterBase _character = null;

		protected override void OnInitialization()
		{
			base.OnInitialization();
			_character.gameObject.SetActive(false);
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