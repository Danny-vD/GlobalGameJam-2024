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
		[SerializeField]
		private LayerMask layerMask;

		[SerializeField, Tooltip("The interval in seconds between raycasts")]
		private float raycastInterval = 0.05f;

		private Camera mainCamera;

		private TimerHandle rayCastTimerHandle;

		protected override void Awake()
		{
			base.Awake();
			mainCamera = GetComponent<Camera>();
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
			if (Physics.Raycast(MouseButtonUtil.GetMouseToWorldRay(mainCamera), out RaycastHit hitInfo, float.MaxValue, layerMask))
			{
				EventManager.RaiseEvent(new CharacterHoveredEvent(hitInfo.collider.gameObject));
			}
		}
	}
}