using UnityEngine;

public class GreenLight : MonoBehaviour
{
    [SerializeField] private float boostDuration = 3f;
    [SerializeField] private float boostMultiplier = 2f;
    
    //// Kai's Fields
    //[Header("Speed Boost Settings")]
    //[SerializeField] private float speedBoostMultiplier = 2f;   // How much faster
    //[SerializeField] private float speedBoostDuration = 3f;     // How long it lasts

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            MouseFollower player = collision.GetComponent<MouseFollower>();
            if (player != null)
            {
                player.SetSpeedBoost(boostDuration, boostMultiplier);
            }
        }
    }
    //// Kai's GreenLight Speed Boost Add-on
    //// ================================
    //private bool isSpeedBoosted = false;
    //private float originalFastSpeed;
    //private float originalSlowSpeed;

    //// Call this to temporarily increase movement speed
    //public void SetSpeedBoost(float customDuration = -1f, float customMultiplier = -1f)
    //{
    //    if (customMultiplier > 0f) speedBoostMultiplier = customMultiplier;
    //    float duration = (customDuration > 0f) ? customDuration : speedBoostDuration;

    //    if (!isSpeedBoosted)
    //        StartCoroutine(SpeedBoostRoutine(duration));
    //}

    //private IEnumerator SpeedBoostRoutine(float duration)
    //{
    //    isSpeedBoosted = true;

    //    // Store original speeds
    //    originalFastSpeed = fastSpeed;
    //    originalSlowSpeed = slowSpeed;

    //    // Apply multiplier
    //    fastSpeed *= speedBoostMultiplier;
    //    slowSpeed *= speedBoostMultiplier;

    //    yield return new WaitForSeconds(duration);

    //    // Restore original speeds
    //    fastSpeed = originalFastSpeed;
    //    slowSpeed = originalSlowSpeed;
    //    isSpeedBoosted = false;
    //}


}
