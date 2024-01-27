
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using VDFramework;

public class MovementAnimation : BetterMonoBehaviour
{
    private Animator animator;
    private static readonly int IsWalking = Animator.StringToHash("isWalking");

    // Start is called before the first frame update
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        InputControlManager.Instance.playerControls.Overworld.Movement.performed += MovementOnPerformed;
        InputControlManager.Instance.playerControls.Overworld.Movement.canceled += MovementOnCanceled;
    }

    private void OnDisable()
    {
        if (!InputControlManager.IsInitialized) return;
        InputControlManager.Instance.playerControls.Overworld.Movement.performed -= MovementOnPerformed;
        InputControlManager.Instance.playerControls.Overworld.Movement.canceled -= MovementOnCanceled;

    }

    // Update is called once per frame
    private void MovementOnCanceled(InputAction.CallbackContext obj)
    {
      animator.SetBool(IsWalking, false);
    }

    private void MovementOnPerformed(InputAction.CallbackContext obj)
    {
        
        //-=obj.ReadValue<>()
        animator.SetBool(IsWalking, true);
    }
}
