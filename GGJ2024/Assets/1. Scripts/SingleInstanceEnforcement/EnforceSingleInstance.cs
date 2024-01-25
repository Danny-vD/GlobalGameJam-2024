using UnityEngine;
using VDFramework;

namespace SingleInstanceEnforcement
{
	public class EnforceSingleInstance : BetterMonoBehaviour
	{
		[SerializeField]
		private GameObject singleInstance;
		
		public static bool IsInitialized { get; private set; }

		private void Awake()
		{
			if (IsInitialized)
			{
				Destroy(singleInstance.gameObject);
			}
			else
			{
				IsInitialized = true;
				
				singleInstance.SetActive(true);
				DontDestroyOnLoad(singleInstance);
			}
			 
			Destroy(gameObject);
		}
	}
}