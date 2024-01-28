using System.Collections;
using System.Collections.Generic;
using FMODUtilityPackage.Core;
using UnityEngine;
using EventType = FMODUtilityPackage.Enums.EventType;

public class PlayerAudioFootsteps : MonoBehaviour
{
    private void playFootsteps()
    {
        AudioPlayer.PlayOneShot2D(EventType.SFX_Player_Footsteps);
    }
}
