using System;
using UnityEngine;
using Utilities;

namespace Environment
{
    [DisallowMultipleComponent]
    public class Environment : MonoBehaviour
    {
        #region Header REFERENCES

        [Space(10)]
        [Header("REFERENCES")]

        #endregion

        #region Tooltip

        [Tooltip("Populate with the SpriteRenderer component on the prefab")]

        #endregion

        public SpriteRenderer spriteRenderer;

        #region Validation

#if UNITY_EDITOR

        private void OnValidate()
        {
            HelperUtilities.ValidateCheckNullValues(this, nameof(spriteRenderer), spriteRenderer);
        }

#endif

        #endregion
    }
}