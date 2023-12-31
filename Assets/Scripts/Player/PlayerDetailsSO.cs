﻿using System;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
using Weapons;

namespace Player
{
    [CreateAssetMenu(fileName = "PlayerDetails_", menuName = "Scriptable Objects/Player/Player Details")]
    public class PlayerDetailsSO : ScriptableObject
    {
        #region Header PLAYER BASE DETAILS

        [Space(10)]
        [Header("PLAYER BASE DETAILS")]

        #endregion

        #region Tooltip

        [Tooltip("Player character name")]

        #endregion

        public string playerCharacterName;

        #region Tooltip

        [Tooltip("Prefab gameobject for the player")]

        #endregion

        public GameObject playerPrefab;

        #region Tooltip

        [Tooltip("Player runtime animator controller")]

        #endregion

        public RuntimeAnimatorController runtimeAnimatorController;

        #region Header HEALTH

        [Space(10)]
        [Header("HEALTH")]

        #endregion

        #region Tooltip

        [Tooltip("Player starting health amount")]

        #endregion

        public int playerHealthAmount;

        #region Tooltip

        [Tooltip(
            "Select if has immunity period immediately after being hit. If so specify the immunity in seconds in the other field")]

        #endregion

        public bool isImmuneAfterHit = false;

        #region Tooltip

        [Tooltip("Immunity time in seconds after being hit")]

        #endregion

        public float hitImmunityTime;

        #region Header WEAPON

        [Space(10)]
        [Header("WEAPON")]

        #endregion

        #region Tooltip

        [Tooltip("Player initial starting weapon")]

        #endregion

        public WeaponDetailsSO startingWeapon;

        #region Tooltip

        [Tooltip("Populate with the list of starting weapons")]

        #endregion

        public List<WeaponDetailsSO> startingWeaponList;

        #region Header OTHER

        [Space(10)]
        [Header("OTHER")]

        #endregion

        #region Tooltip

        [Tooltip("Player icon sprite to be used in the minimap")]

        #endregion

        public Sprite playerMiniMapIcon;

        #region Tooltip

        [Tooltip("Player hand sprite")]

        #endregion

        public Sprite playerHandSprite;

        #region Editor

#if UNITY_EDITOR

        private void OnValidate()
        {
            HelperUtilities.ValidateCheckEmptyString(this, nameof(playerCharacterName), playerCharacterName);
            HelperUtilities.ValidateCheckNullValues(this, nameof(playerPrefab), playerPrefab);
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(playerHealthAmount), playerHealthAmount, false);
            HelperUtilities.ValidateCheckNullValues(this, nameof(startingWeapon), startingWeapon);
            HelperUtilities.ValidateCheckNullValues(this, nameof(playerMiniMapIcon), playerMiniMapIcon);
            HelperUtilities.ValidateCheckNullValues(this, nameof(playerHandSprite), playerHandSprite);
            HelperUtilities.ValidateCheckNullValues(this, nameof(runtimeAnimatorController), runtimeAnimatorController);
            HelperUtilities.ValidateCheckEnumerableValues(this, nameof(startingWeaponList), startingWeaponList);

            if (isImmuneAfterHit)
            {
                HelperUtilities.ValidateCheckPositiveValue(this, nameof(hitImmunityTime), hitImmunityTime, false);
            }
        }

#endif

        #endregion
    }
}