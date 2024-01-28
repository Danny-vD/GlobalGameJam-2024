using FMODUtilityPackage.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetVolumeSlider : MonoBehaviour
{
    public void SetMasterVolume(float Volume)
    {
        AudioManager.Instance.SetMasterVolume(Volume);
    }
}
