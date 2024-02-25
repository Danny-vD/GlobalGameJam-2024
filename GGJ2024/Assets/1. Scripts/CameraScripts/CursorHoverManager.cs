using CombatSystem.Events.CharacterSelection;
using UnityEngine;
using UtilityPackage.CursorManagement.CursorUtility;
using VDFramework.EventSystem;
using VDFramework.Singleton;
using VDFramework.Utility.TimerUtil;
using VDFramework.Utility.TimerUtil.TimerHandles;

namespace CameraScripts
{
    public class CursorHoverManager : Singleton<CursorHoverManager>
    {
        [SerializeField] private LayerMask layerMask;

        [SerializeField] [Tooltip("The interval in seconds between raycasts")]
        private float raycastInterval = 0.05f;

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
            rayCastTimerHandle = TimerManager.StartNewTimer(raycastInterval, CastRay, true);
        }

        private void OnDisable()
        {
            rayCastTimerHandle.Stop();
        }

        private void CastRay()
        {
            if (Physics.Raycast(MouseButtonUtil.GetMouseToWorldRay(cameraComponent), out var hitInfo, float.MaxValue,
                    layerMask))
            {
                if (hitInfo.collider.gameObject != currentlyHoveredObject)
                {
                    currentlyHoveredObject = hitInfo.collider.gameObject;
                    EventManager.RaiseEvent(new CharacterHoveredEvent(currentlyHoveredObject));
                }
            }
            else
            {
                if (currentlyHoveredObject)
                {
                    currentlyHoveredObject = null;
                    EventManager.RaiseEvent(new CharacterHoveredEvent(currentlyHoveredObject));
                }
            }
        }
    }
}