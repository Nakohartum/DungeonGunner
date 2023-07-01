﻿using System;
using System.Collections.Generic;
using NodeGraph;
using Player;
using Sounds;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Tilemaps;
using Utilities;

namespace GameManager
{
    public class GameResources : MonoBehaviour
    {
        private static GameResources instance;

        public static GameResources Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Resources.Load<GameResources>("GameResources");
                }

                return instance;
            }
        }

        #region Header DUNGEON

        [Space(10)]
        [Header("DUNGEON")]

        #endregion

        #region Tooltip

        [Tooltip("Populate with the dungeon RoomNodeTypeListSO")]

        #endregion
        
        public RoomNodeTypeListSO roomNodeTypeList;

        #region Player Selection

        [Space(10)]
        [Header("PLAYER SELECTION")]

        #endregion

        #region Tooltip

        [Tooltip("The PlayerSelection prefab")]

        #endregion

        public GameObject playerSelectionPrefab;

        #region Header PLAYER

        [Space(10)]
        [Header("PLAYER")]

        #endregion

        #region Tooltip

        [Tooltip("Player details list - populate the list with the playerdetails scriptable objects")]

        #endregion

        public List<PlayerDetailsSO> playerDetailsList;

        #region Tooltip

        [Tooltip("The current player scriptable object - this is used to reference the current player between scenes")]

        #endregion

        public CurrentPlayerSO currentPlayer;

        #region Header MUSIC

        [Space(10)]
        [Header("MUSIC")]

        #endregion

        #region Tooltip

        [Tooltip("Populate with the music master mixer group")]

        #endregion

        public AudioMixerGroup musicMasterMixerGroup;

        #region Tooltip

        [Tooltip("Main menu music scriptable object")]

        #endregion

        public MusicTrackSO mainMenuMusic;

        #region Tooltip

        [Tooltip("Music on full snapshot")]

        #endregion

        public AudioMixerSnapshot musicOnFullSnapshot;
        
        #region Tooltip

        [Tooltip("Music low snapshot")]

        #endregion

        public AudioMixerSnapshot musicLowSnapshot;
        
        #region Tooltip

        [Tooltip("Music off snapshot")]

        #endregion

        public AudioMixerSnapshot musicOffSnapshot;

        #region Header SOUNDS

        [Space(10)]
        [Header("SOUNDS")]

        #endregion

        #region Tooltip

        [Tooltip("Populate with the sounds master mixer group")]

        #endregion

        public AudioMixerGroup soundsMasterMixerGroup;
        
        #region Tooltip

        [Tooltip("Door open close sound effect")]

        #endregion

        public SoundEffectSO doorOpenCloseSoundEffect;

        #region Tootltip

        [Tooltip("Populate with the table flip sound effect")]

        #endregion

        public SoundEffectSO tableFlip;

        #region Tooltip

        [Tooltip("Populate with chest open sound effect")]

        #endregion

        public SoundEffectSO chestOpen;

        #region Tooltip

        [Tooltip("Populate with the health pickup sound effect")]

        #endregion
        
        public SoundEffectSO healthPickup;
        
        #region Tooltip

        [Tooltip("Populate with the weapon pickup sound effect")]

        #endregion
        
        public SoundEffectSO weaponPickup;
        
        #region Tooltip

        [Tooltip("Populate with the ammo pickup sound effect")]

        #endregion
        
        public SoundEffectSO ammoPickup;

        #region Header MATERIALS

        [Space(10)]
        [Header("MATERIALS")]

        #endregion

        #region Tooltip

        [Tooltip("Dimmed material")]

        #endregion

        public Material dimmedMaterial;

        #region Tooltip

        [Tooltip("Sprite-Lit-Default Material")]

        #endregion
        
        public Material litMaterial;

        #region Tooltip

        [Tooltip("Populate with the Variable Lit Shader")]

        #endregion

        public Shader variableLitShader;

        #region Tooltip

        [Tooltip("Populate with the Materialize Shader")]

        #endregion

        public Shader materializeShader;

        #region Header SPECIAL TILEMAP TILES

        [Space(10)]
        [Header("SPECIAL TILEMAP TILES")]

        #endregion

        #region Tooltip

        [Tooltip("Collision tiles that the enemies can navigate to")]

        #endregion

        public TileBase[] enemyUnwalkableCollisionTilesArray;
        
        #region Tooltip

        [Tooltip("Preferred path tile for enemy navigation")]

        #endregion

        public TileBase preferredEnemyPathTile;
        
        #region Header UI

        [Space(10)]
        [Header("UI")]

        #endregion

        #region Tooltip

        [Tooltip("Populate with heart image prefab")]

        #endregion

        public GameObject heartPrefab;

        #region Tooltip

        [Tooltip("Populate with ammo icon prefab")]

        #endregion

        public GameObject ammoIconPrefab;

        #region Tooltip

        [Tooltip("The score prefab")]

        #endregion

        public GameObject scorePrefab;

        #region Header CHESTS

        [Space(10)]
        [Header("CHESTS")]

        #endregion

        #region Tooltip

        [Tooltip("Chest item prefab")]

        #endregion

        public GameObject chestItemPrefab;

        #region Tooltip

        [Tooltip("Populate with heart icon sprite")]

        #endregion

        public Sprite heartIcon;
        
        #region Tooltip

        [Tooltip("Populate with bullet icon sprite")]

        #endregion

        public Sprite bulletIcon;

        #region Header MINIMAP

        [Space(10)]
        [Header("MINIMAP")]

        #endregion

        #region Tooltip

        [Tooltip("Minimap skull prefab")]

        #endregion

        public GameObject minimapSkullPrefab;

        #region Validation

