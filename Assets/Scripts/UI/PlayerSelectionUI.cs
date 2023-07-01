using System;
using UnityEngine;
using Utilities;

namespace UI
{
    [DisallowMultipleComponent]
    public class PlayerSelectionUI : MonoBehaviour
    {
        #region Tooltip

        [Tooltip("Populate with the Sprite Renderer on child gameobject WeaponAnchorPosition/WeaponRotationPoint/Hand")]

        #endregion

        public SpriteRenderer playerHandSpriteRenderer;

        #region Tooltip

        [Tooltip("Populate with the Sprite Renderer on child gameobject HandNoWeapon")]

        #endregion

        public SpriteRenderer playerHandNoWeaponSpriteRenderer;
        
        #region Tooltip

        [Tooltip("Populate with the Sprite Renderer on child gameobject WeaponAnchorPosition/WeaponRotationPoint/Weapon")]

        #endregion

        public SpriteRenderer playerWeaponSpriteRenderer;

        #region Tooltip

        [Tooltip("Populate with the Animator component")]

        #endregion

        public Animator animator;

        #region Validation

#if UNITY_EDITOR

        private void OnValidate()
        {
            HelperUtilities.ValidateCheckNullValues(this, nameof(playerHandSpriteRenderer), playerHandSpriteRenderer);
            HelperUtilities.ValidateCheckNullValues(this, nameof(playerHandNoWeaponSpriteRenderer), playerHandNoWeaponSpriteRenderer);
            HelperUtilities.ValidateCheckNullValues(this, nameof(playerWeaponSpriteRenderer), playerWeaponSpriteRenderer);
            HelperUtilities.ValidateCheckNullValues(this, nameof(animator), animator);
        }

#endif

        #endregion
    }
}