﻿using System;
using System.Collections;
using System.Collections.Generic;
using GameManager;
using Player;
using TMPro;
using UnityEngine;
using Utilities;

namespace UI
{
    [DisallowMultipleComponent]
    public class CharacterSelectorUI : MonoBehaviour
    {
        #region Tooltip

        [Tooltip("Populate this with the child CharacterSelector gameobject")]

        #endregion

        [SerializeField]
        private Transform characterSelector;

        #region Tooltip

        [Tooltip("Populate with the TextMeshPro component on the PlayerNameInput gameobject")]

        #endregion

        [SerializeField]
        private TMP_InputField playerNameInput;

        private List<PlayerDetailsSO> playerDetailsList;
        private GameObject playerSelectionPrefab;
        private CurrentPlayerSO currentPlayer;
        private List<GameObject> playerCharacterGameObjectList = new List<GameObject>();
        private Coroutine coroutine;
        private int selectedPlayerIndex = 0;
        private float offset = 4f;

        private void Awake()
        {
            playerSelectionPrefab = GameResources.Instance.playerSelectionPrefab;
            playerDetailsList = GameResources.Instance.playerDetailsList;
            currentPlayer = GameResources.Instance.currentPlayer;
        }

        private void Start()
        {
            for (int i = 0; i < playerDetailsList.Count; i++)
            {
                GameObject playerSelectionObject = Instantiate(playerSelectionPrefab, characterSelector);
                playerCharacterGameObjectList.Add(playerSelectionObject);
                playerSelectionObject.transform.localPosition = new Vector3((offset * i), 0f, 0f);
                PopulatePlayerDetails(playerSelectionObject.GetComponent<PlayerSelectionUI>(), playerDetailsList[i]);
            }

            playerNameInput.text = currentPlayer.playerName;
            currentPlayer.playerDetails = playerDetailsList[selectedPlayerIndex];
        }

        private void PopulatePlayerDetails(PlayerSelectionUI playerSelection, PlayerDetailsSO playerDetails)
        {
            playerSelection.playerHandSpriteRenderer.sprite = playerDetails.playerHandSprite;
            playerSelection.playerHandNoWeaponSpriteRenderer.sprite = playerDetails.playerHandSprite;
            playerSelection.playerWeaponSpriteRenderer.sprite = playerDetails.startingWeapon.weaponSprite;
            playerSelection.animator.runtimeAnimatorController = playerDetails.runtimeAnimatorController;
        }

        public void NextCharacter()
        {
            if (selectedPlayerIndex >= playerDetailsList.Count - 1)
            {
                return;
            }

            selectedPlayerIndex++;
            currentPlayer.playerDetails = playerDetailsList[selectedPlayerIndex];
            MoveToSelectedCharacter(selectedPlayerIndex);
        }
        
        public void PreviousCharacter()
        {
            if (selectedPlayerIndex == 0)
            {
                return;
            }

            selectedPlayerIndex--;
            currentPlayer.playerDetails = playerDetailsList[selectedPlayerIndex];
            MoveToSelectedCharacter(selectedPlayerIndex);
        }

        private void MoveToSelectedCharacter(int index)
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }

            coroutine = StartCoroutine(MoveToSelectedCharacterRoutine(index));
        }

        private IEnumerator MoveToSelectedCharacterRoutine(int index)
        {
            float currentLocalXPosition = characterSelector.localPosition.x;
            float targetLocalXPosition = index * offset * characterSelector.localScale.x * -1f;
            while (Mathf.Abs(currentLocalXPosition - targetLocalXPosition) > 0.01f)
            {
                currentLocalXPosition = Mathf.Lerp(currentLocalXPosition, targetLocalXPosition, Time.deltaTime * 10f);
                characterSelector.localPosition =
                    new Vector3(currentLocalXPosition, characterSelector.localPosition.y, 0f);
                yield return null;
            }

            characterSelector.localPosition = new Vector3(targetLocalXPosition, characterSelector.localPosition.y, 0f);
        }

        public void UpdatePlayerName()
        {
            playerNameInput.text = playerNameInput.text.ToUpper();
            currentPlayer.playerName = playerNameInput.text;
        }

        #region Validation

#if UNITY_EDITOR

        private void OnValidate()
        {
            HelperUtilities.ValidateCheckNullValues(this, nameof(characterSelector), characterSelector);
            HelperUtilities.ValidateCheckNullValues(this, nameof(playerNameInput), playerNameInput);
        }

#endif

        #endregion
    }
}