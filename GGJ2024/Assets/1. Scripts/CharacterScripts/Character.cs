using CharacterScripts.Data;
using CombatSystem.Events;
using UnityEngine;
using VDFramework;

namespace CharacterScripts
{
    public class Character : BetterMonoBehaviour
    {
        [field: SerializeField] public string Name { get; private set; }

        [field: SerializeField] public CharacterAttributes PermanentAttributes { get; private set; }

        public CharacterAttributes TemporaryAttributes { get; } = new();

        public CombinedCharacterAttributes
            Attributes
        {
            get;
            private set;
        } // Using the more specific class so users can directly access the GetAttributes function 

        private void Awake()
        {
            Attributes = new CombinedCharacterAttributes(PermanentAttributes, TemporaryAttributes);
        }

        private void OnEnable()
        {
            CombatEndedEvent.AddListener(ResetTemporaryAttributes);
        }

        private void OnDisable()
        {
            CombatEndedEvent.RemoveListener(ResetTemporaryAttributes);
        }

        private void ResetTemporaryAttributes()
        {
            TemporaryAttributes.ResetValues();
        }
    }
}