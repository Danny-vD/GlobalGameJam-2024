using CombatSystem.Events.CharacterSelection;
using UnityEngine;
using UtilityPackage.CursorManagement.CursorUtility;
using VDFramework.EventSystem;
using VDFramework.Singleton;
using VDFramework.Utility.TimerUtil.TimerHandles;

namespace CameraScripts
{
    public class CursorClickManager : Singleton<CursorClickManager>
    {
        [SerializeField] 
        private LayerMask clickableLayers;

        private Camera cameraComponent;

        private GameObject currentlyHoveredObject;

        private TimerHandle rayCastTimerHandle;

        protected override void Awake()
        {
            base.Awake();
            cameraComponent = GetComponent<Camera>();
            currentlyHoveredObject = null;
        }

        private void OnEnable()
        {
            MouseButtonUtil.OnLeftMouseButtonUp += CastRay;
        }

        private void OnDisable()
        {
            MouseButtonUtil.OnLeftMouseButtonUp -= CastRay;
        }

        private void CastRay()
        {
            if (!Physics.Raycast(MouseButtonUtil.GetMouseToWorldRay(cameraComponent), out RaycastHit hitInfo, float.MaxValue, clickableLayers))
            {
                return;
            }
            
            currentlyHoveredObject = hitInfo.collider.gameObject;
            EventManager.RaiseEvent(new CharacterClickedEvent(currentlyHoveredObject));
        }
    }
}