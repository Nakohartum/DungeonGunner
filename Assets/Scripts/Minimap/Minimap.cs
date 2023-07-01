using System;
using Cinemachine;
using UnityEngine;
using Utilities;

namespace Minimap
{
    [DisallowMultipleComponent]
    public class Minimap : MonoBehaviour
    {
        #region Tooltip

        [Tooltip("Populate with the child MinimapPlayer gameobject")]

        #endregion

        [SerializeField] private GameObject miniMapPlayer;

        private Transform playerTransform;

        private void Start()
        {
            playerTransform = GameManager.GameManager.Instance.GetPlayer().transform;

            CinemachineVirtualCamera cinemachineVirtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
            cinemachineVirtualCamera.Follow = playerTransform;

            SpriteRenderer spriteRenderer = miniMapPlayer.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = GameManager.GameManager.Instance.GetPlayerMiniMapIcon();
            }
        }

        private void Update()
        {
            if (playerTransform != null && miniMapPlayer != null)
            {
                miniMapPlayer.transform.position = playerTransform.position;
            }
        }

        #region Validation

#if UNITY_EDITOR

        private void OnValidate()
        {
            HelperUtilities.ValidateCheckNullValues(this, nameof(miniMapPlayer), miniMapPlayer);
        }

#endif

        #endregion
    }
}