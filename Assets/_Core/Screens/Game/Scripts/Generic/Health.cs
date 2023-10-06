using System;
using UnityEngine;

namespace Screens.Game
{
	public class Health
	{
		public event Action<Health> HealthChangedEvent;
		public event Action<Health> DamagedEvent;
		public event Action<Health> HealedEvent;

		public int HP
		{
			get; private set;
		}

		public int MaxHP
		{
			get; private set;
		}

		public float NormalizedValue => (float)HP / MaxHP;

		public bool IsAlive => HP > 0;

		public bool Invulnerable
		{
			get; set;
		}

		public Health(int hp)
		{
			MaxHP = HP = hp;
		}

		public bool Damage(int amount)
		{
			if(SetHP(HP - amount))
			{
				DamagedEvent?.Invoke(this);
				return true;
			}
			return false;
		}

		public bool Heal(int amount)
		{
			if(SetHP(HP + amount))
			{
				HealedEvent?.Invoke(this);
				return true;
			}
			return false;
		}

		public bool SetHP(int value)
		{
			if(Invulnerable)
			{
				return false;
			}

			if(!IsAlive)
			{
				return false;
			}

			int newHP = Mathf.Clamp(value, 0, MaxHP);
			if (newHP != HP)
			{
				HP = newHP;
				HealthChangedEvent?.Invoke(this);
				return true;
			}

			return false;
		}
	}
}