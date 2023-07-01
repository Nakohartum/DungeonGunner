using System;
using System.Collections.Generic;
using Dungeon;

namespace Utilities
{
    [Serializable]
    public class SpawnableObjectsByLevel<T>
    {
        public DungeonLevelSO dungeonLevel;
        public List<SpawnableObjectRatio<T>> spawnableObjectRatioList;
    }
}