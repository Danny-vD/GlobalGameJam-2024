using FMOD.Studio;
using FMODUtilityPackage.Structs;

namespace FMODUtilityPackage.ExtentionMethods
{
    public static class EventInstanceExtensions
    {
        public static void SetParameters(this EventInstance instance, EventParameters parameters)
        {
            foreach (var pair in parameters) instance.setParameterByName(pair.Key, pair.Value);
        }
    }
}