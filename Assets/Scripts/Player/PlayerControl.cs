﻿using System;
using System.Collections;
using System.Collections.Generic;
using Environment;
using Misc;
using Movement;
using UnityEngine;
using Utilities;
using Weapons;

namespace Player
{
    [RequireComponent(typeof(Player))]
    [DisallowMultipleComponent]
    
    public class PlayerControl : MonoBehaviour
    {
        #region Tooltip

        [Tooltip("MovementDetailsSO scriptable object containing movement details such as speed")]

        #endregion

        [SerializeField]
        private MovementDetailsSO movementDetails;
        
        private Player player;
        private bool leftMouseDownPreviousFrame = false;
        private int currentWeaponIndex = 1;
        private float moveSpeed;
        private Coroutine playerRollCoroutine;
        private WaitForFixedUpdate waitForFixedUpdate;
        [HideInInspector] public bool isPlayerRolling = false;
        private float playerRollCooldownTimer = 0f;
        private bool isPlayerMovementDisabled = false;

        private void Awake()
        {
            player = GetComponent<Player>();
            moveSpeed = movementDetails.GetMoveSpeed();
        }

        private void Start()
        {
            waitForFixedUpdate = new WaitForFixedUpdate();
            SetStartingWeapon();
            SetPlayerAnimationSpeed();
        }

        private void SetStartingWeapon()
        {
            int index = 1;

            foreach (var weapon in player.weaponList)
            {
                if (weapon.weaponDetails == player.playerDetails.startingWeapon)
                {
                    SetWeaponByIndex(index);
                    break;
                }

                index++;
            }
        }

        private void SetPlayerAnimationSpeed()
        {
            player.animator.speed = moveSpeed / Settings.baseSpeedForPlayerAnimations;
        }

        private void Update()
        {
            if (isPlayerMovementDisabled)
            {
                return;
            }
            if (isPlayerRolling)
            {
                return;
            }
            MovementInput();
            WeaponInput();
            UseItemInput();
            PlayerRollCooldownTimer();
        }
        
        private void MovementInput()
        {
            float horizontalMovement = Input.GetAxis("Horizontal");
            float verticalMovement = Input.GetAxis("Vertical");
            bool rightMouseButtonDown = Input.GetMouseButtonDown(1);
            Vector2 direction = new Vector2(horizontalMovement, verticalMovement);
            
            
            if (horizontalMovement != 0f && verticalMovement != 0f)
            {
                direction *= 0.7f;
            }

            if (direction != Vector2.zero)
            {
                if (!rightMouseButtonDown)
                {
                    player.movementByVelocityEvent.CallMovementByVelocityEvent(direction, moveSpeed);
                }
                else if(playerRollCooldownTimer <= 0f)
                {
                    PlayerRoll((Vector3) direction);
                }
            }
            else
            {
                player.idleEvent.CallIdleEvent();
            }
        }

        private void PlayerRoll(Vector3 direction)
        {
            playerRollCoroutine = StartCoroutine(PlayerRollRoutine(direction));
        }

        private IEnumerator PlayerRollRoutine(Vector3 direction)
        {
            float minDistance = 0.2f;
            isPlayerRolling = true;
            Vector3 targetPosition = player.transform.position + (Vector3) direction * movementDetails.rollDistance;
            while (Vector3.Distance(player.transform.position, targetPosition) > minDistance)
            {
                player.movementToPositionEvent.CallMovementToPositionEvent(targetPosition, player.transform.position,
                    movementDetails.rollSpeed, direction, isPlayerRolling);
                yield return waitForFixedUpdate;
            }

            isPlayerRolling = false;
            playerRollCooldownTimer = movementDetails.rollCooldownTime;
            player.transform.position = targetPosition;
        }

        private void PlayerRollCooldownTimer()
        {
            if (playerRollCooldownTimer >= 0f)
            {
                playerRollCooldownTimer -= Time.deltaTime;
            }
        }
        
        private void WeaponInput()
        {
            Vector3 weaponDirection;
            float weaponAngleDegrees, playerAngleDegrees;
            AimDirection playerAimDirection;

            AimWeaponInput(out weaponDirection, out weaponAngleDegrees, out playerAngleDegrees, out playerAimDirection);
            FireWeaponInput(weaponDirection, weaponAngleDegrees, playerAngleDegrees, playerAimDirection);
            SwitchWeaponInput();
            ReloadWeaponInput();
        }
        
        private void AimWeaponInput(out Vector3 weaponDirection, out float weaponAngleDegrees, out float playerAngleDegrees, out AimDirection playerAimDirection)
        {
            Vector3 mouseWorldPosition = HelperUtilities.GetMouseWorldPosition();
            weaponDirection = (mouseWorldPosition - player.activeWeapon.GetShootPosition());
            Vector3 playerDirection = (mouseWorldPosition - transform.position);
            weaponAngleDegrees = HelperUtilities.GetAngleFromVector(weaponDirection);
            playerAngleDegrees = HelperUtilities.GetAngleFromVector(playerDirection);
            playerAimDirection = HelperUtilities.GetAimDirection(playerAngleDegrees);

            player.aimWeaponEvent.CallAimWeaponEvent(playerAimDirection, playerAngleDegrees, weaponAngleDegrees,
                weaponDirection);
        }
        
