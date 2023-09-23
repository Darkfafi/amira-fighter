using RaFSM;
using RaModelsSO;
using RaScenesSO;
using UnityEngine;

public class LoadSceneState : RaGOStateBase
{
	[SerializeField]
	private RaSceneSO _sceneToLoad = null;

	[SerializeField]
	private RaModelSOLocator _models = null;

	protected override void OnInit()
	{

	}

	protected override void OnEnter()
	{
		_models.GetModelSO<RaSceneModelSO>().LoadScene(_sceneToLoad);
	}

	protected override void OnExit(bool isSwitch)
	{

	}

	protected override void OnDeinit()
	{

	}
}