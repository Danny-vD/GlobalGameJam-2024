using System;
using UnityEngine;

namespace CharacterScripts.Data
{
    [Serializable]
    public class CharacterAttributes
    {
        [field: SerializeField] public virtual float Speed { get; set; }


        [field: SerializeField] public virtual float AttackPower { get; set; }

        [field: SerializeField] public virtual float SpecialAttackPower { get; set; }


        [field: SerializeField] public virtual float Defense { get; set; }

        [field: SerializeField] public virtual float SpecialDefense { get; set; }


        [field: SerializeField] public virtual int MaxHP { get; set; }

        [field: SerializeField] public virtual int MaxMP { get; set; }

        public CharacterAttributes()
        {
        }

        /// <summary>
        ///     Copy constructor - calls <see cref="SetValues" /> with the provided attributes
        /// </summary>
        public CharacterAttributes(CharacterAttributes attributes)
        {
            // ReSharper disable once VirtualMemberCallInConstructor // Intended virtual call
            SetValues(attributes);
        }

        /// <summary>
        ///     Set all the values of the attributes to the values of the provided attributes
        /// </summary>
        public virtual void SetValues(CharacterAttributes attributes)
        {
            Speed = attributes.Speed;

            AttackPower = attributes.AttackPower;
            SpecialAttackPower = attributes.SpecialAttackPower;

            Defense = attributes.Defense;
            SpecialDefense = attributes.SpecialDefense;

            MaxHP = attributes.MaxHP;
            MaxMP = attributes.MaxMP;
        }

        /// <summary>
        ///     Reset all attributes to their default value
        /// </summary>
        public virtual void ResetValues()
        {
            Speed = 0;

            AttackPower = 0;
            SpecialAttackPower = 0;

            Defense = 0;
            SpecialDefense = 0;

            MaxHP = 0;
            MaxMP = 0;
        }
    }
}