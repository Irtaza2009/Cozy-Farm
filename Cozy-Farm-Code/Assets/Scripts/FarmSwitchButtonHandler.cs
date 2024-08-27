using UnityEngine;
using UnityEngine.SceneManagement;

public class FarmSwitchButtonHandler : MonoBehaviour
{

    public void SwitchToLeaderboard()
    {
      
        PlayerPrefs.SetInt("previousScene", SceneManager.GetActiveScene().buildIndex);
        PlayerPrefs.Save();
         FindObjectOfType<AudioManager>().Play("Click");
        SceneManager.LoadScene("Leaderboard");
      
    }
    public void SwitchToChickenFarm()
    {
        GameManager.Instance.SaveGameState();
        FindObjectOfType<AudioManager>().Play("Click");
        SceneManager.LoadScene("ChickenFarm");
        //GameManager.Instance.LoadGameState();
    }

    public void SwitchToCowFarm()
    {
        GameManager.Instance.SaveGameState();
         FindObjectOfType<AudioManager>().Play("Click");
        SceneManager.LoadScene("CowFarm");
       // GameManager.Instance.LoadGameState();
    }

     public void SwitchToGarden()
    {
        GameManager.Instance.SaveGameState();
         FindObjectOfType<AudioManager>().Play("Click");
        SceneManager.LoadScene("Garden");
        // GameManager.Instance.LoadGameState();
    }

    public void SwitchToPreviousScene()
    {

        if (PlayerPrefs.GetInt("previousScene") == 0)
        {
            PlayerPrefs.SetInt("previousScene", 1);
            PlayerPrefs.Save();
        }
        Debug.Log("Switching to previous scene: " + PlayerPrefs.GetInt("previousScene"));
         FindObjectOfType<AudioManager>().Play("Click");
        SceneManager.LoadScene(PlayerPrefs.GetInt("previousScene"));


    }



}
