using UI;
using UnityEngine;
using UnityEngine.InputSystem;
using VDFramework.Singleton;

namespace Overworld.Navigation
{
	public class PlayerMovementController : Singleton<PlayerMovementController>
	{
		[SerializeField, Tooltip("The speed of the player in m/s")]
		private float speed;

		private bool isMoving;
		private Vector3 deltaMovement;

		private CharacterController controller;

		protected override void Awake()
		{
			base.Awake();
			controller = GetComponent<CharacterController>();
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

		// Start is called before the first frame update
		private void Start()
		{
			InputControlManager.Instance.playerControls.Overworld.Movement.performed += MovementOnPerformed;
			InputControlManager.Instance.playerControls.Overworld.Movement.canceled  += MovementOnCanceled;

			InputControlManager.Instance.playerControls.Overworld.Interact.performed += OnInteract;
			InputControlManager.Instance.playerControls.Overworld.Select.performed   += OnSelect;
			InputControlManager.Instance.playerControls.Overworld.Start.performed    += OnStart;
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