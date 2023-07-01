using System;
using Misc;
using UnityEngine;
using Utilities;
using Random = UnityEngine.Random;

namespace Weapons.Ammo
{
    public class AmmoPattern : MonoBehaviour, IFireable
    {
        #region Tooltip

        [Tooltip("Populate the array with the child ammo gameobjects")]

        #endregion

        [SerializeField]
        private Ammo[] ammoArray;

        private float ammoRange;
        private float ammoSpeed;
        private Vector3 fireDirectionVector;
        private float fireDirectionAngle;
        private AmmoDetailsSO ammoDetails;
        private float ammoChargeTimer;

        public void InitializeAmmo(AmmoDetailsSO ammoDetails, float aimAngle, float weaponAimAngle, float ammoSpeed,
            Vector3 weaponAimDirectionVector, bool overrideAmmoMovement = false)
        {
            this.ammoDetails = ammoDetails;
            this.ammoSpeed = ammoSpeed;

            SetFireDirection(ammoDetails, aimAngle, weaponAimAngle, weaponAimDirectionVector);
            this.ammoRange = ammoDetails.ammoRange;

            gameObject.SetActive(true);

            foreach (var ammo in ammoArray)
            {
                ammo.InitializeAmmo(ammoDetails, aimAngle, weaponAimAngle, ammoSpeed, weaponAimDirectionVector, true);
            }

            if (ammoDetails.ammoChargeTime > 0f)
            {
                ammoChargeTimer = ammoDetails.ammoChargeTime;
            }
            else
            {
                ammoChargeTimer = 0f;
            }
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }
        
        private void Update()
        {
            if (ammoChargeTimer > 0f)
            {
                ammoChargeTimer -= Time.deltaTime;
                return;
            }

            Vector3 distanceVector = fireDirectionVector * ammoSpeed * Time.deltaTime;

            transform.position += distanceVector;
            transform.Rotate(new Vector3(0f,0f, ammoDetails.ammoRotationSpeed * Time.deltaTime));

            ammoRange -= distanceVector.magnitude;

            if (ammoRange < 0f)
            {
                DisableAmmo();
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
            fireDirectionVector = HelperUtilities.GetDirectionVectorFromAngle(fireDirectionAngle);
        }
        
        private void DisableAmmo()
        {
            gameObject.SetActive(false);
        }

        #region Validation

#if UNITY_EDITOR

        private void OnValidate()
        {
            HelperUtilities.ValidateCheckEnumerableValues(this, nameof(ammoArray), ammoArray);
        }

#endif

        #endregion
    }
}