using System.Collections;
using System.Collections.Generic;
using FMODUtilityPackage.Core;
using FMODUtilityPackage.Enums;
using UnityEngine;
using EventType = FMODUtilityPackage.Enums.EventType;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        AudioPlayer.SetEmitterEvent(EmitterType.BackgroundMusic, EventType.Music_Hell);
        AudioPlayer.PlayEmitter(EmitterType.BackgroundMusic);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
