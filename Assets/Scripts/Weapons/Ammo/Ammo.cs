﻿using System;
using Misc;
using StaticEvents;
using UnityEngine;
using Utilities;
using Random = UnityEngine.Random;

namespace Weapons.Ammo
{
    [DisallowMultipleComponent]
    public class Ammo : MonoBehaviour, IFireable
    {
        #region Tooltip

        [Tooltip("Populate with child TrailRenderer component")]

        #endregion

        [SerializeField] private TrailRenderer trailRenderer;

        private float ammoRange = 0f;
        private float ammoSpeed;
        private Vector3 fireDirectionVector;
        private float fireDirectionAngle;
        private SpriteRenderer spriteRenderer;
        private AmmoDetailsSO ammoDetails;
        private float ammoChargeTimer;
        private bool isAmmoMaterialSet = false;
        private bool overrideAmmoMovement;
        private bool isColliding = false;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            if (ammoChargeTimer > 0f)
            {
                ammoChargeTimer -= Time.deltaTime;
                return;
            }
            else if (!isAmmoMaterialSet)
            {
                SetAmmoMaterial(ammoDetails.ammoMaterial);
                isAmmoMaterialSet = true;
            }

            if (!overrideAmmoMovement)
            {
                Vector3 distanceVector = fireDirectionVector * ammoSpeed * Time.deltaTime;
                transform.position += distanceVector;

                ammoRange -= distanceVector.magnitude;

                if (ammoRange < 0)
                {
                    if (ammoDetails.isPlayerAmmo)
                    {
                        StaticEventHandler.CallMultiplierEvent(false);
                    }
                    DisableAmmo();
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D other) 
        {
            if (isColliding)
            {
                return;
            }
            DealDamage(other);
            AmmoHitEffect();
            DisableAmmo();
        }

        private void DealDamage(Collider2D collision)
        {
            Health.Health health = collision.GetComponent<Health.Health>();
            bool enemyHit = false;
            if (health != null)
            {
                isColliding = true;
                health.TakeDamage(ammoDetails.ammoDamage);

                if (health.enemy != null)
                {
                    enemyHit = true;
                }
            }

            if (ammoDetails.isPlayerAmmo)
            {
                if (enemyHit)
                {
                    StaticEventHandler.CallMultiplierEvent(true);
                }
                else
                {
                    StaticEventHandler.CallMultiplierEvent(false);
                }
            }
        }

        public void InitializeAmmo(AmmoDetailsSO ammoDetails, float aimAngle, float weaponAimAngle, float ammoSpeed,
            Vector3 weaponAimDirectionVector, bool overrideAmmoMovement = false)
        {
            this.ammoDetails = ammoDetails;
            isColliding = false;
            SetFireDirection(ammoDetails, aimAngle, weaponAimAngle, weaponAimDirectionVector);
            spriteRenderer.sprite = ammoDetails.ammoSprite;
            if (ammoDetails.ammoChargeTime > 0f)
            {
                ammoChargeTimer = ammoDetails.ammoChargeTime;
                SetAmmoMaterial(ammoDetails.ammoChargeMaterial);
                isAmmoMaterialSet = false;
            }
            else
            {
                ammoChargeTimer = 0f;
                SetAmmoMaterial(ammoDetails.ammoMaterial);
                isAmmoMaterialSet = true;
            }

            this.ammoRange = ammoDetails.ammoRange;
            this.ammoSpeed = ammoSpeed;
            this.overrideAmmoMovement = overrideAmmoMovement;
            gameObject.SetActive(true);


            if (ammoDetails.isAmmoTrail)
            {
                trailRenderer.gameObject.SetActive(true);
                trailRenderer.emitting = true;
                trailRenderer.material = ammoDetails.ammoTrailMaterial;
                trailRenderer.startWidth = ammoDetails.ammoTrailStartWidth;
                trailRenderer.endWidth = ammoDetails.ammoTrailEndWidth;
                trailRenderer.time = ammoDetails.ammoTrailTime;
            }
            else
            {
                trailRenderer.emitting = false;
                trailRenderer.gameObject.SetActive(false);
            }
        }

        

        private void SetFireDirection(AmmoDetailsSO ammoDetails, float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector)
        {
            float randomSpread = Random.Range(ammoDetails.ammoSpreadMin, this.ammoDetails.ammoSpreadMax);
            int spreadToggle = Random.Range(0, 2) * 2 - 1;
            if (weaponAimDirectionVector.magnitude < Settings.useAimAngleDistance)
            {
                fireDirectionAngle = aimAngle;
            }
            else
            {
                fireDirectionAngle = weaponAimAngle;
            }

            fireDirectionAngle += spreadToggle * randomSpread;
            transform.eulerAngles = new Vector3(0f, 0f, fireDirectionAngle);
            fireDirectionVector = HelperUtilities.GetDirectionVectorFromAngle(fireDirectionAngle);
        }
        
        private void DisableAmmo()
        {
            gameObject.SetActive(false);
        }

        private void AmmoHitEffect()
        {
            if (ammoDetails.ammoHitEffect != null && ammoDetails.ammoHitEffect.ammoHitEffectPrefab != null)
            {
                AmmoHitEffect ammoHitEffect =
                    (AmmoHitEffect) PoolManager.PoolManager.Instance.ReuseComponent(
                        ammoDetails.ammoHitEffect.ammoHitEffectPrefab, transform.position, Quaternion.identity);
                ammoHitEffect.SetHitEffect(ammoDetails.ammoHitEffect);
                ammoHitEffect.gameObject.SetActive(true);
            }
        }
        
        private void SetAmmoMaterial(Material material)
        {
            spriteRenderer.material = material;
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }

        #region Validation

#if UNITY_EDITOR
        private void OnValidate()
        {
            HelperUtilities.ValidateCheckNullValues(this, nameof(trailRenderer), trailRenderer);
        }
#endif

        #endregion
    }
}