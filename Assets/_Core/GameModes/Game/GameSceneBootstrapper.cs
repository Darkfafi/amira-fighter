using RaFSM;
using UnityEngine;

namespace GameModes.Game
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

		public GameCharacterEntity PlayerCharacter
		{
			get; private set;
		}

		private RaGOFiniteStateMachine _gameFSM;

		protected override void OnSetData()
		{
			_gameFSM = new RaGOFiniteStateMachine(this, RaGOFiniteStateMachine.GetGOStates(transform));
		}

		protected override void OnSetDataResolved()
		{
			base.OnSetDataResolved();

			if(CharacterCoreSystem.Instance.SpawnCharacter(Data.PlayerCharacterPrefab, Level.PlayerSpawn.GetSpawnPosition()).Execute(CharacterActionsSystem.Processor, out var result))
			{
				PlayerCharacter = result.CreatedCharacter;
				CameraFollowObject.SetParent(PlayerCharacter.CharacterView.transform, worldPositionStays: false);
				CameraFollowObject.transform.localPosition = Vector3.zero;
			}

			_gameFSM.SwitchState(0);
		}

		protected override void OnClearData()
		{
			_gameFSM.Dispose();

			if (CharacterCoreSystem.IsAvailable)
			{
				CharacterCoreSystem.Instance.DespawnCharacter(PlayerCharacter).Execute(CharacterActionsSystem.Processor);
			}
		}

		public void GoToNextState()
		{
			_gameFSM.GoToNextState(wrap: false);
		}
	}
}