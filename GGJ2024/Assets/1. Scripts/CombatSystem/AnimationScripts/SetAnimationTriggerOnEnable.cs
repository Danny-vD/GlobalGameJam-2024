using System;
using System.Collections;
using UnityEngine;
using VDFramework;

namespace CombatSystem.AnimationScripts
{
	public class SetAnimationBooleanOnEnable : BetterMonoBehaviour
	{
		[SerializeField]
		private Animator animator;
		
		[SerializeField]
		private string parameterName;

		[SerializeField]
		private bool value;
		
		private int parameterID;

		private void Reset()
		{
			animator = GetComponent<Animator>();
		}

		private void Awake()
		{
			parameterID = Animator.StringToHash(parameterName);
		}

		private IEnumerable Update()
		{
			yield return new WaitForSeconds(.5f);
		}

		private void OnEnable()
		{
			animator.SetBool(parameterID, value);
		}
	}
}