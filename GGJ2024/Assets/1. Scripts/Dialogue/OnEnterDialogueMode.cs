using Ink.Parsed;
using Ink.Runtime;
using UnityEngine;
using VDFramework.EventSystem;

namespace Dialogue
{
    public class OnEnterDialogueMode : VDEvent<OnEnterDialogueMode>
    {
        public TextAsset inkFile;
        
        private OnEnterDialogueMode(TextAsset inkJson)
        {
            inkFile = inkJson;
        }
    }
}