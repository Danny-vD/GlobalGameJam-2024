#if UNITY_TIMELINE_EXIST

using System;
using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;
using UnityEngine.Timeline;

namespace FMODUnity
{
    [Serializable]
    public class FMODEventPlayable : PlayableAsset, ITimelineClipAsset
    {
        [FormerlySerializedAs("template")] public FMODEventPlayableBehavior Template = new();

        [FormerlySerializedAs("eventLength")] public float EventLength; //In seconds.

        [Obsolete("Use the eventReference field instead")] [SerializeField]
        public string eventName;

        [FormerlySerializedAs("eventReference")] [SerializeField]
        public EventReference EventReference;

        [FormerlySerializedAs("stopType")] [SerializeField]
        public STOP_MODE StopType;

        [FormerlySerializedAs("parameters")] [SerializeField]
        public ParamRef[] Parameters = new ParamRef[0];

        private FMODEventPlayableBehavior behavior;

        [NonSerialized] public bool CachedParameters;

        public GameObject TrackTargetObject { get; set; }

        public override double duration
        {
            get
            {
                if (EventReference.IsNull)
                    return base.duration;
                return EventLength;
            }
        }

        public TimelineClip OwningClip { get; set; }

        public ClipCaps clipCaps => ClipCaps.None;

        public static event EventHandler<EventArgs> OnCreatePlayable;

        public void LinkParameters(EventDescription eventDescription)
        {
#if UNITY_EDITOR
            if (!EventReference.IsNull)
#else
            if (!CachedParameters && !EventReference.IsNull)
#endif
            {
                for (var i = 0; i < Parameters.Length; i++)
                {
                    PARAMETER_DESCRIPTION parameterDescription;
                    eventDescription.getParameterDescriptionByName(Parameters[i].Name, out parameterDescription);
                    Parameters[i].ID = parameterDescription.id;
                }

                var parameterLinks = Template.ParameterLinks;

                for (var i = 0; i < parameterLinks.Count; i++)
                {
                    PARAMETER_DESCRIPTION parameterDescription;
                    eventDescription.getParameterDescriptionByName(parameterLinks[i].Name, out parameterDescription);
                    parameterLinks[i].ID = parameterDescription.id;
                }

                CachedParameters = true;
            }
        }

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            if (Application.isPlaying)
            {
                LinkParameters(RuntimeManager.GetEventDescription(EventReference));
            }
            else
            {
                // Handled by the editor auditioning system.
                var args = new EventArgs();
                OnCreatePlayable.Invoke(this, args);
            }

            var playable = ScriptPlayable<FMODEventPlayableBehavior>.Create(graph, Template);
            behavior = playable.GetBehaviour();

            behavior.TrackTargetObject = TrackTargetObject;
            behavior.EventReference = EventReference;
            behavior.StopType = StopType;
            behavior.Parameters = Parameters;
            behavior.OwningClip = OwningClip;

            return playable;
        }

#if UNITY_EDITOR
        public void UpdateEventDuration(float duration)
        {
            EventLength = duration / 1000f;
        }

