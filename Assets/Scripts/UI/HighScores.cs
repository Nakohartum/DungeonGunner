using System;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    [Serializable]
    public class HighScores 
    {
        public List<Score> scoreList = new List<Score>();
    }
}