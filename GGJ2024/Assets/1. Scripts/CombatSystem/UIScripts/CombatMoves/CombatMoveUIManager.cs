using CombatSystem.CharacterScripts;
using CombatSystem.ScriptableAssets.CombatMoves;
using LocalisationPackage.Core;
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

		private CombatMove combatMove;
		private CombatMoveManager combatMoveManager;

		public void Initialize(CombatMove move, CombatMoveManager moveManager)
		{
			combatMove        = move;
			combatMoveManager = moveManager;

			selectMoveButton.onClick.AddListener(SelectMove);

			nameLabel.text = combatMove.name; //LocalisationUtil.GetLocalisedString(combatMove.Name);

			//descriptionLabel.text = LocalisationUtil.GetLocalisedString(combatMove.Description);
			//costLabel.text        = combatMove.Cost.ToString();
		}

		private void SelectMove()
		{
			combatMoveManager.SelectMove(combatMove);
		}
	}
}