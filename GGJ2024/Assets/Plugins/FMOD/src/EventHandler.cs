using UnityEngine;
using UnityEngine.EventSystems;

namespace FMODUnity
{
    public abstract class EventHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler,
        IPointerUpHandler
    {
        public string CollisionTag = "";

        protected virtual void Start()
        {
            HandleGameEvent(EmitterGameEvent.ObjectStart);
        }

        private void OnEnable()
        {
            HandleGameEvent(EmitterGameEvent.ObjectEnable);
        }

        private void OnDisable()
        {
            HandleGameEvent(EmitterGameEvent.ObjectDisable);
        }

        protected virtual void OnDestroy()
        {
            HandleGameEvent(EmitterGameEvent.ObjectDestroy);
        }

        private void OnCollisionEnter()
        {
            HandleGameEvent(EmitterGameEvent.CollisionEnter);
        }

        private void OnCollisionEnter2D()
        {
            HandleGameEvent(EmitterGameEvent.CollisionEnter2D);
        }

        private void OnCollisionExit()
        {
            HandleGameEvent(EmitterGameEvent.CollisionExit);
        }

        private void OnCollisionExit2D()
        {
            HandleGameEvent(EmitterGameEvent.CollisionExit2D);
        }

        private void OnMouseDown()
        {
            HandleGameEvent(EmitterGameEvent.ObjectMouseDown);
        }

        private void OnMouseEnter()
        {
            HandleGameEvent(EmitterGameEvent.ObjectMouseEnter);
        }

        private void OnMouseExit()
        {
            HandleGameEvent(EmitterGameEvent.ObjectMouseExit);
        }

        private void OnMouseUp()
        {
            HandleGameEvent(EmitterGameEvent.ObjectMouseUp);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            HandleGameEvent(EmitterGameEvent.UIMouseDown);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            HandleGameEvent(EmitterGameEvent.UIMouseEnter);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            HandleGameEvent(EmitterGameEvent.UIMouseExit);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            HandleGameEvent(EmitterGameEvent.UIMouseUp);
        }

        protected abstract void HandleGameEvent(EmitterGameEvent gameEvent);

#if UNITY_PHYSICS_EXIST
        private void OnTriggerEnter(Collider other)
        {
            if (string.IsNullOrEmpty(CollisionTag) || other.CompareTag(CollisionTag) ||
                (other.attachedRigidbody && other.attachedRigidbody.CompareTag(CollisionTag)))
                HandleGameEvent(EmitterGameEvent.TriggerEnter);
        }

        private void OnTriggerExit(Collider other)
        {
            if (string.IsNullOrEmpty(CollisionTag) || other.CompareTag(CollisionTag) ||
                (other.attachedRigidbody && other.attachedRigidbody.CompareTag(CollisionTag)))
                HandleGameEvent(EmitterGameEvent.TriggerExit);
        }
#endif

#if UNITY_PHYSICS2D_EXIST
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (string.IsNullOrEmpty(CollisionTag) || other.CompareTag(CollisionTag))
                HandleGameEvent(EmitterGameEvent.TriggerEnter2D);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (string.IsNullOrEmpty(CollisionTag) || other.CompareTag(CollisionTag))
                HandleGameEvent(EmitterGameEvent.TriggerExit2D);
        }
#endif
    }
}