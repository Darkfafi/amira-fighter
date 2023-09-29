using Assets.HeroEditor.Common.Scripts.CharacterScripts;
using CartoonFX;
using RaCollection;
using RaFlags;
using RaTweening;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Screens.Game
{
	public class GameCharacterEntity : MonoBehaviour, IRaCollectionElement
	{
		public event Action<bool> CharacterLockedStateChangedEvent;

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
			get; private set;
		}

		[field: SerializeField]
		public KnockbackController KnockbackController
		{
			get; private set;
		}

		[field: Header("Combat")]
		[field: SerializeField]
		public MeleeAttackSkill MeleeAttackSkill
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
		public OrthographicAgent OrthographicAgent
		{
			get; private set;
		}

		[field: SerializeField]
		public Character CharacterView
		{
			get; private set;
		}

		[field: SerializeField]
		public Transform DirectionView
		{
			get; private set;
		}

		[field: SerializeField]
		public float RunSpeedThreshold
		{
			get; private set;
		} = 3f;

		[field: Header("UI")]
		[field: SerializeField]
		public GameCharacterUIView UIView
		{
			get; private set;
		}

		[field: Header("VFX")]
		[field: SerializeField]
		public ParticleSystem HitEffect
		{
			get; private set;
		}

		public Health Health
		{
			get; private set;
		}

		public Vector2 VisualizedVelocity
		{
			get; private set;
		}

		public bool IsCharacterLocked => !CharacterLockedTracker.IsEmpty();

		public CharacterCoreSystem CoreSystem => _coreSystem;

		public RaFlagsTracker CharacterLockedTracker
		{
			get
			{
				if(_characterLockedTracker == null)
				{
					_characterLockedTracker = new RaFlagsTracker(OnCharacterLockedStateChanged);
				}
				return _characterLockedTracker;
			}
		}

		private RaFlagsTracker _characterLockedTracker;
		private readonly object _hitGroup = new object();

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
			Health.DamagedEvent += OnDamagedEvent;

			UIView.SetData(this);

			RefreshMovementAnimation();
		}

		protected void OnDestroy()
		{
			if(Health != null)
			{
				Health.DamagedEvent -= OnDamagedEvent;
			}

			if (UIView)
			{
				UIView.ClearData();
			}
		}

		protected void Update()
		{
			Vector2 velocity = MovementController.Velocity;

			if (VisualizedVelocity != velocity)
			{
				VisualizedVelocity = velocity;
				RefreshMovementAnimation();
			}
		}

		public void SetLookDirection(float direction)
		{
			if(Mathf.Approximately(direction, 0f))
			{
				return;
			}

			DirectionView.localScale = new Vector3(Mathf.Sign(direction), 1f, 1f);
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

		private void RefreshMovementAnimation()
		{
			if (VisualizedVelocity.magnitude > MovementController.Speed * 0.25f)
			{
				CharacterState movementState = MovementController.Speed >= RunSpeedThreshold ? CharacterState.Run : CharacterState.Walk;

				CharacterView.SetState(movementState);
				if (MovementController.Destination.HasValue)
				{
					Vector2 delta = transform.position;
					delta = MovementController.Destination.Value - delta;

					if (delta.magnitude >= .2f)
					{
						SetLookDirection(delta.x);
					}
				}
			}
			else
			{
				CharacterView.SetState(CharacterState.Idle);
			}
		}

		private void OnCharacterLockedStateChanged(bool isEmpty, RaFlagsTracker tracker)
		{
			CharacterLockedStateChangedEvent?.Invoke(!isEmpty);
		}

		private void OnDamagedEvent(Health health)
		{
			HitEffect.SetSortingOrder(OrthographicAgent.SortingOrder);
			HitEffect.Play();

			RaTweenBase.CompleteGroup(_hitGroup);

			CharacterView.transform.TweenPunchScale(Vector3.one * 0.1f, 0.2f, 1).OnComplete(() => 
			{
				CharacterView.transform.localScale = Vector3.one;
			}).SetGroup(_hitGroup);
		}
	}
}