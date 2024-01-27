using CombatSystem.Events;
using UnityEngine;
using VDFramework;

namespace CombatSystem.EventHandlers
{
	public class EnableObjectsOnCombatStart : BetterMonoBehaviour
	{
		[SerializeField]
		private GameObject[] objectsToEnable;
		
		private void OnEnable()
		{
			CombatStartedEvent.ParameterlessListeners += EnableObjects;
			CombatEndedEvent.ParameterlessListeners   += DisableObjects;
		}

		private void OnDisable()
		{
			CombatStartedEvent.ParameterlessListeners -= EnableObjects;
			CombatEndedEvent.ParameterlessListeners -= DisableObjects;
		}

		private void EnableObjects()
		{
			foreach (GameObject obj in objectsToEnable)
			{
				obj.SetActive(true);
			}
		}

		private void DisableObjects()
		{
			foreach (GameObject obj in objectsToEnable)
			{
				obj.SetActive(false);
			}
		}
	}
}