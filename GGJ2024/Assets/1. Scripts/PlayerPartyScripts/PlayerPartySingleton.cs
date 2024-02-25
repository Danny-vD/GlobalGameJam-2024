using System.Collections.Generic;
using UnityEngine;
using VDFramework.Singleton;

namespace PlayerPartyScripts
{
    public class PlayerPartySingleton : Singleton<PlayerPartySingleton>
    {
        [SerializeField] public List<GameObject> Party;
    }
}