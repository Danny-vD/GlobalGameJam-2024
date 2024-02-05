using CombatSystem.Events.CharacterSelection;
using VDFramework;
using VDFramework.EventSystem;

namespace CharacterScripts
{ 
	public class HoverOverCharacterHandler : BetterMonoBehaviour
	{
		private void OnMouseEnter()
		{
			EventManager.RaiseEvent(new CharacterHoveredEvent(gameObject));
		}
	}
}