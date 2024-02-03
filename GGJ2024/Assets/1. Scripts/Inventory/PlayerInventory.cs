using System;
using System.Collections.Generic;
using System.Linq;
using Ink.Runtime;
using VDFramework;

namespace Inventory
{
	public class PlayerInventory : BetterMonoBehaviour
	{
		private Dictionary<IItem, int> itemsList;

		private void Awake()
		{
			itemsList = new Dictionary<IItem, int>();
		}

		public void AddItem(IItem newItem)
		{
			itemsList.TryAdd(newItem, 1);
		}

		public void RemoveItem(IItem toBeRemoved)
		{
			if (itemsList.ContainsKey(toBeRemoved))
			{
				itemsList.Remove(toBeRemoved);
			}
		}

		public void PurgeItemList()
		{
			// Will crash?
			foreach (KeyValuePair<IItem, int> keyValuePair in itemsList.Where(keyValuePair => keyValuePair.Value == 0))
			{
				itemsList.Remove(keyValuePair.Key);
			}
		}

		public Dictionary<IItem, int> GetList()
		{
			return itemsList;
		}
	}
}