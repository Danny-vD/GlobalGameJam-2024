using VDFramework.EventSystem;

namespace Dialogue.Events
{
    public class ChoiceSelectedEvent : VDEvent<ChoiceSelectedEvent>
    {
        public readonly bool Skip;
        public readonly int Index;
        
        public ChoiceSelectedEvent(int index, bool skip)
        {
            Index = index;
            Skip = skip;
        }
    }
}