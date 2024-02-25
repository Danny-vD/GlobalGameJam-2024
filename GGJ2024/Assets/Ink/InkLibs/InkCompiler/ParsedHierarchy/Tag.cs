using Ink.Runtime;

namespace Ink.Parsed
{
    public class Tag : Object
    {
        public bool inChoice;

        public bool isStart;

        public override Runtime.Object GenerateRuntimeObject()
        {
            if (isStart)
                return ControlCommand.BeginTag();
            return ControlCommand.EndTag();
        }

        public override string ToString()
        {
            if (isStart)
                return "#StartTag";
            return "#EndTag";
        }
    }
}