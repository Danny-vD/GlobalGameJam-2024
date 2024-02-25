using FMODUtilityPackage.Core;
using UnityEngine;

namespace TMP
{
    public class SetVolumeSlider : MonoBehaviour
    {
        public void SetMasterVolume(float Volume)
        {
            AudioManager.Instance.SetMasterVolume(Volume);
        }
    }
}