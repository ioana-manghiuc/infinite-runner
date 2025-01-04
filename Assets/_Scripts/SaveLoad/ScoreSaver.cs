using TMPro;
using UnityEngine;

namespace Assets._Scripts.SaveLoad
{
    public class ScoreSaver : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _ScoreText;
        public void Save()
        {
            int lastScore = int.Parse(_ScoreText.text);
            PlayerPrefs.SetInt(SaveLoadConstants.LAST_SCORE_KEY, lastScore);
            int highScore = PlayerPrefs.GetInt(SaveLoadConstants.HIGH_SCORE_KEY, 0);

            if (lastScore > highScore)
            {
                PlayerPrefs.SetInt(SaveLoadConstants.PREVIOUS_HIGH_SCORE_KEY, highScore);
                PlayerPrefs.SetInt(SaveLoadConstants.HIGH_SCORE_KEY, lastScore);
            }
        }
    }
}