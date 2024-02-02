using CombatSystem.CharacterScripts;
using CombatSystem.Events;
using UI;
using UnityEngine;
using VDFramework.EventSystem;
using VDFramework.UnityExtensions;

namespace CombatSystem.Managers
{
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

			foreach (GameObject eventEnemy in @event.Enemies)
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
}