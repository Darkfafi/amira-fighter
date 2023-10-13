using Cinemachine;
using RaFSM;
using RaProgression;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Screens.Game
{
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

		[SerializeField]
		private AudioSource _musicAudioSource = null;

		[Header("State References")]
		[SerializeField]
		private RaGOStateBase _loseState = null;

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
				PlayerCharacter.Health.HealthChangedEvent += OnPlayerHealthChangedEvent;
				CameraFollowObject.SetParent(PlayerCharacter.CharacterView.transform, worldPositionStays: false);
				CameraFollowObject.transform.localPosition = Vector3.zero;

				GameHUDView.SetData(this, resolve: false);
			}

			StartCoroutine(Setup());
		}

		public void LockPlayer(object flag)
		{
			if (PlayerCharacter != null)
			{
				PlayerCharacter.CharacterLockedTracker.Register(flag);
			}
		}

		public void UnlockPlayer(object flag)
		{
			if (PlayerCharacter != null)
			{
				PlayerCharacter.CharacterLockedTracker.Unregister(flag);
			}
		}

		public void ResetPlayerToSpawn()
		{
			object resetFlag = new object();
			PlayerCharacter.CharacterLockedTracker.Register(resetFlag);
			{
				PlayerCharacter.transform.position = Level.PlayerSpawn.GetSpawnPosition();
				PlayerCharacter.SetLookDirection(1f);
			}
			PlayerCharacter.CharacterLockedTracker.Unregister(resetFlag);
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

			_musicAudioSource.Play();
			_gameFSM.SwitchState(0);
			SkyCamera.Priority = 0;

			// Setup HUD
			GameHUDView.Resolve();
		}

		private void OnPlayerHealthChangedEvent(Health health)
		{
			if (!health.IsAlive)
			{
				_gameFSM.SwitchState(_loseState);
			}
		}
	}
}