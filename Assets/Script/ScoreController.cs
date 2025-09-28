using UnityEngine;
using UnityEngine.Events;

public class ScoreController : MonoBehaviour
{
    public int score { get; private set; }

    public UnityEvent OnScoreChanged;

    [SerializeField] private AudioClip beatHighScore;
    [SerializeField] private AudioClip lowScoreSound;

    public void Awake()
    {
        AddScore(0); // Initialize score to 0
        //PlayerPrefs.SetInt("HighScore", 0);
    }
    public void AddScore(int points)
    {
        score += points;

        OnScoreChanged?.Invoke();
    }

    public void UpdateHighScore()
    {
        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        if (score > highScore)
        {
            PlayerPrefs.SetInt("HighScore", score);
            PlayerPrefs.Save();
            SoundFXmanager.Instance.PlaySound(beatHighScore, transform, 1f);
        }
        else
        {
            SoundFXmanager.Instance.PlaySound(lowScoreSound, transform, 1f);
        }
    }
}
