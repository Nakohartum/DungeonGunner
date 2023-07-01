using UnityEngine;

namespace Health
{
    public class HealthBar : MonoBehaviour
    {
        #region Header GAMEOBJECT REFERENCES

        [Space(10)]
        [Header("GAMEOBJECT REFERENCES")]

        #endregion

        #region Tooltip

        [Tooltip("Populate with the child Bar gameobject")]

        #endregion

        [SerializeField]
        private GameObject healthBar;

        public void EnableHealthBar()
        {
            gameObject.SetActive(true);
        }

        public void DisableHealthBar()
        {
            gameObject.SetActive(false);
        }

        public void SetHealthBarValue(float healthPercent)
        {
            healthBar.transform.localScale = new Vector3(healthPercent, 1f, 1f);
        }
    }
}