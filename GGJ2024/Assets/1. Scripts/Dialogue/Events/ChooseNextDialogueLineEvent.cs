using VDFramework.EventSystem;

namespace Dialogue.Events
{
    public class ChooseNextDialogueLineEvent : VDEvent<ChooseNextDialogueLineEvent>
    {
        public readonly int ChoiceIndex;
        
        public ChooseNextDialogueLineEvent(int choice)
        {
            ChoiceIndex = choice;
        }
    }
}