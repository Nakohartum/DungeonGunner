﻿using System;
using UnityEngine;
using Utilities;

namespace Weapons
{
    [RequireComponent(typeof(AimWeaponEvent))]
    [DisallowMultipleComponent]
    public class AimWeapon : MonoBehaviour
    {
        #region Tooltip

        [Tooltip("Populate with the Transform from the child WeaponRotationPoint gameobject")]

        #endregion

        [SerializeField] private Transform weaponRotationPointTransform;

        private AimWeaponEvent aimWeaponEvent;

        private void Awake()
        {
            aimWeaponEvent = GetComponent<AimWeaponEvent>();
        }

        private void OnEnable()
        {
            aimWeaponEvent.OnWeaponAim += AimWeaponEvent_OnWeaponAim;
        }

        private void OnDisable()
        {
            aimWeaponEvent.OnWeaponAim -= AimWeaponEvent_OnWeaponAim;
        }

        private void AimWeaponEvent_OnWeaponAim(AimWeaponEvent aimWeaponEvent, AimWeaponEventArgs aimWeaponEventArgs)
        {
            Aim(aimWeaponEventArgs.aimDirection, aimWeaponEventArgs.aimAngle);
        }

        private void Aim(AimDirection aimDirection, float aimAngle)
        {
            weaponRotationPointTransform.eulerAngles = new Vector3(0f, 0f, aimAngle);
            switch (aimDirection)
            {
                case AimDirection.Left:
                case AimDirection.UpLeft:
                    weaponRotationPointTransform.localScale = new Vector3(1f, -1f, 0f);
                    break;
                case AimDirection.Up:
                case AimDirection.UpRight:
                case AimDirection.Right:
                case AimDirection.Down:
                    weaponRotationPointTransform.localScale = new Vector3(1f, 1f, 0f);
                    break;
            }
        }

        #region Validation

#if UNITY_EDITOR

        private void OnValidate()
        {
            HelperUtilities.ValidateCheckNullValues(this, nameof(weaponRotationPointTransform),
                weaponRotationPointTransform);
        }     
        
#endif

        #endregion
    }
}