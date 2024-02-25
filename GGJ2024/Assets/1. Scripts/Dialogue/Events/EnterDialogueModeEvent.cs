using UnityEngine;
using VDFramework.EventSystem;

namespace Dialogue
{
    public class EnterDialogueModeEvent : VDEvent<EnterDialogueModeEvent>
    {
        public TextAsset inkFile;

        public EnterDialogueModeEvent(TextAsset inkJson)
        {
            inkFile = inkJson;
        }
    }
}