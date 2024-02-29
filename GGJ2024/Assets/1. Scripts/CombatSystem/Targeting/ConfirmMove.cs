using System;
using UnityEngine.UI;
using VDFramework;

namespace CombatSystem.Targeting
{
    public class ConfirmMove : BetterMonoBehaviour
    {
        private Button confirmButton;
        private void Awake()
        {
            confirmButton = GetComponent<Button>();
            confirmButton.onClick.AddListener(CombatTargettingManager.Instance.OnTargetSelectConfirm);
        }
        
    }
}