using CharacterScripts;
using CombatSystem.CharacterScripts;
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
		private CharacterStaminaTimer staminaTimer;

		private StringVariableWriter healthLabelWriter;
		private StringVariableWriter mpLabelWriter;

		public void Initialize(GameObject player)
		{
			character = player.GetComponentInChildren<Character>();

			CharacterStatistics characterStatistics = character.Statistics;
			characterHealth = player.GetComponentInChildren<CharacterHealth>();

			characterHealth.OnHealthChanged += UpdateHealth;

			nameLabel.text = characterStatistics.Name;
			staminaTimer   = player.GetComponentInChildren<CharacterStaminaTimer>();

			healthLabelWriter = new StringVariableWriter(healthLabel.text);
			mpLabelWriter     = new StringVariableWriter(mpLabel.text);

			UpdateHealth();
			UpdateMP();
		}

		private void OnDisable()
		{
			characterHealth.OnHealthChanged -= UpdateHealth;
		}

		private void LateUpdate()
		{
			timerSlider.value = 1 - staminaTimer.NormalizedStaminaTimer;
		}

		private void UpdateHealth()
		{
			healthLabel.text = healthLabelWriter.UpdateText(characterHealth.Health, characterHealth.MaximumHealth);
		}

		private void UpdateMP()
		{
			mpLabel.text = mpLabelWriter.UpdateText(character.CurrentMP, character.Statistics.MaxMP); //TODO MP
		}
	}
}