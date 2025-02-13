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

        private void OnEnable()
        {
            buttonsList = new List<GameObject>();

            EventManager.AddListener<NextLineEvent>(OnNewChoices);
            EventManager.AddListener<FinishedLineEvent>(PerformAnimation);
            EventManager.AddListener<ChoiceSelectedEvent>(OnChoiceSelected);
        }

        private void OnDisable()
        {
            EventManager.RemoveListener<NextLineEvent>(OnNewChoices);
            EventManager.RemoveListener<FinishedLineEvent>(PerformAnimation);
            EventManager.RemoveListener<ChoiceSelectedEvent>(OnChoiceSelected);
        }

        private void PerformAnimation()
        {
            //TODO: ADD ENTER ANIMATION
        }

        private void OnNewChoices(NextLineEvent @event)
        {
            Debug.Log("NEXT LINE EVENT TRIGGERED");
            if (@event.Choices.Count == 0)
            {
                GameObject instance = Instantiate(button, transform);

                instance.GetComponent<ChoiceTextHandler>().SetValues("...", -1);
                buttonsList.Add(instance);
            }
            else
            {
                for (int i = 0; i < @event.Choices.Count; i++)
                {
                    GameObject instance = Instantiate(button, transform);

                    instance.GetComponent<ChoiceTextHandler>().SetValues(@event.Choices[i].text, i);
                    buttonsList.Add(instance);
                }
            }
        }

        private void OnChoiceSelected(ChoiceSelectedEvent @event)
        {
            foreach (GameObject o in buttonsList) Destroy(o);

            Debug.Log("DESTROYED");
            buttonsList.Clear();
        }
    }
}