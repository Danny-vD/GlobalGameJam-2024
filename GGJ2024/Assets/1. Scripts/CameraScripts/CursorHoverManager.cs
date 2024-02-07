using CombatSystem.Events.CharacterSelection;
using UnityEngine;
using UtilityPackage.CursorManagement.CursorUtility;
using VDFramework.EventSystem;
using VDFramework.Singleton;

namespace CameraScripts
{
	public class CursorHoverManager : Singleton<CursorHoverManager> // NOTE unused after discovery that HoverOverCharacterHandler.OnMouseEnter() works fine
	{
		[SerializeField]
		private LayerMask layerMask;

		[SerializeField]
		private float raycastDistance = 1000000f;

		private Camera mainCamera;

		protected override void Awake()
		{
			base.Awake();
			mainCamera = GetComponent<Camera>();
		}

		private void Update()
		{
			// TODO: Frames?
			//CreateRaycast();
		}

		private void CreateRaycast()
		{
			if (Physics.Raycast(MouseButtonUtil.GetMouseWorldPosition(mainCamera), transform.forward, out RaycastHit raycastHitInfo, raycastDistance, layerMask))
			{
				EventManager.RaiseEvent(new CharacterHoveredEvent(raycastHitInfo.collider.gameObject));
			}
		}
	}
}