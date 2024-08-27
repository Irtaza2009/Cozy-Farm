using UnityEngine;


public class Switch : MonoBehaviour
{
    public GameObject switchOn;  // Reference to the "on" sprite
    public GameObject switchOff; // Reference to the "off" sprite


    private bool isMuted = false;

    void Start()
    {
        // Ensure the switch is set to the correct state at the start
        UpdateSwitch();
    }

    public void OnSwitchToggle()
    {
        // Toggle the mute state
        isMuted = !isMuted;

        // Call the mute function in AudioManager
        FindObjectOfType<AudioManager>().OnMuteButtonClicked();


        // Update the switch sprites
        UpdateSwitch();
    }

    void UpdateSwitch()
    {
        if (isMuted)
        {
            switchOn.SetActive(false);
            switchOff.SetActive(true);
        }
        else
        {
            switchOn.SetActive(true);
            switchOff.SetActive(false);
        }
    }
}
