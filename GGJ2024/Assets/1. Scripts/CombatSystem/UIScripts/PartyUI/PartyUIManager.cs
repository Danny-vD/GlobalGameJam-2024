using System;
using CharacterScripts;
using CombatSystem.CharacterScripts.CharacterStates;
using CombatSystem.Structs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VDFramework;
using VDFramework.Utility;

namespace CombatSystem.UIScripts.PartyUI
{
	public class PartyUIManager : BetterMonoBehaviour
	{
		[SerializeField]
		private TMP_Text nameLabel;

		[SerializeField]
		private Slider timerSlider;

		[SerializeField]
		private Slider specialAttackSlider;

		[SerializeField]
		private TMP_Text healthLabel;

		[SerializeField]
		private TMP_Text mpLabel;

		private Character character;
		private CharacterHealth characterHealth;
		
		private IdleState idleState;

		private StringVariableWriter healthLabelWriter;
		private StringVariableWriter mpLabelWriter;

		public void Initialize(GameObject player)
		{
			character = player.GetComponent<Character>();

			CharacterStatistics characterStatistics = character.Statistics;
			characterHealth = player.GetComponent<CharacterHealth>();

			characterHealth.OnHealthChanged += UpdateHealth;

			nameLabel.text = characterStatistics.Name;
			idleState      = player.GetComponent<IdleState>();

			healthLabelWriter = new StringVariableWriter(healthLabel.text);
			mpLabelWriter     = new StringVariableWriter(mpLabel.text);
		}
		
		private void OnDisable()
		{
			characterHealth.OnHealthChanged -= UpdateHealth;
		}

		private void LateUpdate()
		{
			timerSlider.value = 1 - idleState.NormalizedTimer;
		}

		private void UpdateHealth()
		{
			healthLabel.text = healthLabelWriter.UpdateText(characterHealth);
		}

		private void UpdateMP()
		{
			mpLabel.text = mpLabelWriter.UpdateText(5); //TODO MP
		}
	}
}