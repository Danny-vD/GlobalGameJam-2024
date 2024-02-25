using CharacterScripts;
using CombatMoves.ScriptableObjects.BaseClasses;
using CombatSystem.Managers.TargettingSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VDFramework;

namespace CombatSystem.UIScripts.CombatMoves
{
    public class CombatMoveUIManager : BetterMonoBehaviour
    {
        [SerializeField] private Button selectMoveButton;

        [SerializeField] private TMP_Text nameLabel;

        [SerializeField] private TMP_Text descriptionLabel;

        [SerializeField] private TMP_Text costLabel;

        private GameObject characterCaster;

        private AbstractCombatMove combatMove;

        public void Initialize(AbstractCombatMove move, GameObject character)
        {
            combatMove = move;
            characterCaster = character;

            if (character.GetComponent<CharacterMP>().MP >= move.Cost) selectMoveButton.onClick.AddListener(SelectMove);

            nameLabel.text = combatMove.AbilityName;

            //descriptionLabel.text = combatMove.Description;
            //costLabel.text        = combatMove.Cost.ToString();
        }

        private void SelectMove()
        {
            // TODO make a targeting UI that hides the moves buttons, or change the way targetting works so another move can be selected without confirming
            // at the moment clicking on another move after you selected one will both select that move and confirm the target at once, which causes the next confirm input to confim the move from the previous turn
            CombatTargettingManager.Instance.ChooseMove(combatMove, characterCaster);

            CombatMoveUISpawner.HideMoves();
        }
    }
}