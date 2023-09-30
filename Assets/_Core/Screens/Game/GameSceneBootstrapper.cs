using Cinemachine;
using RaFSM;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Screens.Game
{
	// IDEA:
	/*
	 * Cinematic intro:
	 * Amira Playing in her room
	 * Zombies conversing its her birthday, and plan an attack
	 * Amira overhears and gets ready for battle
	 * Then, on her birthday, they try to trick her by congratulating her, and then try to attack
	 * She is ready for battle
	 * Start
	 * 
	 * Timeline of wave system, going to the boss battle
	 * Zombies use Boid behaviour to swarm around and try to navigate to Amira
	 */
	public class GameSceneBootstrapper : SceneBootstrapperBase<GameSceneModel>, IRaFSMCycler
	{
		[field: SerializeField]
		public GameLevel Level

		{
			get; private set;
		}

		[field: SerializeField]
		public Transform CameraFollowObject
		{
			get; private set;
		}

		[field: SerializeField]
		public GameHUDView GameHUDView
		{
			get; private set;
		}

		[SerializeField]
		private GameSystemsCollection _gameSystems;

		[field: SerializeField]
		public CinemachineVirtualCamera SkyCamera
		{
			get; private set;
		}

		[SerializeField]
		private float _secondsBeforeStart = 1f;

		public GameStoryProgress StoryProgress
		{
			get; private set;
		}

		public GameCharacterEntity PlayerCharacter
		{
			get; private set;
		}

		private RaGOFiniteStateMachine _gameFSM;
		private GameSystemsController _gameSystemsController;

		public GameSystemsController GameSystems => _gameSystemsController;

		protected override void OnInitialization()
		{
			_gameSystemsController = new GameSystemsController(_gameSystems.GetItems().ToArray());
			_gameSystemsController.Register(this);

			base.OnInitialization();
		}

		protected override void OnSetData()
		{
			_gameFSM = new RaGOFiniteStateMachine(this, RaGOFiniteStateMachine.GetGOStates(transform));
			StoryProgress = new GameStoryProgress(_gameFSM);
		}

		protected override void OnSetDataResolved()
		{
			base.OnSetDataResolved();

			if(GameSystems.CharacterCoreSystem.SpawnCharacter(Data.PlayerCharacterPrefab, Level.PlayerSpawn.GetSpawnPosition(), 1f)
				.Execute(GameSystems.CharacterCoreSystem.Processor, out var result))
			{
				PlayerCharacter = result.CreatedCharacter;
				GameHUDView.SetData(this);
				CameraFollowObject.SetParent(PlayerCharacter.CharacterView.transform, worldPositionStays: false);
				CameraFollowObject.transform.localPosition = Vector3.zero;
			}

			StartCoroutine(Setup());
		}

		protected override void OnClearData()
		{
			StopAllCoroutines();

			GameHUDView.ClearData();

			StoryProgress.Dispose();
			StoryProgress = null;

			_gameSystemsController.Unregister(this);
			_gameFSM.Dispose();
		}

		public void GoToNextState()
		{
			_gameFSM.GoToNextState(wrap: false);
		}

		private IEnumerator Setup()
		{
			yield return new WaitForSeconds(_secondsBeforeStart);

			_gameFSM.SwitchState(0);
			SkyCamera.Priority = 0;
		}
	}
}