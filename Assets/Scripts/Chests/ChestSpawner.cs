using System;
using System.Collections.Generic;
using Dungeon;
using StaticEvents;
using UnityEngine;
using Utilities;
using Weapons;
using Random = UnityEngine.Random;

namespace Chests
{
    public class ChestSpawner : MonoBehaviour
    {
        [Serializable]
        private struct RangeByLevel
        {
            public DungeonLevelSO dungeonLevel;
            [Range(0, 100)] public int min;
            [Range(0, 100)] public int max;
        }

        #region Header CHEST PREFAB

        [Space(10)]
        [Header("CHEST PREFAB")]

        #endregion

        #region Tooltip

        [Tooltip("Populate with the chest prefab")]

        #endregion

        [SerializeField]
        private GameObject chestPrefab;

        #region Header CHEST SPAWN CHANCE

        [Space(10)]
        [Header("CHEST SPAWN CHANCE")]

        #endregion

        #region Tooltip

        [Tooltip("The minimum probability for spawning a chest")]

        #endregion

        [SerializeField]
        [Range(0, 100)]
        private int chestSpawnChanceMin;
        
        #region Tooltip

        [Tooltip("The maximum probability for spawning a chest")]

        #endregion

        [SerializeField]
        [Range(0, 100)]
        private int chestSpawnChanceMax;

        #region Tooltip

        [Tooltip("You can override the chest spawn chance by dungeon level")]

        #endregion

        [SerializeField]
        private List<RangeByLevel> chestSpawnChanceByLevelList;

        #region Header CHEST SPAWN DETAILS

        [Space(10)]
        [Header("CHEST SPAWN DETAILS")]

        #endregion

        [SerializeField]
        private ChestSpawnEvent chestSpawnEvent;

        [SerializeField] private ChestSpawnPosition chestSpawnPosition;

        #region Tooltip

        [Tooltip(
            "The minimum number of items to spawn (note that a maximum of 1 of each type of ammo, health, and weapon will be spawned)")]

        #endregion

        [SerializeField]
        [Range(0, 3)]
        private int numberOfItemsSpawnMin;
        
        #region Tooltip

        [Tooltip(
            "The maximum number of items to spawn (note that a maximum of 1 of each type of ammo, health, and weapon will be spawned)")]

        #endregion

        [SerializeField]
        [Range(0, 3)]
        private int numberOfItemsSpawnMax;

        #region Header CHEST CONTENT DETAILS

        [Space(10)]
        [Header("CHEST CONTENT DETAILS")]

        #endregion

        #region Tooltip

        [Tooltip("The weapons to spawn for each dungeon level and their spawn ratios")]

        #endregion

        [SerializeField]
        private List<SpawnableObjectsByLevel<WeaponDetailsSO>> weaponSpawnByLevelList;

        #region Tooltip

        [Tooltip("The range of health to spawn for each level")]

        #endregion

        [SerializeField]
        private List<RangeByLevel> healthSpawnByLevelList;

        #region Tooltip

        [Tooltip("The range of ammo to spawn for each level")]

        #endregion

        [SerializeField]
        private List<RangeByLevel> ammoSpawnByLevelList;

        private bool chestSpawned = false;
        private Room chestRoom;

        private void OnEnable()
        {
            StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;
            StaticEventHandler.OnRoomEnemiesDefeated += StaticEventHandler_OnRoomEnemiesDefeated;
        }

        private void OnDisable()
        {
            StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;
            StaticEventHandler.OnRoomEnemiesDefeated -= StaticEventHandler_OnRoomEnemiesDefeated;
        }
        
        private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs)
        {
            if (chestRoom == null)
            {
                chestRoom = GetComponentInParent<InstantiatedRoom>().room;
            }

            if (!chestSpawned && chestSpawnEvent == ChestSpawnEvent.onRoomEntry && chestRoom == roomChangedEventArgs.room)
            {
                SpawnChest();
            }
        }
        
        private void StaticEventHandler_OnRoomEnemiesDefeated(RoomEnemiesDefeatedArgs roomEnemiesDefeatedArgs)
        {
            if (chestRoom == null)
            {
                chestRoom = GetComponentInParent<InstantiatedRoom>().room;
            }

            if (!chestSpawned && chestSpawnEvent == ChestSpawnEvent.onEnemiesDefeated && chestRoom == roomEnemiesDefeatedArgs.room)
            {
                SpawnChest();
            }
        }


