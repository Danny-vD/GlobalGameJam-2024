using System;
using System.Collections;
using System.Collections.Generic;
using CharacterScripts;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using VDFramework;
using VDFramework.Extensions;
using VDFramework.Singleton;

public class PlayerMovementController : Singleton<PlayerMovementController>
{
    [SerializeField] private float speed;

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
        InputControlManager.Instance.playerControls.Overworld.Movement.canceled -= MovementOnCanceled;
        InputControlManager.Instance.playerControls.Overworld.Interact.performed -= OnInteract;
        InputControlManager.Instance.playerControls.Overworld.Select.performed -= OnSelect;
        InputControlManager.Instance.playerControls.Overworld.Start.performed -= OnStart;
    }

    private void MovementOnCanceled(InputAction.CallbackContext obj)
    {
        isMoving = false;
    }

    private void MovementOnPerformed(InputAction.CallbackContext obj)
    {
        isMoving = true;

        var vector = obj.ReadValue<Vector2>();

        Debug.Log($"{vector} [{vector.magnitude}]");
        deltaMovement = new Vector3(vector.x * Time.deltaTime * speed, 0, vector.y * Time.deltaTime * speed);
    }

    // Start is called before the first frame update
    private void Start()
    {
        InputControlManager.Instance.playerControls.Overworld.Movement.performed += MovementOnPerformed;
        InputControlManager.Instance.playerControls.Overworld.Movement.canceled += MovementOnCanceled;

        InputControlManager.Instance.playerControls.Overworld.Interact.performed += OnInteract;
        InputControlManager.Instance.playerControls.Overworld.Select.performed += OnSelect;
        InputControlManager.Instance.playerControls.Overworld.Start.performed += OnStart;
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
            controller.Move(deltaMovement);
        }
    }
}