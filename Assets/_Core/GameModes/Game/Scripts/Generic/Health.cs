﻿using System;
using UnityEngine;


namespace GameModes.Game
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

		public bool IsAlive => HP > 0;

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
			if(IsAlive)
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