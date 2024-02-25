using System.Collections;
using InputScripts;
using UnityEngine;
using UnityEngine.InputSystem;
using UtilityPackage.Utility.MathUtil;
using VDFramework;

namespace Overworld.Navigation
{
    public class PlayerMovementFlip : SafeEnableBehaviour
    {
        [SerializeField] [Tooltip("Y value 0 = facing Left\nY value 1 = facing right")]
        private InterpolationAlongCurve flippingCurve;

        [SerializeField] private bool isFacingRight = true;

        private float interpolationTime;

        private bool isFlipping;
        private Quaternion leftFacingRotation;

        private Quaternion rightFacingRotation;
        private bool shouldFaceRight;

        protected override void Start()
        {
            base.Start();
            shouldFaceRight = isFacingRight;

            if (isFacingRight)
            {
                interpolationTime = flippingCurve.MaxTime;

                rightFacingRotation = transform.rotation;
                leftFacingRotation = Quaternion.Euler(0, 180, 0) * rightFacingRotation;
            }
            else
            {
                interpolationTime = flippingCurve.MinTime;

                leftFacingRotation = transform.rotation;
                rightFacingRotation = Quaternion.Euler(0, 180, 0) * leftFacingRotation;
            }
        }

        private void OnDisable()
        {
            if (InputControlManager.IsInitialized)
                InputControlManager.Instance.playerControls.Overworld.Movement.performed -= OnMoving;
        }

        protected override void OnEnabled()
        {
            InputControlManager.Instance.playerControls.Overworld.Movement.performed += OnMoving;
        }

        private void OnMoving(InputAction.CallbackContext obj)
        {
            var input = obj.ReadValue<Vector2>();

            if (input.x.Equals(0)) // Don't flip for up/down movement
                return;

            SetFlipStateCorrect(input);
        }

        private void SetFlipStateCorrect(Vector2 movementDirection)
        {
            shouldFaceRight = movementDirection.x > 0;

            if (isFacingRight == shouldFaceRight) // Already facing the correct direction
                return;

            if (isFlipping) StopAllCoroutines();

            if (shouldFaceRight)
                StartCoroutine(FlipCharacterRight());
            else
                StartCoroutine(FlipCharacterLeft());
        }

        private Quaternion GetFlipRotation(float interpolationValue)
        {
            return Quaternion.Slerp(leftFacingRotation, rightFacingRotation, interpolationValue);
        }

        private IEnumerator FlipCharacterLeft()
        {
            isFacingRight = false;
            isFlipping = true;

            var targetTime = flippingCurve.MinTime;

            while (interpolationTime > targetTime)
            {
                interpolationTime -= Time.deltaTime;

                var interpolationValue = flippingCurve.EvaluateCurve(interpolationTime);

                transform.rotation = GetFlipRotation(interpolationValue);
                yield return null;
            }

            transform.rotation = GetFlipRotation(0);
            interpolationTime = targetTime;

            isFlipping = false;
        }

        private IEnumerator FlipCharacterRight()
        {
            isFacingRight = true;
            isFlipping = true;

            var targetTime = flippingCurve.MaxTime;

            while (interpolationTime < targetTime)
            {
                interpolationTime += Time.deltaTime;

                var interpolationValue = flippingCurve.EvaluateCurve(interpolationTime);

                transform.rotation = GetFlipRotation(interpolationValue);
                yield return null;
            }

            transform.rotation = GetFlipRotation(1);
            interpolationTime = targetTime;

            isFlipping = false;
        }
    }
}