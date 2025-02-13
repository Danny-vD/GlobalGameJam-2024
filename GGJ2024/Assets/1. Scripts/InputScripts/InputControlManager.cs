using System;
using System.Collections.Generic;
using InputScripts.Enums;
using UnityEngine.InputSystem;
using VDFramework.Singleton;

//TODO: Listeners should unsubscribe when they are destroyed
namespace InputScripts
{
    public class InputControlManager : Singleton<InputControlManager>
    {
        private Dictionary<ControlTypes, InputActionMap> actionMapsByType;

        private ControlTypes beforeMenu;
        private ControlTypes currentType;
        public PlayerControls playerControls;

        protected override void Awake()
        {
            playerControls = new PlayerControls();

            // playerControls.UIMenus.Enable();

            actionMapsByType = new Dictionary<ControlTypes, InputActionMap>
            {
                { ControlTypes.Overworld, playerControls.Overworld.Get() },
                { ControlTypes.Menus, playerControls.UIMenus.Get() },
                { ControlTypes.Combat, playerControls.Combat.Get() },
                { ControlTypes.Dialogue, playerControls.DialogueInteraction.Get() }
            };

            beforeMenu = ControlTypes.Overworld;
            currentType = ControlTypes.Overworld;

            playerControls.UIMenus.Disable();
            playerControls.Overworld.Enable();

            base.Awake();
        }

        public event Action OnMenuOpened = delegate { };
        public event Action OnMenuClosed = delegate { };

        public void ChangeControls(ControlTypes type)
        {
            if (type == ControlTypes.Menus)
            {
                beforeMenu = currentType;
                OpenSettings();
            }

            actionMapsByType[currentType].Disable();
            actionMapsByType[type].Enable();
            currentType = type;
        }

        public void ChangeControls()
        {
            actionMapsByType[currentType].Disable();
            actionMapsByType[beforeMenu].Enable();

            currentType = beforeMenu;
            CloseSettings();
        }

        private void OpenSettings()
        {
            OnMenuOpened?.Invoke();
        }

        private void CloseSettings()
        {
            OnMenuClosed?.Invoke();
        }
    }
}