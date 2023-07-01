using System;
using Misc;
using UnityEngine;
using Utilities;
using Weapons;
using Random = UnityEngine.Random;

namespace Enemies
{
    [RequireComponent(typeof(Enemy))]
    [DisallowMultipleComponent]
    public class EnemyWeaponAI : MonoBehaviour
    {
        #region Tooltip

        [Tooltip("Select the layers that the enemy bullets will hit")]

        #endregion

        [SerializeField]
        private LayerMask layerMask;
        
        #region Tooltip

        [Tooltip("Populate this with the WeaponShootPosition child gameobject transform")]

        #endregion

        [SerializeField]
        private Transform weaponShootPosition;

        private Enemy enemy;
        private EnemyDetailsSO enemyDetails;
        private float firingIntervalTimer;
        private float firingDurationTimer;

        private void Awake()
        {
            enemy = GetComponent<Enemy>();
        }

        private void Start()
        {
            enemyDetails = enemy.enemyDetails;

            firingIntervalTimer = WeaponShootInterval();
            firingDurationTimer = WeaponShootDuration();
        }

        private float WeaponShootDuration()
        {
            return Random.Range(enemyDetails.firingDurationMin, enemyDetails.firingDurationMax);
        }

        private float WeaponShootInterval()
        {
            return Random.Range(enemyDetails.firingIntervalMin, enemyDetails.firingIntervalMax);
        }

        private void Update()
        {
            firingIntervalTimer -= Time.deltaTime;
            if (firingIntervalTimer < 0f)
            {
                if (firingDurationTimer >= 0f)
                {
                    firingDurationTimer -= Time.deltaTime;
                    FireWeapon();
                }
                else
                {
                    firingIntervalTimer = WeaponShootInterval();
                    firingDurationTimer = WeaponShootDuration();
                }
            }
        }

        private void FireWeapon()
        {
            Vector3 playerDirectionVector =
                GameManager.GameManager.Instance.GetPlayer().GetPlayerPosition() - transform.position;
            Vector3 weaponDirection = (GameManager.GameManager.Instance.GetPlayer().GetPlayerPosition() -
                                       weaponShootPosition.position);
            float weaponAngleDegrees = HelperUtilities.GetAngleFromVector(weaponDirection);
            float enemyAngleDegrees = HelperUtilities.GetAngleFromVector(playerDirectionVector);

            AimDirection enemyAimDirection = HelperUtilities.GetAimDirection(enemyAngleDegrees);

            enemy.aimWeaponEvent.CallAimWeaponEvent(enemyAimDirection, enemyAngleDegrees, weaponAngleDegrees,
                weaponDirection);

            if (enemyDetails.enemyWeapon != null)
            {
                float enemyAmmoRange = enemyDetails.enemyWeapon.weaponCurrentAmmo.ammoRange;
                if (playerDirectionVector.magnitude <= enemyAmmoRange)
                {
                    if (enemyDetails.firingLineOfSightRequired && !IsPlayerInLineOfSight(weaponDirection, enemyAmmoRange))
                    {
                        return;
                    }

                    enemy.fireWeaponEvent.CallOnFireWeaponEvent(true, true, enemyAimDirection, enemyAngleDegrees,
                        weaponAngleDegrees, weaponDirection);
                }
            }
        }

        private bool IsPlayerInLineOfSight(Vector3 weaponDirection, float enemyAmmoRange)
        {
            RaycastHit2D raycastHit2D = Physics2D.Raycast(weaponShootPosition.position, (Vector2) weaponDirection,
                enemyAmmoRange, layerMask);
            if (raycastHit2D && raycastHit2D.transform.CompareTag(Settings.playerTag))
            {
                return true;
            }

            return false;
        }

        #region Validation

#if UNITY_EDITOR

        private void OnValidate()
        {
            HelperUtilities.ValidateCheckNullValues(this, nameof(weaponShootPosition), weaponShootPosition);
        }

#endif

        #endregion
    }
}