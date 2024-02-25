using FMOD;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.Serialization;

namespace FMODUnity
{
    [AddComponentMenu("FMOD Studio/FMOD Studio Global Parameter Trigger")]
    public class StudioGlobalParameterTrigger : EventHandler
    {
        [ParamRef] [FormerlySerializedAs("parameter")]
        public string Parameter;

        public EmitterGameEvent TriggerEvent;

        [FormerlySerializedAs("value")] public float Value;

        private PARAMETER_DESCRIPTION parameterDescription;
        public PARAMETER_DESCRIPTION ParameterDescription => parameterDescription;

        protected override void HandleGameEvent(EmitterGameEvent gameEvent)
        {
            if (TriggerEvent == gameEvent) TriggerParameters();
        }

        public void TriggerParameters()
        {
            var paramNameSpecified = !string.IsNullOrEmpty(Parameter);
            if (paramNameSpecified)
            {
                var result = RESULT.OK;
                var paramIDNeedsLookup = string.IsNullOrEmpty(parameterDescription.name);
                if (paramIDNeedsLookup)
                {
                    result = RuntimeManager.StudioSystem.getParameterDescriptionByName(Parameter,
                        out parameterDescription);
                    if (result != RESULT.OK)
                    {
                        RuntimeUtils.DebugLogError(string.Format(
                            "[FMOD] StudioGlobalParameterTrigger failed to lookup parameter {0} : result = {1}",
                            Parameter, result));
                        return;
                    }
                }

                result = RuntimeManager.StudioSystem.setParameterByID(parameterDescription.id, Value);
                if (result != RESULT.OK)
                {
                    RuntimeUtils.DebugLogError(string.Format(
                        "[FMOD] StudioGlobalParameterTrigger failed to set parameter {0} : result = {1}", Parameter,
                        result));
                }
            }
        }
    }
}