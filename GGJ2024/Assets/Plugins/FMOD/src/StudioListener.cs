using System.Collections.Generic;
using FMOD;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace FMODUnity
{
    [AddComponentMenu("FMOD Studio/FMOD Studio Listener")]
    public class StudioListener : MonoBehaviour
    {
        private static readonly List<StudioListener> listeners = new();

        [SerializeField] private GameObject attenuationObject;

#if UNITY_PHYSICS_EXIST
        private Rigidbody rigidBody;
#endif
#if UNITY_PHYSICS2D_EXIST
        private Rigidbody2D rigidBody2D;
#endif

        public static int ListenerCount => listeners.Count;

        public int ListenerNumber => listeners.IndexOf(this);

        private void Update()
        {
            if (ListenerNumber >= 0 && ListenerNumber < CONSTANTS.MAX_LISTENERS) SetListenerLocation();
        }

        private void OnEnable()
        {
            RuntimeUtils.EnforceLibraryOrder();
#if UNITY_PHYSICS_EXIST
            rigidBody = gameObject.GetComponent<Rigidbody>();
#endif
#if UNITY_PHYSICS2D_EXIST
            rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
#endif
            AddListener(this);
        }

        private void OnDisable()
        {
            RemoveListener(this);
        }

        public static float DistanceToNearestListener(Vector3 position)
        {
            var result = float.MaxValue;
            for (var i = 0; i < listeners.Count; i++)
                result = Mathf.Min(result, Vector3.Distance(position, listeners[i].transform.position));
            return result;
        }

        public static float DistanceSquaredToNearestListener(Vector3 position)
        {
            var result = float.MaxValue;
            for (var i = 0; i < listeners.Count; i++)
                result = Mathf.Min(result, (position - listeners[i].transform.position).sqrMagnitude);
            return result;
        }

        private static void AddListener(StudioListener listener)
        {
            // Is the listener already in the list?
            if (listeners.Contains(listener))
            {
                Debug.LogWarning(string.Format("[FMOD] Listener has already been added at index {0}.",
                    listener.ListenerNumber));
                return;
            }

            // If already at the max numListeners
            if (listeners.Count >= CONSTANTS.MAX_LISTENERS)
                Debug.LogWarning(
                    string.Format("[FMOD] Max number of listeners reached : {0}.", CONSTANTS.MAX_LISTENERS));

            listeners.Add(listener);
            RuntimeManager.StudioSystem.setNumListeners(Mathf.Clamp(listeners.Count, 1, CONSTANTS.MAX_LISTENERS));
        }

        private static void RemoveListener(StudioListener listener)
        {
            listeners.Remove(listener);
            RuntimeManager.StudioSystem.setNumListeners(Mathf.Clamp(listeners.Count, 1, CONSTANTS.MAX_LISTENERS));
        }

        private void SetListenerLocation()
        {
#if UNITY_PHYSICS_EXIST
            if (rigidBody)
                RuntimeManager.SetListenerLocation(ListenerNumber, gameObject, rigidBody, attenuationObject);
            else
#endif
#if UNITY_PHYSICS2D_EXIST
            if (rigidBody2D)
                RuntimeManager.SetListenerLocation(ListenerNumber, gameObject, rigidBody2D, attenuationObject);
            else
#endif
                RuntimeManager.SetListenerLocation(ListenerNumber, gameObject, attenuationObject);
        }
    }
}