using System;
using TMPro;
using UnityEngine;
using Utilities;

namespace UI
{
    public class ScorePrefab : MonoBehaviour
    {
        public TextMeshProUGUI rankTMP;
        public TextMeshProUGUI nameTMP;
        public TextMeshProUGUI levelTMP;
        public TextMeshProUGUI scoreTMP;

        #region Validation

#if UNITY_EDITOR

        private void OnValidate()
        {
            HelperUtilities.ValidateCheckNullValues(this, nameof(rankTMP), rankTMP);
            HelperUtilities.ValidateCheckNullValues(this, nameof(nameTMP), nameTMP);
            HelperUtilities.ValidateCheckNullValues(this, nameof(levelTMP), levelTMP);
            HelperUtilities.ValidateCheckNullValues(this, nameof(scoreTMP), scoreTMP);
        }

#endif

        #endregion
    }
}