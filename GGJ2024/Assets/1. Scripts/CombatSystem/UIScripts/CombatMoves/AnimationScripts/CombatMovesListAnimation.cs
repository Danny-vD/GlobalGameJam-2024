using UnityEngine;
using VDFramework;

namespace CombatSystem.UIScripts.CombatMoves.AnimationScripts
{
	public class CombatMovesListAnimation : BetterMonoBehaviour
	{
		private static readonly int isHidden = Animator.StringToHash("IsHidden");
		
		private Animator animator;

		private void Awake()
		{
			animator = GetComponent<Animator>();

			CombatMoveUISpawner.OnShowMoves += PlayShowAnimation;
			CombatMoveUISpawner.OnHideMoves += PlayHideAnimation;
			
		}

		private void OnDestroy()
		{
			CombatMoveUISpawner.OnShowMoves -= PlayShowAnimation;
			CombatMoveUISpawner.OnHideMoves -= PlayHideAnimation;
		}
		
		private void PlayShowAnimation()
		{
            animator.SetBool(isHidden, false);
		}
		
		private void PlayHideAnimation()
		{
            animator.SetBool(isHidden, true);
		}
	}
}