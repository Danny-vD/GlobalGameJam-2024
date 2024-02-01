using System;
using System.Collections;
using System.Collections.Generic;
using CombatSystem.CharacterScripts;
using CombatSystem.Events;
using UI;
using UnityEngine;
using VDFramework.EventSystem;
using VDFramework.UnityExtensions;

public class CombatManager : MonoBehaviour
{
    // Start is called before the first frame update
    
    private void OnEnable()
    {
        EventManager.AddListener<CombatStartedEvent>(OnCombatStart);
        EventManager.AddListener<CombatEndedEvent>(OnCombatEnd);
    }

    private void OnCombatStart(CombatStartedEvent @event)
    {
        InputControlManager.Instance.ChangeControls(ControlTypes.Combat);
        foreach (var eventEnemy in @event.Enemies)
        {
            eventEnemy.GetComponent<CharacterStateManager>().Enable();
        }
        
    }

    private void OnCombatEnd(CombatEndedEvent @event)
    {
        InputControlManager.Instance.ChangeControls(ControlTypes.Overworld);
        //TODO: Combat End Function?
    }
}

