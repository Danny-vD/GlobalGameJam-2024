using PlayerPartyScripts;
using UnityEngine;
using VDFramework;
using VDFramework.UnityExtensions;

namespace CombatSystem.UIScripts.PartyUI
{
    public class PartyUISpawner : BetterMonoBehaviour
    {
        [SerializeField] private Transform partyUIParent;

        [SerializeField] private GameObject partyUIPrefab;

        private void Start()
        {
            InstantiatePartyFields();
        }

        private void InstantiatePartyFields()
        {
            partyUIParent.DestroyChildren();

            foreach (var player in PlayerPartySingleton.Instance.Party)
            {
                var instance = Instantiate(partyUIPrefab, partyUIParent);

                instance.GetComponent<PartyUIManager>().Initialize(player);
            }
        }
    }
}