﻿using System;
using VDFramework;

namespace CharacterScripts
{
	public class CharacterMP : BetterMonoBehaviour
	{
		public event Action<int> OnMPDecrease = delegate { };
		public event Action<int> OnMPIncrease = delegate { };
		
		public event Action OnMPChanged = delegate { };

		public int MaximumMP => character.Attributes.MaxMP;

		public int MP { get; private set; }

		public bool IsOutOfMP => MP <= 0;

		private Character character;
		
		private void Awake()
		{
			character = GetComponent<Character>();
		}

		private void Start()
		{
			MP = MaximumMP;
		}

		public void DecreaseMP(int amount)
		{
			RemoveMP(amount);
			OnMPDecrease.Invoke(amount);
		}

		public void IncreaseMP(int amount)
		{
			RemoveMP(-amount);
			OnMPIncrease.Invoke(amount);
		}

		private void RemoveMP(int amount)
		{
			MP -= amount;
			OnMPChanged.Invoke();
		}

		private void ResetMP()
		{
			MP = MaximumMP;
		}

	}
}