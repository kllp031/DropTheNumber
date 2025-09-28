using UnityEngine;
using UnityEngine.SceneManagement;

public class Button : MonoBehaviour
{
    [SerializeField] private GameObject PauseScreen;

    [SerializeField] private AudioClip buttonClickSound;

    public static bool IsPaused { get; private set; }

    // In your Pause_button method:
    public void Pause_buttton()
    {
        bool isActive = !PauseScreen.activeSelf;
        PauseScreen.SetActive(isActive);
        IsPaused = isActive;

        SoundFXmanager.Instance.PlaySound(buttonClickSound, transform, 0.5f);
        Time.timeScale = (Time.timeScale == 1) ? 0 : 1;
    }

    public void Exit_button()
    {
        SoundFXmanager.Instance.PlaySound(buttonClickSound, transform, 0.5f);
        Application.Quit();
        Debug.Log("Exit Game");
    }

    public void Restart_button()
    {
        PauseScreen.SetActive(false);
        IsPaused = false;

        SoundFXmanager.Instance.PlaySound(buttonClickSound, transform, 0.5f);
        Time.timeScale = 1;
        StartCoroutine(DelayedSceneLoad(SceneManager.GetActiveScene().name));
    }

    public void MainMenu_button()
    {
        PauseScreen.SetActive(false);
        IsPaused = false;

        SoundFXmanager.Instance.PlaySound(buttonClickSound, transform, 0.5f);
        Time.timeScale = 1;
        StartCoroutine(DelayedSceneLoad("MainMenu"));
    }

    private System.Collections.IEnumerator DelayedSceneLoad(string sceneName)
    {
        yield return new WaitForSecondsRealtime(0.2f); // Wait for sound to play
        SceneManager.LoadScene(sceneName);
    }
}
