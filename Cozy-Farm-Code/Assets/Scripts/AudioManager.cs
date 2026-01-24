using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Clips")]
    [SerializeField] private AudioClip bgMusic;
    [SerializeField] private AudioClip clickSfx;
    [SerializeField] private AudioClip eggCrackSfx;
    [SerializeField] private AudioClip eggLaySfx;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (musicSource != null && bgMusic != null)
        {
            musicSource.clip = bgMusic;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    public void PlayClick()
    {
        PlaySfx(clickSfx);
    }

    public void PlayEggCrack()
    {
        PlaySfx(eggCrackSfx);
    }

    public void PlayEggLay()
    {
        PlaySfx(eggLaySfx);
    }

    private void PlaySfx(AudioClip clip)
    {
        if (sfxSource == null || clip == null) return;
        sfxSource.PlayOneShot(clip);
    }
}