#if UNITY_EDITOR

        private void OnValidate()
        {
            HelperUtilities.ValidateCheckNullValues(this, nameof(roomNodeTypeList), roomNodeTypeList);
            HelperUtilities.ValidateCheckNullValues(this, nameof(playerSelectionPrefab), playerSelectionPrefab);
            HelperUtilities.ValidateCheckEnumerableValues(this, nameof(playerDetailsList), playerDetailsList);
            HelperUtilities.ValidateCheckNullValues(this, nameof(currentPlayer), currentPlayer);
            HelperUtilities.ValidateCheckNullValues(this, nameof(soundsMasterMixerGroup), soundsMasterMixerGroup);
            HelperUtilities.ValidateCheckNullValues(this, nameof(mainMenuMusic), mainMenuMusic);
            HelperUtilities.ValidateCheckNullValues(this, nameof(doorOpenCloseSoundEffect), doorOpenCloseSoundEffect);
            HelperUtilities.ValidateCheckNullValues(this, nameof(tableFlip), tableFlip);
            HelperUtilities.ValidateCheckNullValues(this, nameof(chestOpen), chestOpen);
            HelperUtilities.ValidateCheckNullValues(this, nameof(healthPickup), healthPickup);
            HelperUtilities.ValidateCheckNullValues(this, nameof(ammoPickup), ammoPickup);
            HelperUtilities.ValidateCheckNullValues(this, nameof(weaponPickup), weaponPickup);
            HelperUtilities.ValidateCheckNullValues(this, nameof(litMaterial), litMaterial);
            HelperUtilities.ValidateCheckNullValues(this, nameof(dimmedMaterial), dimmedMaterial);
            HelperUtilities.ValidateCheckNullValues(this, nameof(variableLitShader), variableLitShader);
            HelperUtilities.ValidateCheckNullValues(this, nameof(materializeShader), materializeShader);
            HelperUtilities.ValidateCheckEnumerableValues(this, nameof(enemyUnwalkableCollisionTilesArray), enemyUnwalkableCollisionTilesArray);
            HelperUtilities.ValidateCheckNullValues(this, nameof(preferredEnemyPathTile), preferredEnemyPathTile);
            HelperUtilities.ValidateCheckNullValues(this, nameof(musicMasterMixerGroup), musicMasterMixerGroup);
            HelperUtilities.ValidateCheckNullValues(this, nameof(musicOnFullSnapshot), musicOnFullSnapshot);
            HelperUtilities.ValidateCheckNullValues(this, nameof(musicLowSnapshot), musicLowSnapshot);
            HelperUtilities.ValidateCheckNullValues(this, nameof(musicOffSnapshot), musicOffSnapshot);
            HelperUtilities.ValidateCheckNullValues(this, nameof(ammoIconPrefab), ammoIconPrefab);
            HelperUtilities.ValidateCheckNullValues(this, nameof(scorePrefab), scorePrefab);
            HelperUtilities.ValidateCheckNullValues(this, nameof(heartPrefab), heartPrefab); 
            HelperUtilities.ValidateCheckNullValues(this, nameof(chestItemPrefab), chestItemPrefab); 
            HelperUtilities.ValidateCheckNullValues(this, nameof(heartIcon), heartIcon); 
            HelperUtilities.ValidateCheckNullValues(this, nameof(bulletIcon), bulletIcon); 
            HelperUtilities.ValidateCheckNullValues(this, nameof(minimapSkullPrefab), minimapSkullPrefab);
        }

#endif

        #endregion

    }
}