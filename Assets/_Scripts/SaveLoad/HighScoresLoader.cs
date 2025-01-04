using TMPro;
using UnityEngine;

namespace Assets._Scripts.SaveLoad
{
    public class HighScoresLoader : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _PrevHScoreText;
        [SerializeField]
        private TextMeshProUGUI _HighScoreText;
        public void LoadHighScores()
        {
            int highScore = PlayerPrefs.GetInt(SaveLoadConstants.HIGH_SCORE_KEY, 0);
            _HighScoreText.text = highScore.ToString();
            int lastScore = PlayerPrefs.GetInt(SaveLoadConstants.PREVIOUS_HIGH_SCORE_KEY, 0);
            _PrevHScoreText.text = lastScore.ToString();
        }
    }
}
