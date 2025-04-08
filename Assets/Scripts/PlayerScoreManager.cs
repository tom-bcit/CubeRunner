
using UnityEngine;
using TMPro; // For TextMeshPro

public class PlayerScoreManager : MonoBehaviour
{
    public static PlayerScoreManager instance; // Singleton to access score globally
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

    /** The lower your score, the better you are doing. Kinda like how golf points work. */
    public void CheckColor()
    {
        if (EnemyScoreManager.instance.GetScore() > score) // Good for Player; Player is winning
        {
            scoreText.color = Color.green;
        } 
        else if (EnemyScoreManager.instance.GetScore() < score) // Bad for Player; Player is losing
        {
            scoreText.color = Color.red;
        }
        else if (EnemyScoreManager.instance.GetScore() == score) // Player and Enemy are tied
        {
            scoreText.color = Color.white;
        }
    }
}
