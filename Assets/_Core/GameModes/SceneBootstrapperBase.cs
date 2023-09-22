using RaDataHolder;
using RaModelsSO;
using System;
using UnityEngine;

public abstract class SceneBootstrapperBase<TModel> : RaMonoDataHolderBase<TModel>
	where TModel : RaModelSOBase
{
	[field: SerializeField]
	public RaModelSOLocator ModelLocator
	{
		get; private set;
	}

#if UNITY_EDITOR
	protected virtual void OnValidate()
	{
		if(ModelLocator == null)
		{
			ModelLocator = Resources.FindObjectsOfTypeAll<RaModelSOLocator>()[0];
		}
	}
#endif

	protected override void OnInitialization()
	{
		base.OnInitialization();
		SetData(ModelLocator.GetModelSO<TModel>(), resolve: false);
		Invoke(nameof(StartScene), 0.1f);
	}

	private void StartScene()
	{
		Resolve();
	}
}
