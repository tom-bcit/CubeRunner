using UnityEngine;
using TMPro; // For TextMeshPro

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance; // Singleton to access score globally
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

    public void AddPoint()
    {
        Debug.Log("Increasing Score!!");
        score++; // Increase the score
        scoreText.text = "Score: " + score; // Update the UI
    }
}
