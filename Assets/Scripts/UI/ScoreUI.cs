using System;
using StaticEvents;
using TMPro;
using UnityEngine;

namespace UI
{
    public class ScoreUI : MonoBehaviour
    {
        private TextMeshProUGUI scoreTextTMP;

        private void Awake()
        {
            scoreTextTMP = GetComponentInChildren<TextMeshProUGUI>();
        }

        private void OnEnable()
        {
            StaticEventHandler.OnScoreChanged += StaticEventHandler_OnScoreChanged;
        }

        private void OnDisable()
        {
            StaticEventHandler.OnScoreChanged -= StaticEventHandler_OnScoreChanged;
        }

        private void StaticEventHandler_OnScoreChanged(ScoreChangedArgs scoreChangedArgs)
        {
            scoreTextTMP.text = $"SCORE: {scoreChangedArgs.score:###,###0}\nMultiplier: x{scoreChangedArgs.multiplier}";
        }
    }
}