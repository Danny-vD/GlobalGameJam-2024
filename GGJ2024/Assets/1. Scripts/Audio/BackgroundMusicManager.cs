using CombatSystem.Events;
using FMODUtilityPackage.Core;
using FMODUtilityPackage.Enums;
using VDFramework;
using VDFramework.EventSystem;
using VDFramework.Extensions;

namespace Audio
{
    public class BackgroundMusicManager : BetterMonoBehaviour
    {
        // Start is called before the first frame update
        private void Start()
        {
            AudioPlayer.PlayEmitter(EmitterType.BackgroundMusic);
        }
    
        private void OnEnable()
        {   
            EventManager.AddListener<CombatEndedEvent>(CombatEndMusic);
            EventManager.AddListener<CombatStartedEvent>(CombatMusic);
        }

        private void OnDisable()
        {
            EventManager.RemoveListener<CombatEndedEvent>(CombatEndMusic);
            EventManager.RemoveListener<CombatStartedEvent>(CombatMusic);
        }

        private void CombatMusic(CombatStartedEvent @event)
        {
            PlayEmitter(EmitterType.BattleMusic);
        }

        private static void PlayEmitter(EmitterType toPlay)
        {
            foreach (EmitterType emitterType in default(EmitterType).GetValues())
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
            PlayEmitter(EmitterType.BossMusic);
        }

        private void EndMusic()
        {
            PlayEmitter(EmitterType.VictoryMusic);
        }

        private static void CombatEndMusic(CombatEndedEvent @event)
        {
            PlayEmitter(EmitterType.BackgroundMusic);   
        }
    }
}
