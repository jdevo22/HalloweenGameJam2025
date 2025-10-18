using UnityEngine;

// This ensures the token has a Collider2D component
[RequireComponent(typeof(Collider2D))]
public class Token : MonoBehaviour
{
    // We can set this tag in the Inspector. Default is "Player".
    [SerializeField] private string playerTag = "Player";

    private TokenManager tokenManager;
    private bool isCollected = false; // Prevents double collection

    void Start()
    {
        // Find the TokenManager in the scene when the token is created.
        // This assumes you only have ONE TokenManager.
        tokenManager = GetComponentInParent<TokenManager>();

        if (tokenManager == null)
        {
            Debug.LogError("Token could not find TokenManager in the scene!");
        }

        // Ensure the collider is set to "Is Trigger" so the player can pass through it
        GetComponent<Collider2D>().isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object that entered is the player AND the token hasn't been collected yet
        if (other.CompareTag(playerTag) && !isCollected)
        {
            // DO NOT UNCOMMENT
            //isCollected = true; // Mark as collected
            
            // Tell the TokenManager that this specific token was collected
            tokenManager.CollectToken(this.gameObject);
        }
    }
}