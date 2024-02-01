using CombatMoves.BaseClasses;
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

		public void Initialize(AbstractCombatMove move, SelectedMoveHolder moveHolder)
		{
			combatMove         = move;
			selectedMoveHolder = moveHolder;

			selectMoveButton.onClick.AddListener(SelectMove);

			nameLabel.text = combatMove.AbilityName;

			//descriptionLabel.text = combatMove.Description;
			//costLabel.text        = combatMove.Cost.ToString();
		}

		private void SelectMove()
		{
			selectedMoveHolder.SelectMove(combatMove);
		}
	}
}