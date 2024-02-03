using CombatMoves.ScriptableObjects.BaseClasses;
using CombatSystem.CharacterScripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VDFramework;

namespace CombatSystem.UIScripts.CombatMoves
{
	public class CombatMoveUIManager : BetterMonoBehaviour
	{
		[SerializeField]
		private Button selectMoveButton;

		[SerializeField]
		private TMP_Text nameLabel;

		[SerializeField]
		private TMP_Text descriptionLabel;

		[SerializeField]
		private TMP_Text costLabel;

		private AbstractCombatMove combatMove;
		private SelectedMoveHolder selectedMoveHolder;

		public void Initialize(AbstractCombatMove move, GameObject character)
		{
			combatMove = move;

			selectedMoveHolder = character.GetComponent<SelectedMoveHolder>();

			//TODO: logic in between with events for Successful selection and failed selection
			selectMoveButton.onClick.AddListener(SelectMove);

			nameLabel.text = combatMove.AbilityName;

			//descriptionLabel.text = combatMove.Description;
			//costLabel.text        = combatMove.Cost.ToString();
		}

		private void SelectMove()
		{
			//TODO: validate target
			//TODO: select target (support multiple targets as well)
			selectedMoveHolder.SelectMove(combatMove, null);
		}
	}
}