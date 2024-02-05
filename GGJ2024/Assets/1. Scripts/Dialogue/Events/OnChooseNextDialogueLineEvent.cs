using VDFramework.EventSystem;

namespace Dialogue.Events
{
    public class OnChooseNextDialogueLineEvent : VDEvent<OnChooseNextDialogueLineEvent>
    {
        public readonly int ChoiceIndex;
        
        public OnChooseNextDialogueLineEvent(int choice)
        {
            ChoiceIndex = choice;
        }
    }
}