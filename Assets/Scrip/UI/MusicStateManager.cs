using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [Header("Audio")]
    public AudioSource audioSource;

    [Header("Music Clips")]
    public AudioClip menuMusic;
    public AudioClip gameOverMusic;
    public AudioClip winMusic;
    public AudioClip gameplayMusic;

    private void Awake()
    {
        // Singleton (solo uno en todo el juego)
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // NO se destruye al cambiar escena
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayMenuMusic()
    {
        PlayMusic(menuMusic);
    }

    public void PlayGameOverMusic()
    {
        PlayMusic(gameOverMusic);
    }

    public void PlayWinMusic()
    {
        PlayMusic(winMusic);
    }

    public void PlayGameplayMusic()
    {
        PlayMusic(gameplayMusic);
    }

    private void PlayMusic(AudioClip clip)
    {
        if (audioSource.clip == clip) return;

        audioSource.clip = clip;
        audioSource.loop = true;
        audioSource.Play();
    }
}