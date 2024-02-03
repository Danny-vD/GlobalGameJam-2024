using System;
using UnityEngine;
using VDFramework;

namespace CharacterScripts
{
	public class CharacterHealth : BetterMonoBehaviour
	{
		public event Action<int> OnDamaged = delegate { };
		public event Action<int> OnHealed = delegate { };
		
		public event Action OnHealthChanged = delegate { };
		public event Action OnDead = delegate { };
		public event Action OnResurrected = delegate { };

		[field: SerializeField]
		public int MaximumHealth { get; private set; } = 100;

		public int Health { get; private set; }
		
		public bool IsDead { get; private set; }

		private void Awake()
		{
			Health = MaximumHealth;
		}

		public void Damage(int amount)
		{
			RemoveHealth(amount);
			OnDamaged.Invoke(amount);
			
			if (Health <= 0)
			{
				IsDead = true;
				OnDead.Invoke();
			}
		}

		public void Heal(int amount)
		{
			RemoveHealth(-amount);
			OnHealed.Invoke(amount);

			if (IsDead && Health > 0)
			{
				IsDead = false;
				OnResurrected.Invoke();
			}
		}

		private void RemoveHealth(int amount)
		{
			Health -= amount;
			OnHealthChanged.Invoke();
		}

		private void ResetHealth()
		{
			Health = MaximumHealth;
		}
	}
}