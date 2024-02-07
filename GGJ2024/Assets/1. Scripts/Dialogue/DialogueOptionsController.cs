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

            EventManager.AddListener<OnNextLineEvent>(OnNewChoices);
            EventManager.AddListener<OnChooseNextDialogueLineEvent>(OnChoiceSelected);
        }

        private void OnDestroy()
        {
            EventManager.RemoveListener<OnNextLineEvent>(OnNewChoices);
            EventManager.RemoveListener<OnChooseNextDialogueLineEvent>(OnChoiceSelected);
        }

        private void OnNewChoices(OnNextLineEvent @event)
        {
            for (var i = 0; i < @event.Choices.Count; i++)
            {
                var instance = Instantiate(button, transform);

                instance.GetComponent<ChoiceTextHandler>().SetValues(@event.Choices[i].text, i);
                buttonsList.Add(instance);
            }
        }

        private void OnChoiceSelected(OnChooseNextDialogueLineEvent @event)
        {
            foreach (var o in buttonsList)
            {
                Destroy(o);
            }

            buttonsList.Clear();
        }
    }
}