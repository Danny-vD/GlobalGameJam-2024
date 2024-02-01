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

		[field: SerializeField]
		public int MaximumHealth { get; private set; } = 100;

		public int Health { get; private set; }

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
				OnDead.Invoke();
			}
		}

		public void Heal(int amount)
		{
			RemoveHealth(amount);
		}

		private void RemoveHealth(int amount)
		{
			Health -= amount;
			OnHealthChanged.Invoke();
		}
	}
}