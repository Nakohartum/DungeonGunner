using System;
using Misc;
using UnityEngine;
using Utilities;

namespace Health
{
    [DisallowMultipleComponent]
    public class DealContactDamage : MonoBehaviour
    {
        #region Header

        [Space(10)]
        [Header("DEAL DAMAGE")]

        #endregion

        #region Tooltip

        [Tooltip("The contact damage to deal (is overriden by receiver)")]

        #endregion

        [SerializeField]
        private int contactDamageAmount;

        #region Tooltip

        [Tooltip("Specify what layers objects should be on to receive contact damage")]

        #endregion

        [SerializeField]
        private LayerMask layerMask;

        private bool isColliding = false;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (isColliding)
            {
                return;
            }

            ContactDamage(other);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (isColliding)
            {
                return;
            }
            ContactDamage(other);
        }

        private void ContactDamage(Collider2D collision)
        {
            int collisionObjectLayerMask = (1 << collision.gameObject.layer);

            if ((layerMask.value & collisionObjectLayerMask) == 0)
            {
                return;
            }

            ReceiveContactDamage receiveContactDamage = collision.gameObject.GetComponent<ReceiveContactDamage>();

            if (receiveContactDamage != null)
            {
                isColliding = true;

                Invoke(nameof(ResetContactCollision), Settings.contactDamageCollisionResetDelay);

                receiveContactDamage.TakeContactDamage(contactDamageAmount);
            }
        }

        private void ResetContactCollision()
        {
            isColliding = false;
        }

        #region Validation

#if UNITY_EDITOR

        private void OnValidate()
        {
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(contactDamageAmount), contactDamageAmount, true);
        }

#endif

        #endregion
    }
}