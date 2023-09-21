using Assets.HeroEditor.Common.Scripts.CharacterScripts;
using RaActions;
using RaCollection;
using System.Collections.Generic;
using UnityEngine;
using static GameModes.Game.Tools;

namespace GameModes.Game
{
	public class GameCharacterEntity : MonoBehaviour, IRaCollectionElement
	{
		public string Id => GetInstanceID().ToString();

		[SerializeField]
		private CharacterCoreSystem _coreSystem = null;

		[SerializeField]
		private List<string> _tags = new List<string>();

		[SerializeField]
		private Rigidbody2D _rigidBody2D = null;

		[SerializeField]
		private Vector2 _speedRange = Vector2.one;

		[SerializeField]
		private int _hp = 5;

		[SerializeField]
		private float _attackRadius = 1f;

		[SerializeField]
		private float _runSpeedThreshold = 4f;

		public float AttackRadius => _attackRadius;
		public float RunSpeedThreshold => _runSpeedThreshold;

		[field: SerializeField]
		public Character CharacterView
		{
			get; private set;
		}

		[field: Header("ReadOnly")]
		[field: SerializeField]
		public Direction CurrentDirections
		{
			get; set;
		}

		[field: SerializeField]
		public Vector2 CurrentDirVector
		{
			get; set;
		}

		[field: SerializeField]
		public float Speed
		{
			get; private set;
		}

		public Health Health
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
			Health = new Health(_hp);
			Speed = Random.Range(_speedRange.x, _speedRange.y);
			CoreSystem.SetDirectionFlag(this, Direction.None, CharacterCoreSystem.SetDirectionFlagAction.WriteType.Override)
				.Execute(CoreSystem.Processor);
		}

		protected void FixedUpdate()
		{
			if(CurrentDirections != Direction.None)
			{
				_rigidBody2D.MovePosition(_rigidBody2D.position + CurrentDirVector * Speed * Time.fixedDeltaTime);
			}
		}

		protected void OnDrawGizmos()
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(transform.position, _attackRadius);
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