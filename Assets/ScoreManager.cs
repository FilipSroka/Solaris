using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private int score = 0;

    public void IncreaseScore()
    {
        score++;
        Debug.Log("Score: " + score);
    }
}
