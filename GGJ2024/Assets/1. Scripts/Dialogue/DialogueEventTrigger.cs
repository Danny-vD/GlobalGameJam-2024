using System;
using DefaultNamespace;
using UnityEngine;
using VDFramework;
using VDFramework.EventSystem;

namespace Dialogue
{
    public class DialogueEventTrigger : BetterMonoBehaviour, IInteractable
    {
        [SerializeField] private TextAsset InkFile;

        private void OnTriggerEnter(Collider collider)
        {
            EventManager.RaiseEvent(new EnterDialogueModeEvent(InkFile));
            Debug.Log("EVENT RAISED");
        }

        private void Start()
        {
            
        }
    }
}