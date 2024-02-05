using FMOD.Studio;
using FMODUtilityPackage.Core;
using FMODUtilityPackage.Enums;
using UnityEngine;

public class PlayerAudioFootsteps : MonoBehaviour
{
    // NOTE: use an array/list to use multiple
    private EventInstance footstepInstance;

    private void Awake()
    {
        footstepInstance = AudioPlayer.GetEventInstance(AudioEventType.SFX_Player_Footsteps);
    }

    private void PlayFootsteps()
    {
        footstepInstance.start();
    }
    
    private void OnDestroy()
    {
        footstepInstance.release();
        footstepInstance.stop(STOP_MODE.IMMEDIATE);
    }
}
