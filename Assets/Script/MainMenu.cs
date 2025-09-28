using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    [SerializeField] private AudioClip buttonClickSound;

    public void Play()
    {
        // Play button click sound
        SoundFXmanager.Instance.PlaySound(buttonClickSound, transform, 0.5f);
        SceneManager.LoadScene("DropTheNumber");
    }

    public void Exit()
    {
        // Play button click sound
        SoundFXmanager.Instance.PlaySound(buttonClickSound, transform, 0.5f);
        Application.Quit();
    }
}
