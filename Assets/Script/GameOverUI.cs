using TMPro;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private ScoreController scoreController;
    [SerializeField] private GameObject scoreTxt;
    [SerializeField] private ScoreUI highScoreNum;
    private void OnEnable()
    {
        int originalHighScore = PlayerPrefs.GetInt("HighScore", 0);

        scoreController.UpdateHighScore();

        if (scoreController.score > originalHighScore)
        {
            scoreTxt.GetComponent<TMP_Text>().text = "HIGH SCORE!";
            highScoreNum.UpdateHighScoreScoreUI();
        }
        else
        {
            scoreTxt.GetComponent<TMP_Text>().text = "SCORE";
            highScoreNum.UpdateScore(scoreController);
        }
    }
}
