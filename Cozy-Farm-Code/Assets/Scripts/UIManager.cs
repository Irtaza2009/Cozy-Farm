using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Panels")]
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private ChickenManager chickenManager;

    [Header("Settings Buttons")]
    [SerializeField] private UnityEngine.UI.Button sfxButton;
    [SerializeField] private UnityEngine.UI.Button musicButton;
    [SerializeField] private Sprite sfxNormal;
    [SerializeField] private Sprite sfxPressed;
    [SerializeField] private Sprite sfxDisabled;
    [SerializeField] private Sprite sfxDisabledPressed;
    [SerializeField] private Sprite musicNormal;
    [SerializeField] private Sprite musicPressed;
    [SerializeField] private Sprite musicDisabled;
    [SerializeField] private Sprite musicDisabledPressed;

    private bool isShopOpen;
    private bool isSettingsOpen;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        shopPanel.SetActive(false);
        isShopOpen = false;
        RefreshAudioButtons();
    }

    public void ToggleShop()
    {
        isShopOpen = !isShopOpen;
        shopPanel.SetActive(isShopOpen);

        if (isShopOpen && chickenManager != null)
        {
            chickenManager.ForceRefreshUI();
        }
    }

    public void CloseShop()
    {
        isShopOpen = false;
        shopPanel.SetActive(false);
    }

    public void OpenShop()
    {
        isShopOpen = true;
        shopPanel.SetActive(true);
    }

    public void ToggleSettings()
    {
        isSettingsOpen = !isSettingsOpen;
        settingsPanel.SetActive(isSettingsOpen);
    }

    public void CloseSettings()
    {
        isSettingsOpen = false;
        settingsPanel.SetActive(false);
    }

    public void OpenSettings()
    {
        isSettingsOpen = true;
        settingsPanel.SetActive(true);
    }

    public void ToggleSfx()
    {
        if (AudioManager.Instance == null) return;
        bool newState = !AudioManager.Instance.SfxEnabled;
        AudioManager.Instance.SetSfxEnabled(newState);
        RefreshAudioButtons();
    }

    public void ToggleMusic()
    {
        if (AudioManager.Instance == null) return;
        bool newState = !AudioManager.Instance.MusicEnabled;
        AudioManager.Instance.SetMusicEnabled(newState);
        RefreshAudioButtons();
    }

    private void RefreshAudioButtons()
    {
        if (AudioManager.Instance == null) return;

        if (sfxButton != null)
        {
            var spriteState = sfxButton.spriteState;
            if (AudioManager.Instance.SfxEnabled)
            {
                sfxButton.interactable = true;
                sfxButton.image.sprite = sfxNormal;
                spriteState.pressedSprite = sfxPressed;
                spriteState.disabledSprite = sfxDisabled;
            }
            else
            {
                sfxButton.interactable = true; // allow re-enable
                sfxButton.image.sprite = sfxDisabled;
                spriteState.pressedSprite = sfxDisabledPressed;
                spriteState.disabledSprite = sfxDisabled;
            }
            sfxButton.spriteState = spriteState;
        }

        if (musicButton != null)
        {
            var spriteState = musicButton.spriteState;
            if (AudioManager.Instance.MusicEnabled)
            {
                musicButton.interactable = true;
                musicButton.image.sprite = musicNormal;
                spriteState.pressedSprite = musicPressed;
                spriteState.disabledSprite = musicDisabled;
            }
            else
            {
                musicButton.interactable = true; // allow re-enable
                musicButton.image.sprite = musicDisabled;
                spriteState.pressedSprite = musicDisabledPressed;
                spriteState.disabledSprite = musicDisabled;
            }
            musicButton.spriteState = spriteState;
        }
    }

    public void PlayButton()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
    }
}
