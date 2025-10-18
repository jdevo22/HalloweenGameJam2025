using UnityEngine;
using System.Collections.Generic; // Required for using List
using UnityEngine.SceneManagement; // Required for changing levels

public class TokenManager : MonoBehaviour
{
    private int goalScore; // Set automatically by the number of child tokens
    private int currentScore; // Tracks currently collected tokens

    // This list will hold all token GameObjects for management
    private List<GameObject> tokens;

    // Optional: Add sound effects
    [SerializeField] private AudioClip collectSound;
    private AudioSource audioSource;

    void Awake()
    {
        // Initialize the list
        tokens = new List<GameObject>();

        // Find all child GameObjects (our tokens) and add them to the list
        foreach (Transform child in transform)
        {
            tokens.Add(child.gameObject);
            // Ensure all tokens are active at the start
            child.gameObject.SetActive(true);
        }

        // Set the goal score to the number of tokens we found
        goalScore = tokens.Count;
        currentScore = 0;

        // Get or add an AudioSource component to play sounds
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        Debug.Log("Token Manager initialized. Goal Score: " + goalScore);
    }

    /// <summary>
    /// This public method is called by the Token.cs script when it's collected.
    /// </summary>
    /// <param name="token">The specific token GameObject that was collected.</param>
    public void CollectToken(GameObject token)
    {
        // 1. Disable the token
        // We don't need to check if it's in the list, just disable it.
        token.SetActive(false);

        // 2. Increment score
        currentScore++;
        Debug.Log("Token collected! Score: " + currentScore + "/" + goalScore);

        // 3. Play sound (if one is assigned)
        if (collectSound != null)
        {
            audioSource.PlayOneShot(collectSound);
        }

        // 4. Check if the level is complete
        if (currentScore >= goalScore)
        {
            CompleteLevel();
        }
    }

    /// <summary>
    /// This method is called by the Player's script when the player dies.
    /// </summary>
    public void OnDeath()
    {
        Debug.Log("Player died. Resetting tokens.");

        // 1. Reset the current score
        currentScore = 0;

        // 2. Re-enable all tokens in the list
        foreach (GameObject token in tokens)
        {
            token.SetActive(true);
        }
    }

    private void CompleteLevel()
    {
        Debug.Log("Level Complete! Loading next level.");

        // --- Example: Load the next level in the build settings ---
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        // Check if there is a next level, otherwise loop back to the first level (index 0)
        if (nextSceneIndex >= SceneManager.sceneCountInBuildSettings)
        {
            nextSceneIndex = 0; // Or load a "You Win!"
        }

        SceneManager.LoadScene(nextSceneIndex);
    }
}