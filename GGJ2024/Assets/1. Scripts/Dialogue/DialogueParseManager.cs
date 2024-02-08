using Dialogue.Events;
using Ink.Runtime;
using InputScripts;
using InputScripts.Enums;
using UnityEngine;
using VDFramework.EventSystem;
using VDFramework.Singleton;

namespace Dialogue
{
    public class DialogueParseManager : Singleton<DialogueParseManager>
    {
        private Story currentStory;
        public bool Conversing { get; set; }

        private void OnEnable()
        {
            OnEnterDialogueMode.Listeners += EnterDialogueMode;
        }

        private void OnDisable()
        {
            OnEnterDialogueMode.Listeners -= EnterDialogueMode;
        }

        private void EnterDialogueMode(OnEnterDialogueMode onEnterDialogueMode)
        {
            InputControlManager.Instance.ChangeControls(ControlTypes.Dialogue);
            currentStory = new Story(onEnterDialogueMode.inkFile.text);
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
                EventManager.RaiseEvent<OnNextLineEvent>(new OnNextLineEvent(currentStory.currentText,
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