        private void SpawnChest()
        {
            chestSpawned = true;

            if (!RandomSpawnChest())
            {
                return;
            }

            GetItemsToSpawn(out int ammoNum, out int healthNum, out int weaponNum);

            GameObject chestGameObject = Instantiate(chestPrefab, this.transform);

            if (chestSpawnPosition == ChestSpawnPosition.atSpawnerPosition)
            {
                chestGameObject.transform.position = transform.position;
            }
            else if (chestSpawnPosition == ChestSpawnPosition.atPlayerPosition)
            {
                Vector3 spawnPosition =
                    HelperUtilities.GetSpawnPositionNearestToPlayer(GameManager.GameManager.Instance.GetPlayer()
                        .transform.position);
                Vector3 variation = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);
                chestGameObject.transform.position = spawnPosition + variation;
            }

            Chest chest = chestGameObject.GetComponent<Chest>();
            if (chestSpawnEvent == ChestSpawnEvent.onRoomEntry)
            {
                chest.Initialize(false, GetHealthPercentToSpawn(healthNum), GetWeaponDetailsToSpawn(weaponNum), GetAmmoPercentToSpawn(ammoNum));
            }
            else
            {
                chest.Initialize(true, GetHealthPercentToSpawn(healthNum), GetWeaponDetailsToSpawn(weaponNum), GetAmmoPercentToSpawn(ammoNum));
            }
        }
        
        private bool RandomSpawnChest()
        {
            int chancePercent = Random.Range(chestSpawnChanceMin, chestSpawnChanceMax + 1);

            foreach (var rangeByLevel in chestSpawnChanceByLevelList)
            {
                if (rangeByLevel.dungeonLevel == GameManager.GameManager.Instance.GetCurrentDungeonLevel())
                {
                    chancePercent = Random.Range(rangeByLevel.min, rangeByLevel.max + 1);
                    break;
                }
            }

            int randomPercent = Random.Range(1, 100 + 1);
            if (randomPercent <= chancePercent)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        
        private void GetItemsToSpawn(out int ammo, out int health, out int weapons)
        {
            ammo = 0;
            health = 0;
            weapons = 0;

            int numberOfItemsSpawn = Random.Range(numberOfItemsSpawnMin, numberOfItemsSpawnMax + 1);
            int choice;
            if (numberOfItemsSpawn == 1)
            {
                choice = Random.Range(0, 3);
                if (choice == 0)
                {
                    weapons++;
                    return;
                }

                if (choice == 1)
                {
                    ammo++;
                    return;
                }

                if (choice == 2)
                {
                    health++;
                    return;
                }
                return;
            }
            else if (numberOfItemsSpawn == 2)
            {
                choice = Random.Range(0, 3);
                if (choice == 0)
                {
                    weapons++;
                    ammo++;
                    return;
                }

                if (choice == 1)
                {
                    ammo++;
                    health++;
                    return;
                }

                if (choice == 2)
                {
                    health++;
                    weapons++;
                    return;
                }
                return;
            }
            else if (numberOfItemsSpawn >= 3)
            {
                weapons++;
                ammo++;
                health++;
                return;
            }
        }
        
        private int GetAmmoPercentToSpawn(int ammoNum)
        {
            if (ammoNum == 0)
            {
                return 0;
            }

            foreach (var rangeByLevel in ammoSpawnByLevelList)
            {
                if (rangeByLevel.dungeonLevel == GameManager.GameManager.Instance.GetCurrentDungeonLevel())
                {
                    return Random.Range(rangeByLevel.min, rangeByLevel.max);
                }
            }

            return 0;
        }
        
        private int GetHealthPercentToSpawn(int healthNum)
        {
            if (healthNum == 0)
            {
                return 0;
            }

            foreach (var rangeByLevel in healthSpawnByLevelList)
            {
                if (rangeByLevel.dungeonLevel == GameManager.GameManager.Instance.GetCurrentDungeonLevel())
                {
                    return Random.Range(rangeByLevel.min, rangeByLevel.max);
                }
            }

            return 0;
        }

        private WeaponDetailsSO GetWeaponDetailsToSpawn(int weaponNum)
        {
            if (weaponNum == 0)
            {
                return null;
            }

            RandomSpawnableObject<WeaponDetailsSO> weaponRandom =
                new RandomSpawnableObject<WeaponDetailsSO>(weaponSpawnByLevelList);
            WeaponDetailsSO weaponDetails = weaponRandom.GetItem();
            return weaponDetails;
        }

        #region Validation

#if UNITY_EDITOR

        private void OnValidate()
        {
            HelperUtilities.ValidateCheckNullValues(this, nameof(chestPrefab), chestPrefab);
            HelperUtilities.ValidateCheckPositiveRange(this, nameof(chestSpawnChanceMin), chestSpawnChanceMin,
                nameof(chestSpawnChanceMax), chestSpawnChanceMax, true);
            if (chestSpawnChanceByLevelList != null && chestSpawnChanceByLevelList.Count > 0)
            {
                HelperUtilities.ValidateCheckEnumerableValues(this, nameof(chestSpawnChanceByLevelList),
                    chestSpawnChanceByLevelList);
                foreach (var rangeByLevel in chestSpawnChanceByLevelList)
                {
                    HelperUtilities.ValidateCheckNullValues(this, nameof(rangeByLevel.dungeonLevel),
                        rangeByLevel.dungeonLevel);
                    HelperUtilities.ValidateCheckPositiveRange(this, nameof(rangeByLevel.min), rangeByLevel.min,
                        nameof(rangeByLevel.max), rangeByLevel.max, true);
                }
            }

            HelperUtilities.ValidateCheckPositiveRange(this, nameof(numberOfItemsSpawnMin), numberOfItemsSpawnMin,
                nameof(numberOfItemsSpawnMax), numberOfItemsSpawnMax, true);

            if (weaponSpawnByLevelList != null && weaponSpawnByLevelList.Count > 0)
            {
                foreach (var weaponDetailsByLevel in weaponSpawnByLevelList)
                {
                    HelperUtilities.ValidateCheckNullValues(this, nameof(weaponDetailsByLevel.dungeonLevel),
                        weaponDetailsByLevel.dungeonLevel);
                    foreach (var weaponRatio in weaponDetailsByLevel.spawnableObjectRatioList)
                    {
                        HelperUtilities.ValidateCheckNullValues(this, nameof(weaponRatio.dungeonObject),
                            weaponRatio.dungeonObject);
                        HelperUtilities.ValidateCheckPositiveValue(this, nameof(weaponRatio.ratio), weaponRatio.ratio, true);
                    }
                }
            }

            if (healthSpawnByLevelList != null && healthSpawnByLevelList.Count > 0)
            {
                HelperUtilities.ValidateCheckEnumerableValues(this, nameof(healthSpawnByLevelList),
                    healthSpawnByLevelList);
                foreach (var rangeByLevel in healthSpawnByLevelList)
                {
                    HelperUtilities.ValidateCheckNullValues(this, nameof(rangeByLevel.dungeonLevel),
                        rangeByLevel.dungeonLevel);
                    HelperUtilities.ValidateCheckPositiveRange(this, nameof(rangeByLevel.min), rangeByLevel.min,
                        nameof(rangeByLevel.max), rangeByLevel.max, true);
                }
            }

            if (ammoSpawnByLevelList != null && ammoSpawnByLevelList.Count > 0)
            {
                HelperUtilities.ValidateCheckEnumerableValues(this, nameof(ammoSpawnByLevelList), ammoSpawnByLevelList);
                foreach (var rangeByLevel in ammoSpawnByLevelList)
                {
                    HelperUtilities.ValidateCheckNullValues(this, nameof(rangeByLevel.dungeonLevel),
                        rangeByLevel.dungeonLevel);
                    HelperUtilities.ValidateCheckPositiveRange(this, nameof(rangeByLevel.min), rangeByLevel.min,
                        nameof(rangeByLevel.max), rangeByLevel.max, true);
                }
            }
        }

#endif

        #endregion
    }
}