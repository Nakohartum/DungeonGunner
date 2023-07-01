using System;
using System.Collections;
using Dungeon;
using Health;
using Misc;
using Sounds;
using StaticEvents;
using UnityEngine;
using Utilities;
using Random = UnityEngine.Random;

namespace Enemies
{
    [DisallowMultipleComponent]
    public class EnemySpawner : SingletonMonobehaviour<EnemySpawner>
    {
        private int enemiesToSpawn;
        private int currentEnemyCount;
        private int enemiesSpawnedSoFar;
        private int enemyMaxConcurrentSpawnNumber;
        private Room currentRoom;
        private RoomEnemySpawnParameters roomEnemySpawnParameters;

        private void OnEnable()
        {
            StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;
        }

        private void OnDisable()
        {
            StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;
        }

        private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs)
        {
            enemiesSpawnedSoFar = 0;
            currentEnemyCount = 0;
            currentRoom = roomChangedEventArgs.room;
            
            MusicManager.Instance.PlayMusic(currentRoom.ambientMusic, 0.2f, 2f);
            
            if (currentRoom.roomNodeType.isCorridorEW || currentRoom.roomNodeType.isCorridorNS || currentRoom.roomNodeType.isEntrance)
            {
                return;
            }

            if (currentRoom.isClearedOfEnemies)
            {
                return;
            }

            enemiesToSpawn =
                currentRoom.GetNumberOfEnemiesToSpawn(GameManager.GameManager.Instance.GetCurrentDungeonLevel());
            roomEnemySpawnParameters =
                currentRoom.GetRoomEnemySpawnParameters(GameManager.GameManager.Instance.GetCurrentDungeonLevel());
            if (enemiesToSpawn == 0)
            {
                currentRoom.isClearedOfEnemies = true;
                return;
            }

            enemyMaxConcurrentSpawnNumber = GetConcurrentEnemies();
            
            MusicManager.Instance.PlayMusic(currentRoom.battleMusic, 0.2f, 0.5f);
            
            currentRoom.instantiatedRoom.BlockDoors();

            SpawnEnemies();
        }

        private void SpawnEnemies()
        {
            if (GameManager.GameManager.Instance.gameState == GameState.bossStage)
            {
                GameManager.GameManager.Instance.previousGameState = GameState.bossStage;
                GameManager.GameManager.Instance.gameState = GameState.engagingBoss;
            }
            else if (GameManager.GameManager.Instance.gameState == GameState.playingLevel)
            {
                GameManager.GameManager.Instance.previousGameState = GameState.playingLevel;
                GameManager.GameManager.Instance.gameState = GameState.engagingEnemies;
            }

            StartCoroutine(SpawnEnemiesRoutine());
        }

        private IEnumerator SpawnEnemiesRoutine()
        {
            Grid grid = currentRoom.instantiatedRoom.grid;

            RandomSpawnableObject<EnemyDetailsSO> randomEnemyHelperClass =
                new RandomSpawnableObject<EnemyDetailsSO>(currentRoom.enemiesByLevelList);

            if (currentRoom.spawnPositionArray.Length > 0)
            {
                for (int i = 0; i < enemiesToSpawn; i++)
                {
                    while (currentEnemyCount >= enemyMaxConcurrentSpawnNumber)
                    {
                        yield return null;
                    }

                    Vector3Int cellPosition =
                        (Vector3Int) currentRoom.spawnPositionArray[
                            Random.Range(0, currentRoom.spawnPositionArray.Length)];
                    CreateEnemy(randomEnemyHelperClass.GetItem(), grid.CellToWorld(cellPosition));
                    yield return new WaitForSeconds(GetEnemySpawnInterval());
                }
            }
        }
        
        private float GetEnemySpawnInterval()
        {
            return (Random.Range(roomEnemySpawnParameters.minSpawnInterval, roomEnemySpawnParameters.maxSpawnInterval));
        }

        private int GetConcurrentEnemies()
        {
            return (Random.Range(roomEnemySpawnParameters.minConcurrentEnemies,
                roomEnemySpawnParameters.maxConcurrentEnemies));
        }
        
        private void CreateEnemy(EnemyDetailsSO enemyDetails, Vector3 position)
        {
            enemiesSpawnedSoFar++;
            currentEnemyCount++;

            DungeonLevelSO dungeonLevel = GameManager.GameManager.Instance.GetCurrentDungeonLevel();

            GameObject enemy = Instantiate(enemyDetails.enemyPrefab, position, Quaternion.identity, transform);
            
            enemy.GetComponent<Enemy>().EnemyInitialization(enemyDetails, enemiesSpawnedSoFar, dungeonLevel);
            enemy.GetComponent<DestroyedEvent>().OnDestroyed += Enemy_OnDestroyed;
        }

        private void Enemy_OnDestroyed(DestroyedEvent destroyedEvent, DestroyedEventArgs destroyedEventArgs)
        {
            destroyedEvent.OnDestroyed -= Enemy_OnDestroyed;
            currentEnemyCount--;
            StaticEventHandler.CallPointsScoredEvent(destroyedEventArgs.points);
            if (currentEnemyCount <= 0 && enemiesSpawnedSoFar == enemiesToSpawn)
            {
                currentRoom.isClearedOfEnemies = true;
                if (GameManager.GameManager.Instance.gameState == GameState.engagingEnemies)
                {
                    GameManager.GameManager.Instance.gameState = GameState.playingLevel;
                    GameManager.GameManager.Instance.previousGameState = GameState.engagingEnemies;
                }
                else if (GameManager.GameManager.Instance.gameState == GameState.engagingBoss)
                {
                    GameManager.GameManager.Instance.gameState = GameState.bossStage;
                    GameManager.GameManager.Instance.previousGameState = GameState.engagingBoss;
                }
                
                currentRoom.instantiatedRoom.UnlockDoors(Settings.doorUnlockDelay);

                MusicManager.Instance.PlayMusic(currentRoom.ambientMusic, 0.2f, 2f);
                
                StaticEventHandler.CallRoomEnemiesDefeatedEvent(currentRoom);
            }
        }
    }
}