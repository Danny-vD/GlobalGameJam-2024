using System;
using Dialogue.Events;
using TMPro;
using UnityEngine;
using VDFramework;
using VDFramework.EventSystem;

namespace Dialogue.Markup
{
    public class SpecialMarkupHandler : BetterMonoBehaviour
    {
        private TMP_Text dialogueText;

        private void OnEnable()
        {
            // EventManager.AddListener<NextLineEvent>(OnLineChosen);
        }

        private void OnDisable()
        {
            // EventManager.RemoveListener<NextLineEvent>(OnLineChosen);
        }
        
        

        private void Update()
        {
            throw new NotImplementedException();
        }
    }
}