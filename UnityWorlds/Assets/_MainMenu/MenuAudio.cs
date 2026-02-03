using UnityEngine;

public class MenuAudio : MonoBehaviour
{
    public static MenuAudio Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Clips")]
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip hoverSfx;
    [SerializeField] private AudioClip clickSfx;

    [Header("Volumes")]
    [Range(0f, 1f)][SerializeField] private float musicVolume = 0.35f;
    [Range(0f, 1f)][SerializeField] private float sfxVolume = 0.8f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        //DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        PlayMenuMusic();
    }

    public void PlayMenuMusic()
    {
        if (musicSource == null || menuMusic == null) return;

        musicSource.loop = true;
        musicSource.clip = menuMusic;
        musicSource.volume = musicVolume;

        if (!musicSource.isPlaying)
            musicSource.Play();
    }

    public void PlayHover()
    {
        if (sfxSource == null || hoverSfx == null) return;
        sfxSource.PlayOneShot(hoverSfx, sfxVolume);
    }

    public void PlayClick()
    {
        if (sfxSource == null || clickSfx == null) return;
        sfxSource.PlayOneShot(clickSfx, sfxVolume);
    }
}
