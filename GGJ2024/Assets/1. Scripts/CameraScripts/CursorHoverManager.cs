using System;
using CombatSystem.Events.CharacterSelection;
using InputScripts;
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
            MouseButtonUtil.OnLeftMouseButtonUp += CastRay;
        }

        private void OnDisable()
        {
            MouseButtonUtil.OnLeftMouseButtonUp -= CastRay;
        }

        private void CastRay()
        {
            var a = MouseButtonUtil.GetMouseToWorldRay(cameraComponent);
            
            Debug.Log(a.origin);
            Debug.DrawRay(a.origin, a.direction, Color.red);

            if (!Physics.Raycast(a, out var hitInfo, float.MaxValue,
                    layerMask)) return;
            
            
            
            currentlyHoveredObject = hitInfo.collider.gameObject;
            EventManager.RaiseEvent(new CharacterHoveredEvent(currentlyHoveredObject));
        }

        private void Update()
        {
            var a = MouseButtonUtil.GetMouseToWorldRay(cameraComponent);
            Debug.DrawLine(a.origin, a.origin + (a.direction * 200), Color.red);
        }
    }
}