using FMOD.Studio;
using FMODUtilityPackage.Core;
using FMODUtilityPackage.Enums;
using FMODUtilityPackage.Interfaces;
using UnityEngine;
using VDFramework;

namespace FMODUtilityPackage.Audioplayers
{
    public class AudioPlayerComponent : BetterMonoBehaviour, IAudioplayer
    {
        [SerializeField] private AudioEventType audioEventType;

        private EventInstance eventInstance;

        private void Start()
        {
            CacheEventInstance();
        }

        private void OnDestroy()
        {
            eventInstance.release();
        }

        public void Play()
        {
            eventInstance.start();
        }

        public void PlayIfNotPlaying()
        {
            eventInstance.getPlaybackState(out var state);

            if (state is PLAYBACK_STATE.STOPPED or PLAYBACK_STATE.STOPPING) eventInstance.start();
        }

        public void SetPause(bool paused)
        {
            eventInstance.setPaused(paused);
        }

        public void Stop()
        {
            Stop(STOP_MODE.ALLOWFADEOUT);
        }

        public void SetEventType(AudioEventType newAudioEventType)
        {
            audioEventType = newAudioEventType;
            CacheEventInstance();
        }

        public void Stop(STOP_MODE stopMode)
        {
            eventInstance.stop(stopMode);
        }

        private void CacheEventInstance()
        {
            eventInstance.release();
            eventInstance = AudioPlayer.GetEventInstance(audioEventType);
        }
    }
}