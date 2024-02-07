using System.Collections.Generic;
using Ink.Runtime;
using VDFramework.EventSystem;

namespace Dialogue.Events
{
    public class OnNextLineEvent : VDEvent<OnNextLineEvent>
    {
        public readonly string Author;
        public readonly List<Choice> Choices;
        public readonly string Line;
        public readonly bool IsFinal;

        public OnNextLineEvent( string author, List<Choice> choices, string line, bool isFinal)
        {
            Author = author;
            Choices = choices;
            Line = line;
            IsFinal = isFinal;
        }
        
        public OnNextLineEvent(List<Choice> choices/*, string author, string line, bool isFinal*/)
        {
            Choices = choices;
        }
    }
}