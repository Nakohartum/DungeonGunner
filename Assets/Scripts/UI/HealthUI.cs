using System;
using System.Collections.Generic;
using GameManager;
using Health;
using Misc;
using UnityEngine;

namespace UI
{
    [DisallowMultipleComponent]
    public class HealthUI : MonoBehaviour
    {
        private List<GameObject> healthHeartsList = new List<GameObject>();

        private void OnEnable()
        {
            GameManager.GameManager.Instance.GetPlayer().healthEvent.OnHealthChanged += HealthEvent_OnHealthChanged;
        }

        private void OnDisable()
        {
            GameManager.GameManager.Instance.GetPlayer().healthEvent.OnHealthChanged -= HealthEvent_OnHealthChanged;
        }

        private void HealthEvent_OnHealthChanged(HealthEvent healthEvent, HealthEventArgs healthEventArgs)
        {
            SetHealthBar(healthEventArgs);
        }

        private void ClearHealthBar()
        {
            foreach (var heartIcon in healthHeartsList)
            {
                Destroy(heartIcon);
            }
            healthHeartsList.Clear();
        }

        private void SetHealthBar(HealthEventArgs healthEventArgs)
        {
            ClearHealthBar();
            int healthHearts = Mathf.CeilToInt(healthEventArgs.healthPercent * 100f / 20f);

            for (int i = 0; i < healthHearts; i++)
            {
                GameObject heart = Instantiate(GameResources.Instance.heartPrefab, transform);
                heart.GetComponent<RectTransform>().anchoredPosition = new Vector2(Settings.uiHeartSpacing * i, 0f);
                healthHeartsList.Add(heart);
            }
        }
    }
}