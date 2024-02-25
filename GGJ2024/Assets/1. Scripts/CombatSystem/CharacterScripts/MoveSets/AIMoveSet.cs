using System.Collections.Generic;
using System.Linq;
using CombatMoves.ScriptableObjects.BaseClasses;
using CombatSystem.Interfaces;
using CombatSystem.ScriptableObjects;
using UnityEngine;
using VDFramework.LootTables;
using VDFramework.LootTables.Structs;

namespace CombatSystem.CharacterScripts.MoveSets
{
    public class AIMoveSet : AbstractMoveset, IAIMoveset
    {
        [SerializeField] private AIMovesetLootTableObject movesWeightedLootTable;

        [SerializeField] [Tooltip("If no weight is provided when adding a new move, this weight will be used instead")]
        private long defaultWeightForNewMove = 5;

        private List<AbstractCombatMove> cachedMovesList;

        public AbstractCombatMove ChooseAIMove()
        {
            return movesWeightedLootTable.GetLoot();
        }

        public override List<AbstractCombatMove> GetMoves()
        {
            if (cachedMovesList != null) return cachedMovesList;

            var lootList = movesWeightedLootTable.GetLootTable().GetLootList();
            cachedMovesList = new List<AbstractCombatMove>(lootList.Count);

            CacheLootList(lootList);

            return cachedMovesList;
        }

        public void AddMove(AbstractCombatMove abstractCombatMove, long weight)
        {
            if (movesWeightedLootTable.TryAdd(abstractCombatMove, weight)) cachedMovesList = null; // Reset the cache
        }

        public override void AddMove(AbstractCombatMove abstractCombatMove)
        {
            AddMove(abstractCombatMove, defaultWeightForNewMove);
        }

        public override void RemoveMove(AbstractCombatMove abstractCombatMove)
        {
            if (movesWeightedLootTable.TryRemove(abstractCombatMove)) cachedMovesList = null; // Reset the cache
        }

        private void CacheLootList(IEnumerable<LootTablePair<AbstractCombatMove>> lootList)
        {
            foreach (var loot in lootList.Select(pair => pair.Loot))
            {
                if (loot is WeightedLootTable<AbstractCombatMove> nestedTable)
                {
                    CacheLootList(nestedTable.GetLootList());
                    continue;
                }

                cachedMovesList.Add(loot.GetLoot());
            }
        }
    }
}