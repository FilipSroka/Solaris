using UnityEngine;

public class GoldenCube : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Destroy the golden cube
        if (other.gameObject.CompareTag("Player"))  // Adjust the tag as needed
        {
            // Get the ScoreManager component from the GameManager object
            CubeController scoreManager = GameObject.Find("CubeController").GetComponent<CubeController>();

            // Call the IncreaseScore method
            scoreManager.IncreaseScore();

            Destroy(gameObject);
        }
    }
}
