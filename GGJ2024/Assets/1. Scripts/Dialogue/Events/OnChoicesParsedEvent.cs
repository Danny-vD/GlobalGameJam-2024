using System.Collections.Generic;
using Ink.Runtime;
using VDFramework.EventSystem;

namespace Dialogue.Events
{
    public class OnChoicesParsedEvent : VDEvent<OnChoicesParsedEvent>
    {
        public readonly List<Choice> Choices;
        
        public OnChoicesParsedEvent(List<Choice> choices)
        {
            Choices = choices;
        }
    }
}