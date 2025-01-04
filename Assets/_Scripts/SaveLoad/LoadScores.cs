using UnityEngine;

namespace Assets._Scripts.SaveLoad
{
    public class LoadScores: MonoBehaviour
    {
        [SerializeField] private HighScoresLoader _highScoresLoader;
        [SerializeField] private LastScoreLoader _lastScoreLoader;
        [SerializeField] private Canvas _highScoresCanvas;
        [SerializeField] private Canvas _lastScoreCanvas;
        public void Start()
        {
            int lastScore = PlayerPrefs.GetInt(SaveLoadConstants.LAST_SCORE_KEY, 0);
            int highScore = PlayerPrefs.GetInt(SaveLoadConstants.HIGH_SCORE_KEY, 0);

            if (lastScore < highScore)
            {
                _lastScoreLoader.LoadLastScore();
                _highScoresCanvas.gameObject.SetActive(false);
                _lastScoreCanvas.gameObject.SetActive(true);
            }
            else
            {
                _highScoresLoader.LoadHighScores();
                _highScoresCanvas.gameObject.SetActive(true);
                _lastScoreCanvas.gameObject.SetActive(false);
            }
        }
    }
}
