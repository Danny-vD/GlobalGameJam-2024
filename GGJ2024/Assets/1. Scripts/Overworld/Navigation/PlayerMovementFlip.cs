using UI;
using UnityEngine;
using UnityEngine.InputSystem;
using VDFramework;

namespace Overworld.Navigation
{
	public class PlayerMovementFlip : SafeEnableBehaviour
	{
		private bool isFacingLeft;

		protected override void OnEnabled()
		{
			InputControlManager.Instance.playerControls.Overworld.Movement.performed += OnMoving;
		}

		private void OnDisable()
		{
			InputControlManager.Instance.playerControls.Overworld.Movement.performed -= OnMoving;
		}

		private void OnMoving(InputAction.CallbackContext obj)
		{
			Vector2 input = obj.ReadValue<Vector2>();

			SetFlipStateCorrect(input);
		}

		private void SetFlipStateCorrect(Vector2 movementDirection)
		{
			bool shouldFaceLeft = movementDirection.x < 0;

			if (isFacingLeft != shouldFaceLeft)
			{
				transform.Rotate(Vector3.up, 180);
				isFacingLeft = shouldFaceLeft;
			}
		}
	}
}