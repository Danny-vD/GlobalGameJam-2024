using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CharacterScripts.Data
{
	public class CombinedCharacterAttributes : CharacterAttributes
	{
		public override float Speed => accumulatedAttributesList.Sum(attributes => attributes.Speed);

		public override float AttackPower => accumulatedAttributesList.Sum(attributes => attributes.AttackPower);
		public override float SpecialAttackPower => accumulatedAttributesList.Sum(attributes => attributes.SpecialAttackPower);

		public override float Defense => accumulatedAttributesList.Sum(attributes => attributes.Defense);
		public override float SpecialDefense => accumulatedAttributesList.Sum(attributes => attributes.SpecialDefense);

		public override int MaxHP => accumulatedAttributesList.Sum(attributes => attributes.MaxHP);
		public override int MaxMP => accumulatedAttributesList.Sum(attributes => attributes.MaxMP);

		private readonly List<CharacterAttributes> accumulatedAttributesList = new List<CharacterAttributes>();

		public CombinedCharacterAttributes()
		{
		}

		public CombinedCharacterAttributes(params CharacterAttributes[] attributesCollection)
		{
			foreach (CharacterAttributes attributes in attributesCollection)
			{
				AddAttributesToAccumulate(attributes);
			}
		}

		public CombinedCharacterAttributes(CharacterAttributes attributes) : base(attributes)
		{
			AddAttributesToAccumulate(attributes);
		}

		/// <summary>
		/// Add the provided attributes to the list of attributes to add together when getting an attribute from this instance
		/// </summary>
		public void AddAttributesToAccumulate(CharacterAttributes attributes)
		{
			if (ReferenceEquals(attributes, this))
			{
				Debug.LogError("Cannot add itself to accumulated attributes!");
				return; // Don't allow adding ourselves (would cause a stack overflow in GetAttributes())
			}
			
			accumulatedAttributesList.Add(attributes);
		}

		/// <summary>
		/// Removes the provided attributes from the list of attributes to add together when getting an attribute from this instance
		/// </summary>
		public void RemoveAttributesToAccumulate(CharacterAttributes attributes)
		{
			accumulatedAttributesList.Remove(attributes);
		}

		/// <summary>
		/// Getting the individual attributes one by one is very inefficient, use this function instead if you need multiple at once
		/// </summary>
		public CharacterAttributes GetAttributes()
		{
			CharacterAttributes characterAttributes = new CharacterAttributes();

			foreach (CharacterAttributes attributes in accumulatedAttributesList) // Only iterate over the list once to add together the attributes
			{
				// Make sure we use the more efficient method of getting attributes if it is a CombinedAttributes instance
				CharacterAttributes attributesToAdd = attributes is CombinedCharacterAttributes combinedAttributes ? combinedAttributes.GetAttributes() : attributes;

				characterAttributes.Speed += attributesToAdd.Speed;

				characterAttributes.AttackPower        += attributesToAdd.AttackPower;
				characterAttributes.SpecialAttackPower += attributesToAdd.SpecialAttackPower;

				characterAttributes.Defense        += attributesToAdd.Defense;
				characterAttributes.SpecialDefense += attributesToAdd.SpecialDefense;

				characterAttributes.MaxHP += attributesToAdd.MaxHP;
				characterAttributes.MaxMP += attributesToAdd.MaxMP;
			}

			return characterAttributes;
		}

		/// <summary>
		/// This function is overriden to do nothing
		/// </summary>
		public override void SetValues(CharacterAttributes attributes)
		{
			// We don't store values of our own, so SetValues does nothing
		}

		/// <summary>
		/// This function is overriden to do nothing
		/// </summary>
		public override void ResetValues()
		{
			// We don't store values of our own, so ResetValues does nothing
		}
	}
}