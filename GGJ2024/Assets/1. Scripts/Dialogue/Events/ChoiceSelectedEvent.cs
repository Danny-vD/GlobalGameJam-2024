using VDFramework.EventSystem;

namespace Dialogue.Events
{
    public class ChoiceSelectedEvent : VDEvent<ChoiceSelectedEvent>
    {
        public readonly int Index;
        public readonly bool Skip;

        public ChoiceSelectedEvent(int index, bool skip)
        {
            Index = index;
            Skip = skip;
        }
    }
}