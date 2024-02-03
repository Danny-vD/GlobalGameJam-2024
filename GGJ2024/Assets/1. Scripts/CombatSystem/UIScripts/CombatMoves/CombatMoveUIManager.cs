using CharacterScripts;
using CombatMoves.ScriptableObjects.BaseClasses;
using CombatSystem.CharacterScripts;
using CombatSystem.Managers;
using CombatSystem.Managers.TargettingSystem;
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
		private ConfirmedMoveHolder confirmedMoveHolder;
		private GameObject characterCaster;

		public void Initialize(AbstractCombatMove move, GameObject character)
		{
			combatMove      = move;
			characterCaster = character;

			confirmedMoveHolder = character.GetComponent<ConfirmedMoveHolder>();

			//TODO: logic in between with events for Successful selection and failed selection
			if (character.GetComponent<Character>().CurrentMP >= move.Cost)
			{
				selectMoveButton.onClick.AddListener(SelectMove);
			}
			

			nameLabel.text = combatMove.AbilityName;

			//descriptionLabel.text = combatMove.Description;
			//costLabel.text        = combatMove.Cost.ToString();
		}

		private void SelectMove()
		{
			CombatTargettingManager.Instance.ChooseMove(combatMove, characterCaster);
		}

		private void a()
		{
			//TODO: validate target
			//TODO: select target (support multiple targets as well)
			confirmedMoveHolder.SelectMove(combatMove, null);
		}
	}
}