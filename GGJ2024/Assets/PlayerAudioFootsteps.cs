using CombatMoves.TargetingLogic.Enums;
using FMOD.Studio;
using FMODUtilityPackage.Core;
using UnityEngine;
using EventType = FMODUtilityPackage.Enums.EventType;

public class PlayerAudioFootsteps : MonoBehaviour
{
    // NOTE: use an array/list to use multiple
    private EventInstance footstepInstance;

    [SerializeField]
    private ValidTargets test;

    private void Awake()
    {
        footstepInstance = AudioPlayer.GetEventInstance(EventType.SFX_Player_Footsteps);
    }

    private void PlayFootsteps()
    {
        AudioPlayer.PlayOneShot2D(EventType.SFX_Player_Footsteps);
    }
    
    private void OnDestroy()
    {
        footstepInstance.release();
        footstepInstance.stop(STOP_MODE.IMMEDIATE);
    }
}
