using InputScripts;
using InputScripts.Enums;
using UnityEngine;
using UnityEngine.InputSystem;
using VDFramework;

namespace Overworld.Navigation
{
	public class PlayerMovementController : SafeEnableBehaviour
	{
		[SerializeField, Tooltip("The speed of the player in m/s")]
		private float speed = 10;

		private bool isMoving;
		private Vector3 deltaMovement;

		private CharacterController controller;

		protected void Awake()
		{
			controller = GetComponent<CharacterController>();
		}

		protected override void OnEnabled()
		{
			InputControlManager.Instance.playerControls.Overworld.Movement.performed += MovementOnPerformed;
			InputControlManager.Instance.playerControls.Overworld.Movement.canceled  += MovementOnCanceled;

			InputControlManager.Instance.playerControls.Overworld.Interact.performed += OnInteract;
			InputControlManager.Instance.playerControls.Overworld.Select.performed   += OnSelect;
			InputControlManager.Instance.playerControls.Overworld.Start.performed    += OnStart;
		}

		private void OnDisable()
		{
			if (!InputControlManager.IsInitialized) return;

			InputControlManager.Instance.playerControls.Overworld.Movement.performed -= MovementOnPerformed;
			InputControlManager.Instance.playerControls.Overworld.Movement.canceled  -= MovementOnCanceled;
			
			InputControlManager.Instance.playerControls.Overworld.Interact.performed -= OnInteract;
			InputControlManager.Instance.playerControls.Overworld.Select.performed   -= OnSelect;
			InputControlManager.Instance.playerControls.Overworld.Start.performed    -= OnStart;
		}

		private void MovementOnCanceled(InputAction.CallbackContext obj)
		{
			isMoving = false;
		}

		private void MovementOnPerformed(InputAction.CallbackContext obj)
		{
			isMoving = true;

			Vector2 vector = obj.ReadValue<Vector2>();

			deltaMovement = new Vector3(vector.x, 0, vector.y);
		}

		private void OnSelect(InputAction.CallbackContext obj)
		{
		}

		private void OnInteract(InputAction.CallbackContext obj)
		{
		}

		private void OnStart(InputAction.CallbackContext obj)
		{
			Debug.Log("START PRESSED");
			InputControlManager.Instance.ChangeControls(ControlTypes.Menus);
		}

		// Update is called once per frame
		private void Update()
		{
			if (isMoving)
			{
				Vector3 delta = speed * deltaMovement;
				controller.SimpleMove(delta);
			}
		}
	}
}