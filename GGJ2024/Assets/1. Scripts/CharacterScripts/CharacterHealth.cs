using System;
using VDFramework;

namespace CharacterScripts
{
	public class CharacterHealth : BetterMonoBehaviour
	{
		public event Action<int> OnDamaged = delegate { };
		public event Action<int> OnHealed = delegate { };
		
		public event Action OnHealthChanged = delegate { };
		public event Action OnDied = delegate { };
		public event Action OnResurrected = delegate { };

		public int MaximumHealth => character.Attributes.MaxHP;

		public int Health { get; private set; }
		
		public bool IsDead { get; private set; }

		private Character character;
		
		private void Awake()
		{
			character = GetComponent<Character>();
		}

		private void Start()
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
				OnDied.Invoke();
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