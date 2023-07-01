using System;
using Dungeon;

namespace Enemies
{
    [Serializable]
    public struct EnemyHealthDetails
    {
        public DungeonLevelSO dungeonLevel;
        public int enemyHealthAmount;
    }
}