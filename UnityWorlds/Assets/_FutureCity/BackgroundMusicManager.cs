using UnityEngine;

public class BackgroundMusicManager : MonoBehaviour
{
    [Header("Music")]
    [SerializeField] private AudioSource musicSource;  // assign UI_Music_Source AudioSource here
    [SerializeField] private AudioClip musicClip;      // assign the music clip here
    [Range(0f, 1f)][SerializeField] private float volume = 0.35f;

    void Awake()
    {
        if (musicSource == null)
            musicSource = GetComponent<AudioSource>();

        if (musicSource == null)
        {
            Debug.LogWarning("[BackgroundMusicManager] No AudioSource found.");
            return;
        }

        // Configure
        musicSource.playOnAwake = false;
        musicSource.loop = true;
        musicSource.spatialBlend = 0f; // 2D
        musicSource.volume = volume;

        // Ensure clip
        if (musicClip != null)
            musicSource.clip = musicClip;

        // Play
        if (musicSource.clip != null && !musicSource.isPlaying)
            musicSource.Play();
        else if (musicSource.clip == null)
            Debug.LogWarning("[BackgroundMusicManager] musicClip is not assigned.");
    }

    void OnDisable()
    {
        if (musicSource != null && musicSource.isPlaying)
            musicSource.Stop();
    }
}
