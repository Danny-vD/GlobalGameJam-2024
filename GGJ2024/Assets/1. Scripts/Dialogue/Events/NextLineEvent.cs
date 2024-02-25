using System.Collections.Generic;
using Ink.Runtime;
using VDFramework.EventSystem;

namespace Dialogue.Events
{
    public class NextLineEvent : VDEvent<NextLineEvent>
    {
        public readonly string Author;
        public readonly List<Choice> Choices;
        public readonly bool IsFinal;
        public readonly string Line;

        public NextLineEvent(string author, List<Choice> choices, string line, bool isFinal)
        {
            Author = author;
            Choices = choices;
            Line = line;
            IsFinal = isFinal;
        }

        public NextLineEvent(List<Choice> choices /*, string author, string line, bool isFinal*/)
        {
            Choices = choices;
        }
    }
}