using CharacterScripts.Data;
using CombatSystem.Events;
using UnityEngine;
using VDFramework;

namespace CharacterScripts
{
	public class Character : BetterMonoBehaviour
	{
		[field: SerializeField]
		public string Name { get; private set; }

		[field: SerializeField]
		public CharacterAttributes PermanentAttributes { get; private set; }

		public CharacterAttributes TemporaryAttributes { get; private set; }

		public CombinedCharacterAttributes Attributes { get; private set; } // Use the more specific class so users can directly access the GetAttributes function 

		public int CurrentMP { get; private set; }

		private void Awake()
		{
			TemporaryAttributes = new CharacterAttributes();
			Attributes          = new CombinedCharacterAttributes(PermanentAttributes, TemporaryAttributes);
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