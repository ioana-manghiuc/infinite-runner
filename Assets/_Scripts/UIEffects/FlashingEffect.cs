using TMPro;
using UnityEngine;

public class FlashingText : MonoBehaviour
{
    public TextMeshProUGUI GameOverText;

    [SerializeField]
    public Color firstColor;
    [SerializeField]
    public Color secondColor;
    [SerializeField]
    public float speed;

    void Start()
    {   }

    void Update()
    {
        GameOverText.faceColor = Color.Lerp(firstColor, secondColor, Mathf.Sin(Time.time * speed));
    }
}
