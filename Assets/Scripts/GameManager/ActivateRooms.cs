﻿using System;
using Dungeon;
using UnityEngine;
using Utilities;

namespace GameManager
{
    [DisallowMultipleComponent]
    public class ActivateRooms : MonoBehaviour
    {
        #region Header

        [Header("POPULATE WITH MINIMAP CAMERA")]

        #endregion

        [SerializeField]
        private Camera miniMapCamera;

        private Camera mainCamera;

        private void Start()
        {
            mainCamera = Camera.main;
            InvokeRepeating(nameof(EnableRooms), 0.5f, 0.75f);
        }

        private void EnableRooms()
        {
            if (GameManager.Instance.gameState == GameState.dungeonOverviewMap)
            {
                return;
            }
            HelperUtilities.CameraWorldPositionBounds(out Vector2Int miniMapCameraWorldPositionLowerBounds,
                out Vector2Int miniMapCameraWorldPositionUpperBounds, miniMapCamera);
            
            HelperUtilities.CameraWorldPositionBounds(out Vector2Int mainCameraWorldPositionLowerBounds,
                out Vector2Int mainCameraWorldPositionUpperBounds, mainCamera);

            foreach (var keyValuePair in DungeonBuilder.Instance.dungeonBuilderRoomDictionary)
            {
                Room room = keyValuePair.Value;

                if ((room.lowerBounds.x <= miniMapCameraWorldPositionUpperBounds.x && room.lowerBounds.y <= miniMapCameraWorldPositionUpperBounds.y)
                    && (room.upperBounds.x >= miniMapCameraWorldPositionLowerBounds.x && room.upperBounds.y >= miniMapCameraWorldPositionLowerBounds.y))
                {
                    room.instantiatedRoom.gameObject.SetActive(true);

                    if ((room.lowerBounds.x <= mainCameraWorldPositionUpperBounds.x && room.lowerBounds.y <= mainCameraWorldPositionUpperBounds.y)
                        &&  (room.upperBounds.x >= mainCameraWorldPositionLowerBounds.x && room.upperBounds.y >= mainCameraWorldPositionLowerBounds.y))
                    {
                        room.instantiatedRoom.ActivateEnvironmentGameObjects();
                    }
                    else
                    {
                        room.instantiatedRoom.DeactivateEnvironmentGameObjects();
                    }
                }
                else
                {
                    room.instantiatedRoom.gameObject.SetActive(false);
                }
            }
        }

        #region Validation

#if UNITY_EDITOR

        private void OnValidate()
        {
            HelperUtilities.ValidateCheckNullValues(this, nameof(miniMapCamera), miniMapCamera);
        }

#endif

        #endregion
    }
}