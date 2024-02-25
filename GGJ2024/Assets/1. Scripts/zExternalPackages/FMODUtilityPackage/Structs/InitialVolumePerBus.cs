using System;
using FMODUtilityPackage.Enums;
using UnityEngine;
using VDFramework.Interfaces;

namespace FMODUtilityPackage.Structs
{
    [Serializable]
    public struct InitialVolumePerBus : IKeyValuePair<BusType, float>
    {
        [SerializeField] private BusType key;

        [SerializeField] private float value;

        public bool isMuted;

        public InitialVolumePerBus(BusType busType, float volumeValue, bool muted)
        {
            key = busType;
            value = volumeValue;
            isMuted = muted;
        }

        public static InitialVolumePerBus DefaultValue => new(default, 1, false);

        public BusType Key
        {
            get => key;
            set => key = value;
        }

        public float Value
        {
            get => value;
            set => this.value = value;
        }

        public bool Equals(IKeyValuePair<BusType, float> other)
        {
            return other != null && other.Key == Key;
        }
    }
}