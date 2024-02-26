using FMODUtilityPackage.Core;
using FMODUtilityPackage.Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopBackgroundMusic : MonoBehaviour
{
    private void OnMouseEnter()
    {
        AudioPlayer.StopEmitter(EmitterType.BackgroundMusic);
    }
    void OnMouseExit()
    {
        AudioPlayer.PlayEmitter(EmitterType.BackgroundMusic);
    }
}
