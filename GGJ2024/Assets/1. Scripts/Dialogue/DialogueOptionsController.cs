using System;
using System.Collections.Generic;
using Dialogue.Events;
using UnityEngine;
using VDFramework;
using VDFramework.EventSystem;

namespace Dialogue
{
    public class DialogueOptionsController : BetterMonoBehaviour
    {
        [SerializeField] private GameObject button;
        private List<GameObject> buttonsList;

        private void Start()
        {
            buttonsList = new List<GameObject>();

            EventManager.AddListener<NextLineEvent>(OnNewChoices);
            EventManager.AddListener<ChooseNextDialogueLineEvent>(OnChoiceSelected);
        }

        private void OnDestroy()
        {
            EventManager.RemoveListener<NextLineEvent>(OnNewChoices);
            EventManager.RemoveListener<ChooseNextDialogueLineEvent>(OnChoiceSelected);
        }

        private void OnNewChoices(NextLineEvent @event)
        {
            for (var i = 0; i < @event.Choices.Count; i++)
            {
                var instance = Instantiate(button, transform);

                instance.GetComponent<ChoiceTextHandler>().SetValues(@event.Choices[i].text, i);
                buttonsList.Add(instance);
            }
        }

        private void OnChoiceSelected(ChooseNextDialogueLineEvent @event)
        {
            foreach (var o in buttonsList)
            {
                Destroy(o);
            }

            buttonsList.Clear();
        }
    }
}