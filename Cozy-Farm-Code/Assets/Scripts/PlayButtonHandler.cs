using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class PlayButtonHandler : MonoBehaviour
{
    public TMP_InputField nameInputField; 
    public Button playButton;         
    public TextMeshProUGUI errorMessage;         

    void Start()
    {

        // Delete all playerprefs
        // PlayerPrefs.DeleteAll();
        // PlayerPrefs.Save();

        // Disable the play button initially
        playButton.interactable = false;

        nameInputField.text = PlayerPrefs.GetString("PlayerName", ""); // Loading the player's name from PlayerPrefs
        ValidateName(); // Validate the player's name after getting it from PlayerPrefs

        // Add listener to the input field to check for text changes
        nameInputField.onValueChanged.AddListener(delegate { ValidateName(); });

        // Add listener to the play button to handle the click event
        playButton.onClick.AddListener(LoadMainScene);


    }

    // Method to validate the input field for the player's name
    void ValidateName()
    {
        if (string.IsNullOrEmpty(nameInputField.text))
        {

            playButton.interactable = false;
            errorMessage.text = "Please enter your name.";
        }
        else
        {
            playButton.interactable = true;
            errorMessage.text = ""; // Clear the error message
        }
    }

    // Method to load the main scene if the player's name is entered
    void LoadMainScene()
    {
        if (!string.IsNullOrEmpty(nameInputField.text))
        {
            // Save the player's name in PlayerPrefs
            PlayerPrefs.SetString("PlayerName", nameInputField.text);
            PlayerPrefs.Save();

            // Load the main scene
            SceneManager.LoadScene("Main");
        }
        else
        {
            // Display the error message if no name is entered
            errorMessage.text = "Please enter your name.";
        }
    }
}
