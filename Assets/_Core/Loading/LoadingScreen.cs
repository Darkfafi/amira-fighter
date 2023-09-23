using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RaScenesSO
{
	public class LoadingScreen : RaSceneLoaderGeneric
	{
		protected override Task PostLoadSceneJob(LoadingProcess loadingProcess, CancellationToken token)
		{
			SceneManager.SetActiveScene(SceneModel.CurrentScene.GetUnitySceneData());
			return base.PostLoadSceneJob(loadingProcess, token);
		}
	}
}