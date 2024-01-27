using VDFramework.EventSystem;

namespace Dialogue
{
    public class OnChooseNextDialogueLine : VDEvent<OnChooseNextDialogueLine>
    {
        public readonly int choiceIndex;
        
        public OnChooseNextDialogueLine(int choice)
        {
            choiceIndex = choice;
            
        }
    }
}