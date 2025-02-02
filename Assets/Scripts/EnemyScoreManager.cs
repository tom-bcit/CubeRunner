using UnityEngine;
using TMPro; // For TextMeshPro

public class EnemyScoreManager : MonoBehaviour
{
    public static EnemyScoreManager instance; // Singleton to access score globally
    public TextMeshProUGUI scoreText; // Assign this in the Inspector
    private int score = 0;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject); // Prevent duplicates
        }
    }

    public int GetScore()
    {
        return score;
    }

    public void AddPoint()
    {
        Debug.Log("Increasing Score!!");
        score++; // Increase the score
        scoreText.text = "Score: " + score; // Update the UI
        // CheckColor(); // Change the color of the score
    }

    public void CheckColor()
    {
        if (PlayerScoreManager.instance.GetScore() > score) // Good for Enemy; Enemy is winning
        {
            scoreText.color = Color.green;
        } 
        else if (PlayerScoreManager.instance.GetScore() < score) // Bad for Enemy; Enemy is losing
        {
            scoreText.color = Color.red;
        }
        else if (PlayerScoreManager.instance.GetScore() == score) // Player and Enemy are tied
        {
            scoreText.color = Color.white;
        }
    }
}
