using System;
using Dialogue.Events;
using Ink.Runtime;
using InputScripts;
using InputScripts.Enums;
using SerializableDictionaryPackage.SerializableDictionary;
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
            EventManager.AddListener<ChoiceSelectedEvent>(OnChoiceSelected, -1);
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
                else if (@event.Index == -1)
                {
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
                currentStory.Continue();
                EventManager.RaiseEvent<NextLineEvent>(new NextLineEvent(GetAuthor(),
                    currentStory.currentChoices, currentStory.currentText, false));
            }
            else
            {
                Conversing = false;
                EventManager.RaiseEvent<ExitDialogueModeEvent>(new ExitDialogueModeEvent());
                InputControlManager.Instance.ChangeControls(ControlTypes.Overworld);
                // TODO: CHANGE TO WHATEVER AT END  
            }
        }

        private string GetAuthor()
        {
            return currentStory.currentTags?[0].Split(':', 2)[1].Trim();
            
        }
    }
}