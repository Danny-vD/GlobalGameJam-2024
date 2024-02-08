using System;
using Dialogue.Events;
using Ink.Runtime;
using UI;
using UnityEngine;
using VDFramework.EventSystem;
using VDFramework.Singleton;

namespace Dialogue
{
    public class DialogueParseManager : Singleton<DialogueParseManager>
    {
        private Story currentStory;
        public bool Conversing { get; set; }

        [SerializeField] private GameObject canvas;

        private void Start()
        {
            EnterDialogueModeEvent.Listeners += EnterDialogueMode;
            EventManager.AddListener<ChoiceSelectedEvent>(OnChoiceSelected);
        }

        private void OnDisable()
        {
            EnterDialogueModeEvent.Listeners -= EnterDialogueMode;
            EventManager.RemoveListener<ChoiceSelectedEvent>(OnChoiceSelected);
        }
        

        private void EnterDialogueMode(EnterDialogueModeEvent enterDialogueModeEvent)
        {
            
            InputControlManager.Instance.ChangeControls(ControlTypes.Dialogue);
            canvas.SetActive(true);
            currentStory = new Story(enterDialogueModeEvent.inkFile.text);
            ParseLine();
        }

        private void OnChoiceSelected(ChoiceSelectedEvent @event)
        {
            if (@event.Skip)
            {
                ParseLine();
            }
            else
            {
                if (currentStory.currentChoices.Count >= @event.Index && currentStory.currentChoices.Count != 0)
                {
                    currentStory.ChooseChoiceIndex(@event.Index);
                    ParseLine();
                }
                else
                {
                    Debug.Log("OUT OF INDEX");
                }
            }
        }

        private void ParseLine()
        {
            if (currentStory.canContinue)
            {
                var CurrentLine = currentStory.Continue();
                EventManager.RaiseEvent<NextLineEvent>(new NextLineEvent(currentStory.currentText,
                    currentStory.currentChoices, GetAuthor(), false));
            }
            else
            {
                Conversing = false;
                EventManager.RaiseEvent<ExitDialogueModeEvent>(new ExitDialogueModeEvent());
            }
        }

        private string GetAuthor()
        {
            return currentStory.currentTags?[0];
        }
    }
}