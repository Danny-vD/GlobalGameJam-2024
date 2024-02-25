using System;
using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;

namespace FMODUnity
{
    [AddComponentMenu("FMOD Studio/FMOD Studio Event Emitter")]
    public class StudioEventEmitter : EventHandler
    {
        private const string SnapshotString = "snapshot";

        private static readonly List<StudioEventEmitter> activeEmitters = new();
        public EventReference EventReference;

        [Obsolete("Use the EventReference field instead")]
        public string Event = "";

        public EmitterGameEvent PlayEvent = EmitterGameEvent.None;
        public EmitterGameEvent StopEvent = EmitterGameEvent.None;
        public bool AllowFadeout = true;
        public bool TriggerOnce;
        public bool Preload;
        public bool AllowNonRigidbodyDoppler;
        public ParamRef[] Params = new ParamRef[0];
        public bool OverrideAttenuation;
        public float OverrideMinDistance = -1.0f;
        public float OverrideMaxDistance = -1.0f;
        private readonly List<ParamRef> cachedParams = new();

        protected EventDescription eventDescription;

        private bool hasTriggered;

        protected EventInstance instance;
        private bool isOneshot;
        private bool isQuitting;

        public EventDescription EventDescription => eventDescription;

        public EventInstance EventInstance => instance;

        public bool IsActive { get; private set; }

        private float MaxDistance
        {
            get
            {
                if (OverrideAttenuation) return OverrideMaxDistance;

                if (!eventDescription.isValid()) Lookup();

                float minDistance, maxDistance;
                eventDescription.getMinMaxDistance(out minDistance, out maxDistance);
                return maxDistance;
            }
        }

        protected override void Start()
        {
            RuntimeUtils.EnforceLibraryOrder();
            if (Preload)
            {
                Lookup();
                eventDescription.loadSampleData();
            }

            HandleGameEvent(EmitterGameEvent.ObjectStart);

            // If a Rigidbody is added, turn off "allowNonRigidbodyDoppler" option
#if UNITY_PHYSICS_EXIST
            if (AllowNonRigidbodyDoppler && GetComponent<Rigidbody>()) AllowNonRigidbodyDoppler = false;
#endif
        }

        protected override void OnDestroy()
        {
            if (!isQuitting)
            {
                HandleGameEvent(EmitterGameEvent.ObjectDestroy);

                if (instance.isValid())
                {
                    RuntimeManager.DetachInstanceFromGameObject(instance);

                    if (eventDescription.isValid() && isOneshot)
                    {
                        instance.release();
                        instance.clearHandle();
                    }
                }

                DeregisterActiveEmitter(this);

                if (Preload) eventDescription.unloadSampleData();
            }
        }

        private void OnApplicationQuit()
        {
            isQuitting = true;
        }

        public static void UpdateActiveEmitters()
        {
            foreach (var emitter in activeEmitters) emitter.UpdatePlayingStatus();
        }

        private static void RegisterActiveEmitter(StudioEventEmitter emitter)
        {
            if (!activeEmitters.Contains(emitter)) activeEmitters.Add(emitter);
        }

        private static void DeregisterActiveEmitter(StudioEventEmitter emitter)
        {
            activeEmitters.Remove(emitter);
        }

        private void UpdatePlayingStatus(bool force = false)
        {
            // If at least one listener is within the max distance, ensure an event instance is playing
            var playInstance = StudioListener.DistanceSquaredToNearestListener(transform.position) <=
                               MaxDistance * MaxDistance;

            if (force || playInstance != IsPlaying())
            {
                if (playInstance)
                    PlayInstance();
                else
                    StopInstance();
            }
        }

        protected override void HandleGameEvent(EmitterGameEvent gameEvent)
        {
            if (PlayEvent == gameEvent) Play();
            if (StopEvent == gameEvent) Stop();
        }

        private void Lookup()
        {
            eventDescription = RuntimeManager.GetEventDescription(EventReference);

            if (eventDescription.isValid())
                for (var i = 0; i < Params.Length; i++)
                {
                    PARAMETER_DESCRIPTION param;
                    eventDescription.getParameterDescriptionByName(Params[i].Name, out param);
                    Params[i].ID = param.id;
                }
        }

        public void Play()
        {
            if (TriggerOnce && hasTriggered) return;

            if (EventReference.IsNull) return;

            cachedParams.Clear();

            if (!eventDescription.isValid()) Lookup();

            bool isSnapshot;
            eventDescription.isSnapshot(out isSnapshot);

            if (!isSnapshot) eventDescription.isOneshot(out isOneshot);

            bool is3D;
            eventDescription.is3D(out is3D);

            IsActive = true;

            if (is3D && !isOneshot && Settings.Instance.StopEventsOutsideMaxDistance)
            {
                RegisterActiveEmitter(this);
                UpdatePlayingStatus(true);
            }
            else
            {
                PlayInstance();
            }
        }

