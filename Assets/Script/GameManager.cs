using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{

    [SerializeField] private float _TimeToWaitBeforeExit;

    public void OnGameOver()
    {
        Invoke(nameof(EndGame), _TimeToWaitBeforeExit);
    }

    private void EndGame()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