        private void FireWeaponInput(Vector3 weaponDirection, float weaponAngleDegrees, float playerAngleDegrees, AimDirection playerAimDirection)
        {
            if (Input.GetMouseButton(0))
            {
                player.fireWeaponEvent.CallOnFireWeaponEvent(true, leftMouseDownPreviousFrame, playerAimDirection, playerAngleDegrees,
                    weaponAngleDegrees, weaponDirection);
                leftMouseDownPreviousFrame = true;
            }
            else
            {
                leftMouseDownPreviousFrame = false;
            }
        }

        private void SwitchWeaponInput()
        {
            if (Input.mouseScrollDelta.y < 0f)
            {
                PreviousWeapon();
            }

            if (Input.mouseScrollDelta.y > 0f)
            {
                NextWeapon();
            }

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SetWeaponByIndex(1);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SetWeaponByIndex(2);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                SetWeaponByIndex(3);
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                SetWeaponByIndex(4);
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                SetWeaponByIndex(5);
            }
            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                SetWeaponByIndex(6);
            }
            if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                SetWeaponByIndex(7);
            }
            if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                SetWeaponByIndex(8);
            }
            if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                SetWeaponByIndex(9);
            }
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                SetWeaponByIndex(10);
            }

            if (Input.GetKeyDown(KeyCode.Minus))
            {
                SetCurrentWeaponToFirstInTheList();
            }
        }

        private void SetWeaponByIndex(int index)
        {
            if (index - 1 < player.weaponList.Count)
            {
                currentWeaponIndex = index;
                player.setActiveWeaponEvent.CallSetActiveWeaponEvent(player.weaponList[index - 1]);
            }
        }

        private void NextWeapon()
        {
            currentWeaponIndex++;
            if (currentWeaponIndex > player.weaponList.Count)
            {
                currentWeaponIndex = 1;
            }

            SetWeaponByIndex(currentWeaponIndex);
        }

        private void PreviousWeapon()
        {
            currentWeaponIndex--;
            if (currentWeaponIndex < 1)
            {
                currentWeaponIndex = player.weaponList.Count;
            }

            SetWeaponByIndex(currentWeaponIndex);
        }

        
        private void ReloadWeaponInput()
        {
            Weapon currentWeapon = player.activeWeapon.GetCurrentWeapon();
            if (currentWeapon.isWeaponReloading)
            {
                return;
            }

            if (currentWeapon.weaponRemainingAmmo < currentWeapon.weaponDetails.weaponClipCapacity && !currentWeapon.weaponDetails.hasInfiniteAmmo)
            {
                return;
            }

            if (currentWeapon.weaponRemainingAmmo == currentWeapon.weaponDetails.weaponClipCapacity)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                player.reloadWeaponEvent.CallReloadWeaponEvent(player.activeWeapon.GetCurrentWeapon(), 0);
            }
        }
        
        private void UseItemInput()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                float useItemRadius = 2f;
                Collider2D[] collider2DArray = Physics2D.OverlapCircleAll(player.GetPlayerPosition(), useItemRadius);

                foreach (var collider2D in collider2DArray)
                {
                    IUseable iUseable = collider2D.GetComponent<IUseable>();
                    if (iUseable != null)
                    {
                        iUseable.UseItem();
                    }
                }
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            StopPlayerRollRoutine();
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            StopPlayerRollRoutine();
        }

        private void StopPlayerRollRoutine()
        {
            if (playerRollCoroutine != null)
            {
                StopCoroutine(playerRollCoroutine);
                isPlayerRolling = false;
            }
        }

        public void EnablePlayer()
        {
            isPlayerMovementDisabled = false;
        }

        public void DisablePlayer()
        {
            isPlayerMovementDisabled = true;
            player.idleEvent.CallIdleEvent();
        }
        
        private void SetCurrentWeaponToFirstInTheList()
        {
            List<Weapon> tempWeaponList = new List<Weapon>();
            Weapon currentWeapon = player.weaponList[currentWeaponIndex - 1];
            currentWeapon.weaponListPosition = 1;
            tempWeaponList.Add(currentWeapon);
            int index = 2;
            foreach (var weapon in player.weaponList)
            {
                if (weapon == currentWeapon)
                {
                    continue;
                }
                tempWeaponList.Add(weapon);
                weapon.weaponListPosition = index;
                index++;
            }

            player.weaponList = tempWeaponList;
            currentWeaponIndex = 1;
            SetWeaponByIndex(currentWeaponIndex);
        }

        #region Validation

#if UNITY_EDITOR

        private void OnValidate()
        {
            HelperUtilities.ValidateCheckNullValues(this, nameof(movementDetails), movementDetails);
        }

#endif

        #endregion
    }
}