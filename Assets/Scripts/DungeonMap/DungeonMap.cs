using System;
using System.Collections;
using Cinemachine;
using Dungeon;
using Misc;
using StaticEvents;
using UnityEngine;
using Utilities;

namespace DungeonMap
{
    public class DungeonMap : SingletonMonobehaviour<DungeonMap>
    {
        #region Header GAMEOBJECT REFERENCES

        [Space(10)]
        [Header("GAMEOBJECT REFERENCES")]

        #endregion

        #region Tooltip

        [Tooltip("Populate with the MinimapUI gameobject")]

        #endregion

        [SerializeField]
        private GameObject minimapUI;

        private Camera dungeonMapCamera;
        private Camera cameraMain;

        private void Start()
        {
            cameraMain = Camera.main;
            Transform playerTransform = GameManager.GameManager.Instance.GetPlayer().transform;

            CinemachineVirtualCamera cinemachineVirtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
            cinemachineVirtualCamera.Follow = playerTransform;

            dungeonMapCamera = GetComponentInChildren<Camera>();
            
            dungeonMapCamera.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0) && GameManager.GameManager.Instance.gameState == GameState.dungeonOverviewMap)
            {
                GetRoomClicked();
            }
        }

        private void GetRoomClicked()
        {
            Vector3 worldPosition = dungeonMapCamera.ScreenToWorldPoint(Input.mousePosition);
            worldPosition = new Vector3(worldPosition.x, worldPosition.y, 0f);

            Collider2D[] collider2DArray = Physics2D.OverlapCircleAll(new Vector2(worldPosition.x, worldPosition.y), 1f);

            foreach (var collider2D in collider2DArray)
            {
                if (collider2D.GetComponent<InstantiatedRoom>() != null)
                {
                    InstantiatedRoom instantiatedRoom = collider2D.GetComponent<InstantiatedRoom>();
                    if (instantiatedRoom.room.isClearedOfEnemies && instantiatedRoom.room.isPreviouslyVisited)
                    {
                        StartCoroutine(MovePlayerToRoom(worldPosition, instantiatedRoom.room));
                    }
                }
            }
        }

        private IEnumerator MovePlayerToRoom(Vector3 worldPosition, Room room)
        {
            StaticEventHandler.CallRoomChangedEvent(room);

            yield return StartCoroutine(GameManager.GameManager.Instance.Fade(0f, 1f, 0f, Color.black));
            
            ClearDungeonOverviewMap();
            
            GameManager.GameManager.Instance.GetPlayer().playerControl.DisablePlayer();
            Vector3 spawnPoint = HelperUtilities.GetSpawnPositionNearestToPlayer(worldPosition);
            GameManager.GameManager.Instance.GetPlayer().transform.position = spawnPoint;

            yield return StartCoroutine(GameManager.GameManager.Instance.Fade(1f, 0f, 1f, Color.black));
            
            GameManager.GameManager.Instance.GetPlayer().playerControl.EnablePlayer();
        }

        public void DisplayDungeonOverviewMap()
        {
            GameManager.GameManager.Instance.previousGameState = GameManager.GameManager.Instance.gameState;
            GameManager.GameManager.Instance.gameState = GameState.dungeonOverviewMap;
            
            GameManager.GameManager.Instance.GetPlayer().playerControl.DisablePlayer();
            
            cameraMain.gameObject.SetActive(false);
            dungeonMapCamera.gameObject.SetActive(true);

            ActivateRoomsForDisplay();

            minimapUI.SetActive(false);
        }
        
        public void ClearDungeonOverviewMap()
        {
            GameManager.GameManager.Instance.gameState = GameManager.GameManager.Instance.previousGameState;
            GameManager.GameManager.Instance.previousGameState = GameState.dungeonOverviewMap;
            
            GameManager.GameManager.Instance.GetPlayer().playerControl.EnablePlayer();
            cameraMain.gameObject.SetActive(true);
            dungeonMapCamera.gameObject.SetActive(false);
            
            minimapUI.SetActive(true);
        }
        
        private void ActivateRoomsForDisplay()
        {
            foreach (var keyValuePair in DungeonBuilder.Instance.dungeonBuilderRoomDictionary)
            {
                Room room = keyValuePair.Value;
                room.instantiatedRoom.gameObject.SetActive(true);
            }   
        }
    }
}