        public void OnValidate()
        {
            if (OwningClip != null)
            {
                if (EventReference.IsNull)
                {
                    OwningClip.displayName = "FMODEventPlayable";
                }
                else
                {
                    var index = EventReference.Path.LastIndexOf("/");
                    OwningClip.displayName = EventReference.Path.Substring(index + 1);
                }
            }

            if (behavior != null) behavior.EventReference = EventReference;
        }
#endif //UNITY_EDITOR
    }

    public enum STOP_MODE
    {
        AllowFadeout,
        Immediate,
        None
    }

    [Serializable]
    public class ParameterAutomationLink
    {
        public string Name;
        public int Slot;
        public PARAMETER_ID ID;
    }

    [Serializable]
    public class FMODEventPlayableBehavior : PlayableBehaviour
    {
        [FormerlySerializedAs("eventReference")]
        public EventReference EventReference;

        [FormerlySerializedAs("stopType")] public STOP_MODE StopType = STOP_MODE.AllowFadeout;

        [FormerlySerializedAs("parameters")] [NotKeyable]
        public ParamRef[] Parameters = new ParamRef[0];

        [FormerlySerializedAs("parameterLinks")]
        public List<ParameterAutomationLink> ParameterLinks = new();

        [FormerlySerializedAs("parameterAutomation")]
        public AutomatableSlots ParameterAutomation;

        private EventInstance eventInstance;

        private bool isPlayheadInside;

        [NonSerialized] public TimelineClip OwningClip;

        [NonSerialized] public GameObject TrackTargetObject;

        public FMODEventPlayableBehavior()
        {
            CurrentVolume = 1;
        }

        public float ClipStartTime { get; private set; }

        public float CurrentVolume { get; private set; }

        public static event EventHandler<EventArgs> Enter;
        public static event EventHandler<EventArgs> Exit;
        public static event EventHandler<EventArgs> GraphStop;

        protected void PlayEvent()
        {
            if (!EventReference.IsNull)
            {
                eventInstance = RuntimeManager.CreateInstance(EventReference);

                // Only attach to object if the game is actually playing, not auditioning.
                if (Application.isPlaying && TrackTargetObject)
                {
#if UNITY_PHYSICS_EXIST
                    if (TrackTargetObject.GetComponent<Rigidbody>())
                        RuntimeManager.AttachInstanceToGameObject(eventInstance, TrackTargetObject.transform,
                            TrackTargetObject.GetComponent<Rigidbody>());
                    else
#endif
#if UNITY_PHYSICS2D_EXIST
                    if (TrackTargetObject.GetComponent<Rigidbody2D>())
                        RuntimeManager.AttachInstanceToGameObject(eventInstance, TrackTargetObject.transform,
                            TrackTargetObject.GetComponent<Rigidbody2D>());
                    else
#endif
                        RuntimeManager.AttachInstanceToGameObject(eventInstance, TrackTargetObject.transform);
                }
                else
                {
                    eventInstance.set3DAttributes(Vector3.zero.To3DAttributes());
                }

                foreach (var param in Parameters) eventInstance.setParameterByID(param.ID, param.Value);

                eventInstance.setVolume(CurrentVolume);
                eventInstance.setTimelinePosition((int)(ClipStartTime * 1000.0f));
                eventInstance.start();
            }
        }

        protected virtual void OnEnter()
        {
            if (!isPlayheadInside)
            {
                isPlayheadInside = true;

                if (Application.isPlaying)
                {
                    PlayEvent();
                }
                else
                {
                    // Handled by the editor auditioning system.
                    var args = new EventArgs();
                    Enter.Invoke(this, args);
                    eventInstance = args.eventInstance;
                }
            }
        }

        protected virtual void OnExit()
        {
            if (isPlayheadInside)
            {
                isPlayheadInside = false;

                if (Application.isPlaying)
                {
                    if (eventInstance.isValid())
                    {
                        if (StopType != STOP_MODE.None)
                            eventInstance.stop(StopType == STOP_MODE.Immediate
                                ? FMOD.Studio.STOP_MODE.IMMEDIATE
                                : FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                        eventInstance.release();
                        eventInstance.clearHandle();
                    }
                }
                else
                {
                    // Handled by the editor auditioning system.
                    var args = new EventArgs();
                    args.eventInstance = eventInstance;
                    Exit.Invoke(this, args);
                }
            }
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (eventInstance.isValid())
                foreach (var link in ParameterLinks)
                {
                    var value = ParameterAutomation.GetValue(link.Slot);
                    eventInstance.setParameterByID(link.ID, value);
                }
        }

        public void UpdateBehavior(float time, float volume)
        {
            if (volume != CurrentVolume)
            {
                CurrentVolume = volume;

                if (eventInstance.isValid()) eventInstance.setVolume(volume);
            }

            if (time >= OwningClip.start && time < OwningClip.end)
            {
                ClipStartTime = time - (float)OwningClip.start;
                OnEnter();
            }
            else
            {
                OnExit();
            }
        }

        public override void OnGraphStop(Playable playable)
        {
            isPlayheadInside = false;

            if (Application.isPlaying)
            {
                if (eventInstance.isValid())
                {
                    eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                    eventInstance.release();
                    RuntimeManager.StudioSystem.update();
                }
            }
            else
            {
                // Handled by the editor auditioning system.
                var args = new EventArgs();
                args.eventInstance = eventInstance;
                GraphStop.Invoke(this, args);
            }
        }

        public class EventArgs : System.EventArgs
        {
            public EventInstance eventInstance { get; set; }
        }
    }
}
#endif