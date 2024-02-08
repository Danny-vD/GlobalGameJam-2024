using System;
using InputScripts;
using UnityEngine;
using UnityEngine.InputSystem;
using VDFramework.Singleton;
using VDFramework.UnityExtensions;

namespace Overworld.Navigation
{
    public class MenuInputController : Singleton<MenuInputController>
    {

        [SerializeField] private GameObject canvas;

        private void Start()
        {
            InputControlManager.Instance.playerControls.UIMenus.Navigation.performed += OnNavigationCancelled;
            InputControlManager.Instance.playerControls.UIMenus.Navigation.canceled += OnNavigation;

            InputControlManager.Instance.playerControls.UIMenus.Return.performed += OnInteract;
            InputControlManager.Instance.playerControls.UIMenus.Select.performed += OnSelect;
            InputControlManager.Instance.playerControls.UIMenus.Start.performed += OnStart;
            
            InputControlManager.Instance.OnMenuOpened += OnMenuOpened;
            InputControlManager.Instance.OnMenuClosed += OnMenuClosed;
        }

        private void OnMenuClosed()
        {
            canvas.SetActive(false);
        }

        private void OnMenuOpened()
        {
            canvas.SetActive(true);
        }

        private void OnDisable()
        {
            if (!InputControlManager.IsInitialized) return;
            
            InputControlManager.Instance.playerControls.UIMenus.Navigation.performed -= OnNavigationCancelled;
            InputControlManager.Instance.playerControls.UIMenus.Navigation.canceled -= OnNavigation;

            InputControlManager.Instance.playerControls.UIMenus.Return.performed -= OnInteract;
            InputControlManager.Instance.playerControls.UIMenus.Select.performed -= OnSelect;
            InputControlManager.Instance.playerControls.UIMenus.Start.performed -= OnStart;

            InputControlManager.Instance.OnMenuClosed -= OnMenuClosed;
            InputControlManager.Instance.OnMenuOpened -= OnMenuOpened;
        }

        private void OnNavigationCancelled(InputAction.CallbackContext obj)
        {
            
        }

        private void OnNavigation(InputAction.CallbackContext obj)
        {
            
        }

        private void OnInteract(InputAction.CallbackContext obj)
        {
           
        }

        private void OnSelect(InputAction.CallbackContext obj)
        {
            
        }

        private void OnStart(InputAction.CallbackContext obj)
        {
            InputControlManager.Instance.ChangeControls();
        }
    }
}