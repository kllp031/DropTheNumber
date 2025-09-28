using UnityEngine;
using TMPro;
public class ScoreUI : MonoBehaviour
{
    [SerializeField] private string scoreTextBegin;
    private TMP_Text scoreText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        scoreText = GetComponent<TMP_Text>();
        if (scoreText == null)
        {
            Debug.LogError("ScoreUI: TMP_Text component not found on the GameObject.");
            return;
        }
    }

    public void UpdateScore(ScoreController scoreController)
    {
        if (scoreText == null)
        {
            scoreText = GetComponent<TMP_Text>();
        }
        scoreText.text = $"{scoreTextBegin} {scoreController.score}";
    }

    public void UpdateHighScoreScoreUI()
    {
        if (scoreText == null)
        {
            scoreText = GetComponent<TMP_Text>();
        }
        scoreText.text = $"{scoreTextBegin} {PlayerPrefs.GetInt("HighScore", 0)}";
    }

}