        private void PlayInstance()
        {
            if (!instance.isValid()) instance.clearHandle();

            // Let previous oneshot instances play out
            if (isOneshot && instance.isValid())
            {
                instance.release();
                instance.clearHandle();
            }

            bool is3D;
            eventDescription.is3D(out is3D);

            if (!instance.isValid())
            {
                eventDescription.createInstance(out instance);

                // Only want to update if we need to set 3D attributes
                if (is3D)
                {
                    var transform = GetComponent<Transform>();
#if UNITY_PHYSICS_EXIST
                    if (GetComponent<Rigidbody>())
                    {
                        var rigidBody = GetComponent<Rigidbody>();
                        instance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject, rigidBody));
                        RuntimeManager.AttachInstanceToGameObject(instance, transform, rigidBody);
                    }
                    else
#endif
#if UNITY_PHYSICS2D_EXIST
                    if (GetComponent<Rigidbody2D>())
                    {
                        var rigidBody2D = GetComponent<Rigidbody2D>();
                        instance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject, rigidBody2D));
                        RuntimeManager.AttachInstanceToGameObject(instance, transform, rigidBody2D);
                    }
                    else
#endif
                    {
                        instance.set3DAttributes(gameObject.To3DAttributes());
                        RuntimeManager.AttachInstanceToGameObject(instance, transform, AllowNonRigidbodyDoppler);
                    }
                }
            }

            foreach (var param in Params) instance.setParameterByID(param.ID, param.Value);

            foreach (var cachedParam in cachedParams) instance.setParameterByID(cachedParam.ID, cachedParam.Value);

            if (is3D && OverrideAttenuation)
            {
                instance.setProperty(EVENT_PROPERTY.MINIMUM_DISTANCE, OverrideMinDistance);
                instance.setProperty(EVENT_PROPERTY.MAXIMUM_DISTANCE, OverrideMaxDistance);
            }

            instance.start();

            hasTriggered = true;
        }

        public void Stop()
        {
            DeregisterActiveEmitter(this);
            IsActive = false;
            cachedParams.Clear();
            StopInstance();
        }

        private void StopInstance()
        {
            if (TriggerOnce && hasTriggered) DeregisterActiveEmitter(this);

            if (instance.isValid())
            {
                instance.stop(AllowFadeout ? FMOD.Studio.STOP_MODE.ALLOWFADEOUT : FMOD.Studio.STOP_MODE.IMMEDIATE);
                instance.release();
                if (!AllowFadeout) instance.clearHandle();
            }
        }

        public void SetParameter(string name, float value, bool ignoreseekspeed = false)
        {
            if (Settings.Instance.StopEventsOutsideMaxDistance && IsActive)
            {
                var findName = name;
                var cachedParam = cachedParams.Find(x => x.Name == findName);

                if (cachedParam == null)
                {
                    PARAMETER_DESCRIPTION paramDesc;
                    eventDescription.getParameterDescriptionByName(name, out paramDesc);

                    cachedParam = new ParamRef();
                    cachedParam.ID = paramDesc.id;
                    cachedParam.Name = paramDesc.name;
                    cachedParams.Add(cachedParam);
                }

                cachedParam.Value = value;
            }

            if (instance.isValid()) instance.setParameterByName(name, value, ignoreseekspeed);
        }

        public void SetParameter(PARAMETER_ID id, float value, bool ignoreseekspeed = false)
        {
            if (Settings.Instance.StopEventsOutsideMaxDistance && IsActive)
            {
                var findId = id;
                var cachedParam = cachedParams.Find(x => x.ID.Equals(findId));

                if (cachedParam == null)
                {
                    PARAMETER_DESCRIPTION paramDesc;
                    eventDescription.getParameterDescriptionByID(id, out paramDesc);

                    cachedParam = new ParamRef();
                    cachedParam.ID = paramDesc.id;
                    cachedParam.Name = paramDesc.name;
                    cachedParams.Add(cachedParam);
                }

                cachedParam.Value = value;
            }

            if (instance.isValid()) instance.setParameterByID(id, value, ignoreseekspeed);
        }

        public bool IsPlaying()
        {
            if (instance.isValid())
            {
                PLAYBACK_STATE playbackState;
                instance.getPlaybackState(out playbackState);
                return playbackState != PLAYBACK_STATE.STOPPED;
            }

            return false;
        }
    }
}