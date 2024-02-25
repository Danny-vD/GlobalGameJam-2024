using CombatSystem.CharacterScripts;
using CombatSystem.Events;
using InputScripts;
using InputScripts.Enums;
using VDFramework;
using VDFramework.EventSystem;
using VDFramework.UnityExtensions;

namespace CombatSystem.Managers
{
    public class CombatManager : BetterMonoBehaviour
    {
        // Start is called before the first frame update

        private void OnEnable()
        {
            EventManager.AddListener<CombatStartedEvent>(OnCombatStart);
            EventManager.AddListener<CombatEndedEvent>(OnCombatEnd);
        }

        private void OnDisable()
        {
            EventManager.RemoveListener<CombatStartedEvent>(OnCombatStart);
            EventManager.RemoveListener<CombatEndedEvent>(OnCombatEnd);
        }

        private void OnCombatStart(CombatStartedEvent @event)
        {
            InputControlManager.Instance.ChangeControls(ControlTypes.Combat);

            foreach (var eventEnemy in @event.Enemies) eventEnemy.GetComponent<CharacterStateManager>().Enable();
        }

        private void OnCombatEnd(CombatEndedEvent @event)
        {
            InputControlManager.Instance.ChangeControls(ControlTypes.Overworld);
        }
    }
}