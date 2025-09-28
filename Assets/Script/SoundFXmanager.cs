using UnityEngine;

public class SoundFXmanager : MonoBehaviour
{
    public static SoundFXmanager Instance;

    [SerializeField] private AudioSource soundFXSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySound(AudioClip clip, Transform spawnTransform, float volume)
    {
        AudioSource audioSource = Instantiate(soundFXSource, spawnTransform.position, Quaternion.identity);

        audioSource.clip = clip;

        audioSource.volume = volume;

        audioSource.Play();

        Destroy(audioSource.gameObject, clip.length);
    }
}
