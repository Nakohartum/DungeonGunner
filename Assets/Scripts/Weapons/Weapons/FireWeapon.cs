using System;
using System.Collections;
using Sounds;
using UnityEngine;
using Weapons.Ammo;
using Random = UnityEngine.Random;

namespace Weapons
{
    [RequireComponent(typeof(ActiveWeapon))]
    [RequireComponent(typeof(FireWeaponEvent))]
    [RequireComponent(typeof(ReloadWeaponEvent))]
    [RequireComponent(typeof(WeaponFiredEvent))]
    [DisallowMultipleComponent]
    public class FireWeapon : MonoBehaviour
    {
        private float firePrechargeTimer = 0f;
        private float fireRateCoolDownTimer = 0f;
        private ActiveWeapon activeWeapon;
        private FireWeaponEvent fireWeaponEvent;
        private ReloadWeaponEvent reloadWeaponEvent;
        private WeaponFiredEvent weaponFiredEvent;

        private void Awake()
        {
            activeWeapon = GetComponent<ActiveWeapon>();
            fireWeaponEvent = GetComponent<FireWeaponEvent>();
            reloadWeaponEvent = GetComponent<ReloadWeaponEvent>();
            weaponFiredEvent = GetComponent<WeaponFiredEvent>();
        }

        private void OnEnable()
        {
            fireWeaponEvent.OnFireWeapon += FireWeaponEvent_OnFireWeapon;
        }

        private void OnDisable()
        {
            fireWeaponEvent.OnFireWeapon -= FireWeaponEvent_OnFireWeapon;
        }

        private void FireWeaponEvent_OnFireWeapon(FireWeaponEvent fireWeaponEvent, FireWeaponEventArgs fireWeaponEventArgs)
        {
            WeaponFire(fireWeaponEventArgs);
        }

        private void Update()
        {
            fireRateCoolDownTimer -= Time.deltaTime;
        }

        private void WeaponFire(FireWeaponEventArgs fireWeaponEventArgs)
        {
            WeaponPreCharge(fireWeaponEventArgs);
            if (fireWeaponEventArgs.fire)
            {
                if (IsWeaponReadyToFire())
                {
                    FireAmmo(fireWeaponEventArgs.aimAngle, fireWeaponEventArgs.weaponAimAngle,
                        fireWeaponEventArgs.weaponAimDirectionVector);

                    ResetCooldownTimer();

                    ResetPrechargeTimer();
                }
            }
        }

        private void WeaponPreCharge(FireWeaponEventArgs fireWeaponEventArgs)
        {
            if (fireWeaponEventArgs.firePreviousFrame)
            {
                firePrechargeTimer -= Time.deltaTime;
            }
            else
            {
                ResetPrechargeTimer();
            }
        }

        private bool IsWeaponReadyToFire()
        {
            if (activeWeapon.GetCurrentWeapon().weaponRemainingAmmo <= 0 && !activeWeapon.GetCurrentWeapon().weaponDetails.hasInfiniteAmmo)
            {
                return false;
            }

            if (activeWeapon.GetCurrentWeapon().isWeaponReloading)
            {
                return false;
            }

            if (firePrechargeTimer > 0f || fireRateCoolDownTimer > 0f)
            {
                return false;
            }

            if (!activeWeapon.GetCurrentWeapon().weaponDetails.hasInfiniteClipCapacity && activeWeapon.GetCurrentWeapon().weaponClipRemainigAmmo <= 0)
            {
                reloadWeaponEvent.CallReloadWeaponEvent(activeWeapon.GetCurrentWeapon(), 0);
                return false;
            }

            return true;
        }
        
        private void FireAmmo(float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector)
        {
            AmmoDetailsSO currentAmmo = activeWeapon.GetCurrentAmmo();
            if (currentAmmo != null)
            {
                StartCoroutine(FireAmmoRoutine(currentAmmo, aimAngle, weaponAimAngle, weaponAimDirectionVector));
            }
        }

        private IEnumerator FireAmmoRoutine(AmmoDetailsSO currentAmmo, float aimAngle, float weaponAimAngle,
            Vector3 weaponAimDirectionVector)
        {
            int ammoCounter = 0;

            int ammoPerShot = Random.Range(currentAmmo.ammoSpawnAmountMin, currentAmmo.ammoSpawnAmountMax + 1);

            float ammoSpawnInterval;

            if (ammoPerShot > 1)
            {
                ammoSpawnInterval = Random.Range(currentAmmo.ammoSpawnIntervalMin, currentAmmo.ammoSpawnIntervalMax);
            }
            else
            {
                ammoSpawnInterval = 0f;
            }

            while (ammoCounter < ammoPerShot)
            {
                ammoCounter++;
                
                GameObject ammoPrefab =
                    currentAmmo.ammoPrefabArray[Random.Range(0, currentAmmo.ammoPrefabArray.Length)];
                float ammoSpeed = Random.Range(currentAmmo.ammoSpeedMin, currentAmmo.ammoSpeedMax);
                IFireable ammo = (IFireable) PoolManager.PoolManager.Instance.ReuseComponent(ammoPrefab,
                    activeWeapon.GetShootPosition(), Quaternion.identity);
                ammo.InitializeAmmo(currentAmmo, aimAngle, weaponAimAngle, ammoSpeed, weaponAimDirectionVector);
                
                yield return new WaitForSeconds(ammoSpawnInterval);
            }
            
            
            if (!activeWeapon.GetCurrentWeapon().weaponDetails.hasInfiniteClipCapacity)
            {
                activeWeapon.GetCurrentWeapon().weaponClipRemainigAmmo--;
                activeWeapon.GetCurrentWeapon().weaponRemainingAmmo--;
            }
            weaponFiredEvent.CallWeaponFiredEvent(activeWeapon.GetCurrentWeapon());
            WeaponShootEffect(aimAngle);
            WeaponSoundEffect();
        }

        private void ResetCooldownTimer()
        {
            fireRateCoolDownTimer = activeWeapon.GetCurrentWeapon().weaponDetails.weaponFireRate;
        }

        private void ResetPrechargeTimer()
        {
            firePrechargeTimer = activeWeapon.GetCurrentWeapon().weaponDetails.weaponPrechargeTime;
        }

        private void WeaponShootEffect(float aimAngle)
        {
            if (activeWeapon.GetCurrentWeapon().weaponDetails.weaponShootEffect != null &&
                activeWeapon.GetCurrentWeapon().weaponDetails.weaponShootEffect.weaponShootEffectPrefab != null)
            {
                WeaponShootEffect weaponShootEffect =
                    (WeaponShootEffect) PoolManager.PoolManager.Instance.ReuseComponent(
                        activeWeapon.GetCurrentWeapon().weaponDetails.weaponShootEffect.weaponShootEffectPrefab,
                        activeWeapon.GetShootEffectPosition(), Quaternion.identity);
                weaponShootEffect.SetShootEffect(activeWeapon.GetCurrentWeapon().weaponDetails.weaponShootEffect,
                    aimAngle);
                weaponShootEffect.gameObject.SetActive(true);
            }
        }
        
        private void WeaponSoundEffect()
        {
            if (activeWeapon.GetCurrentWeapon().weaponDetails.weaponFiringSoundEffect != null)
            {
                SoundEffectManager.Instance.PlaySoundEffect(activeWeapon.GetCurrentWeapon().weaponDetails
                    .weaponFiringSoundEffect);
            }
        }
    }
}