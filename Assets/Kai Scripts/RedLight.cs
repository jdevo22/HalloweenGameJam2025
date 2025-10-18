using UnityEngine;

public class RedLight : MonoBehaviour
{
    [SerializeField] private float effectDuration = 2f;
    [SerializeField] private float effectAmount = 0.16f;
    [SerializeField] private float effectSpeed = 12f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            MouseFollower player = collision.GetComponent<MouseFollower>();
            if (player != null)
            {
                player.SetDestabilized(effectDuration, effectAmount, effectSpeed);
            }
        }
    }
}
