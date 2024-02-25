using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;

namespace FMODUtilityPackage.ExtentionMethods
{
    public static class EventReferenceExtensions
    {
        public static EventDescription GetEventDescription(this ref EventReference instance)
        {
            return RuntimeManager.GetEventDescription(instance.Guid);
        }

        public static PARAMETER_DESCRIPTION GetParameterDescriptionByName(this EventReference instance,
            string parameterName)
        {
            var eventDescription = RuntimeManager.GetEventDescription(instance.Guid);
            eventDescription.getParameterDescriptionByName(parameterName, out var parameterDescription);

            return parameterDescription;
        }

        public static IEnumerable<PARAMETER_DESCRIPTION> GetParameters(this EventReference instance)
        {
            var eventDescription = RuntimeManager.GetEventDescription(instance.Guid);
            eventDescription.getParameterDescriptionCount(out var count);

            var array = new PARAMETER_DESCRIPTION[count];

            for (var i = 0; i < count; i++)
            {
                eventDescription.getParameterDescriptionByIndex(i, out var parameterDescription);
                array[i] = parameterDescription;
            }

            return array;
        }
    }
}