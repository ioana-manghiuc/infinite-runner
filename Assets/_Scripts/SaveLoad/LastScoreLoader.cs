using TMPro;
using UnityEngine;
namespace Assets._Scripts.SaveLoad
{
    public class LastScoreLoader : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _LastScoreText;
        public void LoadLastScore()
        {
            int lastScore = PlayerPrefs.GetInt(SaveLoadConstants.LAST_SCORE_KEY, 0);
            _LastScoreText.text = lastScore.ToString();
        }
    }
}
