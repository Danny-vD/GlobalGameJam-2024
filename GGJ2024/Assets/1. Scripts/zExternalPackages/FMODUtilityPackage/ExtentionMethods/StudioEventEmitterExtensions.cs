using System.Collections.Generic;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using FMODUtilityPackage.Structs;

namespace FMODUtilityPackage.ExtentionMethods
{
    public static class StudioEventEmitterExtensions
    {
        private static readonly Dictionary<string, GUID> guidsPerEvent = new();

        public static PARAMETER_ID GetParameterID(this StudioEventEmitter emitter, string parameterName)
        {
            var eventDescription = RuntimeManager.GetEventDescription(emitter.EventReference.Guid);
            eventDescription.getParameterDescriptionByName(parameterName, out var parameterDescription);
            return parameterDescription.id;
        }

        private static GUID GetGuid(string emitterEventPath)
        {
            if (guidsPerEvent.TryGetValue(emitterEventPath, out var guid)) return guid;

            guid = RuntimeManager.PathToGUID(emitterEventPath);
            guidsPerEvent.Add(emitterEventPath, guid);

            return guid;
        }

        public static void SetParameters(this StudioEventEmitter instance, EventParameters parameters)
        {
            foreach (var pair in parameters) instance.SetParameter(pair.Key, pair.Value);
        }
    }
}