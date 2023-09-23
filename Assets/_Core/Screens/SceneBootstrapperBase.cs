using RaDataHolder;
using RaModelsSO;
using RaScenesSO;
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

		RaSceneModelSO sceneModelSO = ModelLocator.GetModelSO<RaSceneModelSO>();
		if(sceneModelSO.IsLoading)
		{
			sceneModelSO.SceneLoadEndedEvent += OnSceneLoadedEvent;
		}
		else
		{
			OnSceneLoadedEvent(sceneModelSO.CurrentScene);
		}
	}

	private void OnSceneLoadedEvent(RaSceneSO scene)
	{
		ModelLocator.GetModelSO<RaSceneModelSO>().SceneLoadEndedEvent -= OnSceneLoadedEvent;
		SetData(ModelLocator.GetModelSO<TModel>());
	}
}
