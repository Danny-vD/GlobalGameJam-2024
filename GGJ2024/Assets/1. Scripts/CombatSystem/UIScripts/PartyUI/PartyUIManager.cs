using CharacterScripts;
using CombatSystem.CharacterScripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VDFramework;
using VDFramework.Utility;

namespace CombatSystem.UIScripts.PartyUI
{
    public class PartyUIManager : BetterMonoBehaviour
    {
        [SerializeField] private TMP_Text nameLabel;

        [SerializeField] private Slider timerSlider;

        [SerializeField] private Slider specialAttackSlider;

        [SerializeField] private TMP_Text healthLabel;

        [SerializeField] private TMP_Text mpLabel;

        private Character character;
        private CharacterHealth characterHealth;
        private CharacterMP characterMP;

        private StringVariableWriter healthLabelWriter;
        private StringVariableWriter mpLabelWriter;

        private CharacterStaminaTimer staminaTimer;

        private void LateUpdate()
        {
            timerSlider.value = 1 - staminaTimer.NormalizedStaminaTimer;
        }

        private void OnDisable()
        {
            characterHealth.OnHealthChanged -= UpdateHealth;
            characterMP.OnMPChanged -= UpdateMP;
        }

        public void Initialize(GameObject player)
        {
            character = player.GetComponent<Character>();
            characterHealth = player.GetComponent<CharacterHealth>();
            characterMP = player.GetComponent<CharacterMP>();

            characterHealth.OnHealthChanged += UpdateHealth;
            characterMP.OnMPChanged += UpdateMP;

            nameLabel.text = character.Name;
            staminaTimer = player.GetComponent<CharacterStaminaTimer>();

            healthLabelWriter = new StringVariableWriter(healthLabel.text);
            mpLabelWriter = new StringVariableWriter(mpLabel.text);

            UpdateHealth();
            UpdateMP();
        }

        private void UpdateHealth()
        {
            healthLabel.text = healthLabelWriter.UpdateText(characterHealth.Health, characterHealth.MaximumHealth);
        }

        private void UpdateMP()
        {
            mpLabel.text = mpLabelWriter.UpdateText(characterMP.MP, character.Attributes.MaxMP);
        }
    }
}