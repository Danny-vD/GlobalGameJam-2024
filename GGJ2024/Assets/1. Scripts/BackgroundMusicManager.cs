using System;
using System.Collections.Generic;
using CombatSystem.Events;
using FMODUtilityPackage.Core;
using FMODUtilityPackage.Enums;
using VDFramework;
using VDFramework.EventSystem;
using VDFramework.Extensions;
using VDFramework.Utility;

public class BackgroundMusicManager : BetterMonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        AudioPlayer.SetEmitterEvent(EmitterType.BackgroundMusic, AudioEventType.Music_Hell);
        AudioPlayer.PlayEmitter(EmitterType.BackgroundMusic);
        
        AudioPlayer.SetEmitterEvent(EmitterType.VictoryMusic, AudioEventType.Music_BattleVictory);
        AudioPlayer.SetEmitterEvent(EmitterType.BattleMusic, AudioEventType.Music_Battle);
        
    }
    
    private void OnEnable()
    {   
        EventManager.AddListener<CombatEndedEvent>(CombatEndMusic);
        EventManager.AddListener<CombatStartedEvent>(CombatMusic);
    }

    private void CombatMusic(CombatStartedEvent @event)
    {
        PlayEmitter(EmitterType.BattleMusic);
    }

    private static void PlayEmitter(EmitterType toPlay)
    {
        foreach (var emitterType in EmitterType.BackgroundMusic.GetValues())
        {
            if (emitterType == toPlay)
            {
                AudioPlayer.PlayEmitter(toPlay);
            }
            else
            {
                AudioPlayer.StopEmitter(emitterType);
            }
        }
    }

    private void BossMusic()
    {
        
    }

    private void EndMusic()
    {
        
    }

    private void CombatEndMusic(CombatEndedEvent @event)
    {
        PlayEmitter(EmitterType.BackgroundMusic);
    }
}
