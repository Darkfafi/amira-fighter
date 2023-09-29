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
		public event Action CharacterKilledEvent;

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
		public PushMovementController PushMovementController
		{
			get; private set;
		}

		[field: Header("Combat")]
		[field: SerializeField]
		public MeleeAttackSkill MeleeAttackSkill
		{
			get; private set;
		}

		[field: SerializeField]
		public CharacterSkillBase[] AllSkills
		{
			get; private set;
		}

		[SerializeField]
		private int _hp = 5;

		[SerializeField]
		private bool _despawnOnDeath = true;

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

		[field: SerializeField]
		public float LookDirectionThreshold
		{
			get; private set;
		} = .2f;

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

		[field: SerializeField]
		public ParticleSystem DespawnEffectPrefab
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
		private readonly object _deathCharacterLocker = new object();

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
            if (IsCharacterLocked)
            {
				return;
            }

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
			if (!IsCharacterLocked)
			{
				if (VisualizedVelocity.magnitude > MovementController.Speed * 0.25f)
				{
					CharacterState movementState = MovementController.Speed >= RunSpeedThreshold ? CharacterState.Run : CharacterState.Walk;

					CharacterView.SetState(movementState);
					if (MovementController.Destination.HasValue)
					{
						Vector2 delta = transform.position;
						delta = MovementController.Destination.Value - delta;

						if (delta.magnitude >= LookDirectionThreshold)
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

			CharacterView.transform.TweenScale(0.9f, 0.25f).SetModifier(RaModifierType.AbsSin).OnComplete(() => 
			{
				CharacterView.transform.localScale = Vector3.one;
			}).SetGroup(_hitGroup);

			if(!health.IsAlive)
			{
				// Lock Character & Movement
				CharacterLockedTracker.Register(_deathCharacterLocker);
				MovementController.MovementBlockers.Register(_deathCharacterLocker);
				MovementController.AgentDisablers.Register(_deathCharacterLocker);

				CharacterView.SetState(UnityEngine.Random.Range(0f, 1f) >= 0.5f ? CharacterState.DeathB : CharacterState.DeathF);
				
				if (_despawnOnDeath)
				{
					int preDeathSortingOrder = OrthographicAgent.SortingOrder;
					CharacterView.transform.TweenScale(0f, 0.25f).SetEasing(RaEasingType.InExpo).SetDelay(3f).OnComplete(() =>
					{
						Instantiate(DespawnEffectPrefab, CharacterView.transform.position, Quaternion.identity)
							.SetSortingOrder(preDeathSortingOrder + 25)
							.transform.localScale = Vector3.one * 0.75f;

						CoreSystem.DespawnCharacter(this).Execute(CoreSystem.Processor);
					});
				}

				CharacterKilledEvent?.Invoke();
			}
		}
	}
}