using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class Leaderboard : MonoBehaviour
{
    public TextMeshProUGUI leaderboardText;
    private List<int> scores = new List<int>();

    void Start()
    {
        // Load scores from PlayerPrefs
        LoadScores();
        UpdateLeaderboard();
    }

    public void AddScore(int score)
    {
        scores.Add(score);
        scores.Sort((a, b) => b.CompareTo(a)); // Sort in descending order
        SaveScores();
        UpdateLeaderboard();
    }

    private void LoadScores()
    {
        scores.Clear();
        for (int i = 0; i < 10; i++)
        {
            if (PlayerPrefs.HasKey("Score" + i))
            {
                scores.Add(PlayerPrefs.GetInt("Score" + i));
            }
        }
    }

    private void SaveScores()
    {
        for (int i = 0; i < scores.Count && i < 10; i++)
        {
            PlayerPrefs.SetInt("Score" + i, scores[i]);
        }
        PlayerPrefs.Save();
    }

    private void UpdateLeaderboard()
    {
        leaderboardText.text = "Leaderboard\n";
        for (int i = 0; i < scores.Count && i < 10; i++)
        {
            leaderboardText.text += (i + 1) + ". " + scores[i] + "\n";
        }
    }
}
