using Assets.HeroEditor.Common.Scripts.CharacterScripts;
using RaActions;
using RaCollection;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using static Screens.Game.Tools;

namespace Screens.Game
{

	public class GameCharacterEntity : MonoBehaviour, IRaCollectionElement
	{
		public string Id => GetInstanceID().ToString();

		[Header("Systems")]
		[SerializeField]
		private CharacterCoreSystem _coreSystem = null;

		[Header("Data")]
		[SerializeField]
		private List<string> _tags = new List<string>();

		[field: Header("Controllers")]
		[field: SerializeField]
		public MovementController MovementController
		{
			get; set;
		}

		[field: Header("Combat")]
		[field: SerializeField]
		public float AttackRadius
		{
			get; private set;

		}

		[SerializeField]
		private int _hp = 5;

		[field: Header("View")]
		[field: SerializeField]
		public string CharacterName
		{
			get; private set;
		}

		[field: SerializeField]
		public Character CharacterView
		{
			get; private set;
		}

		[field: SerializeField]
		public float RunSpeedThreshold
		{
			get; private set;
		} = 3f;

		public Health Health
		{
			get; private set;
		}

		public Vector2 VisualizedVelocity
		{
			get; private set;
		}

		public CharacterCoreSystem CoreSystem => _coreSystem;

		protected void OnValidate()
		{
			if(CharacterView == null)
			{
				CharacterView = GetComponentInChildren<Character>();
			}
		}

		protected void Awake()
		{
			MovementController.Setup();
			Health = new Health(_hp);

			CharacterView.SetState(CharacterState.Idle);
		}

		protected void Update()
		{
			Vector2 velocity = MovementController.Velocity;

			if (VisualizedVelocity != velocity)
			{
				VisualizedVelocity = velocity;

				if (VisualizedVelocity.magnitude > 0)
				{
					CharacterState movementState = MovementController.Speed >= RunSpeedThreshold ? CharacterState.Run : CharacterState.Walk;

					CharacterView.SetState(movementState);
					if (Mathf.Abs(VisualizedVelocity.x) > MovementController.Speed * 0.35)
					{
						CharacterView.transform.localScale = new Vector3(Mathf.Sign(VisualizedVelocity.x), 1f, 1f);
					}
				}
				else
				{
					CharacterView.SetState(CharacterState.Idle);
				}
			}
		}

		protected void OnDrawGizmos()
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(transform.position, AttackRadius);
			Gizmos.color = Color.white;
		}

		public bool AddTag(string tag)
		{
			if(HasTag(tag))
			{
				return false;
			}

			_tags.Add(tag);
			return true;
		}

		public bool RemoveTag(string tag)
		{
			return _tags.Remove(tag);
		}

		public bool HasTag(string tag)
		{
			return _tags.Contains(tag);
		}
	}
}