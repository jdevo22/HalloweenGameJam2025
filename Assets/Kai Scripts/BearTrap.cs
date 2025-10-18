using UnityEngine;

public class BearTrap : MonoBehaviour
{
    [SerializeField] private AudioSource triggerSound;
    [SerializeField] private Animator animator;

    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
        {
            return;
        }

        Debug.Log("Object is Player");

        MouseFollower player = collision.GetComponent<MouseFollower>();            

        if (player != null && !triggered)
        {
            triggered = true;

            // Call Jackson's death function
            player.OnDeath();

            // Optional: trigger trap animation & sound
            if (animator != null)
                animator.SetTrigger("Snap");

            if (triggerSound != null)
                triggerSound.Play();

            triggered = false;
        }
        
    }
}
