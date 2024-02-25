using System;
using System.Collections.Generic;
using UnityEngine;

namespace FMODUnity
{
    public class EventCache : ScriptableObject
    {
        [SerializeField] public List<EditorBankRef> EditorBanks;

        [SerializeField] public List<EditorEventRef> EditorEvents;

        [SerializeField] public List<EditorParamRef> EditorParameters;

        [SerializeField] public List<EditorBankRef> MasterBanks;

        [SerializeField] public List<EditorBankRef> StringsBanks;

        [SerializeField] private long cacheTime;

        [SerializeField] public int cacheVersion;

        public EventCache()
        {
            EditorBanks = new List<EditorBankRef>();
            EditorEvents = new List<EditorEventRef>();
            EditorParameters = new List<EditorParamRef>();
            MasterBanks = new List<EditorBankRef>();
            StringsBanks = new List<EditorBankRef>();
            cacheTime = 0;
        }

        public DateTime CacheTime
        {
            get => new(cacheTime);
            set => cacheTime = value.Ticks;
        }
    }
}