using CombatSystem.Events;
using VDFramework;

namespace CombatSystem.Managers
{
	public class CombatManager : BetterMonoBehaviour
	{
		// Is a player currently picking a move?
		// Keep track of all the moves and apply them one by one
		

		private void Awake()
		{
			CombatStartedEvent.ParameterlessListeners += StartCombat;
			CombatEndedEvent.ParameterlessListeners   += EndCombat;
		}

		public void StartCombat()
		{
		}

		public void EndCombat()
		{
		}

		private void OnDestroy()
		{
			CombatStartedEvent.ParameterlessListeners -= StartCombat;
			CombatEndedEvent.ParameterlessListeners   -= EndCombat;
		}
	}
}