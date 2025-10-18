using UnityEngine;
using UnityEngine.U2D;
using System.Collections;

public class LightTest2 : MonoBehaviour
{
    // ... (rayCount, rayLength, etc. are unchanged) ...
    public int rayCount = 50;
    public float rayLength = 10f;
    [Range(0, 360)]
    public float lightAngle = 90f;
    public LayerMask obstacleLayer;
    public SpriteShapeController lightShapeController;

    // 1. ADD PUBLIC VARIABLES FOR TIMING
    [Header("Shrink Effect Settings")]
    [Tooltip("How long the shrink and grow animations take.")]
    public float shrinkGrowDuration = 0.25f;
    [Tooltip("How long the light stays small.")]
    public float holdDuration = 15f;

    private MouseFollower player;
    private float initialRayLength;
    private Coroutine shrinkEffectCoroutine; // 2. Variable to track the running coroutine

    void Awake()
    {
        initialRayLength = rayLength;
    }

    void OnEnable()
    {
        TokenManager.OnTokenCollected += ShrinkLightForDuration;
    }

    void OnDisable()
    {
        TokenManager.OnTokenCollected -= ShrinkLightForDuration;
    }

    private void ShrinkLightForDuration()
    {
        // 3. Stop the previous coroutine if it's still running
        if (shrinkEffectCoroutine != null)
        {
            StopCoroutine(shrinkEffectCoroutine);
        }
        // Start the new one and store a reference to it
        shrinkEffectCoroutine = StartCoroutine(ShrinkEffectCoroutine());
    }

    // 4. REWRITTEN COROUTINE FOR SMOOTH ANIMATION
    private IEnumerator ShrinkEffectCoroutine()
    {
        float startLength = rayLength; // Current size
        float targetLength = initialRayLength * 0.5f; // Shrunken size
        float elapsedTime = 0f;

        // --- SHRINK PHASE ---
        while (elapsedTime < shrinkGrowDuration)
        {
            // Interpolate from the starting size to the target size over time
            rayLength = Mathf.Lerp(startLength, targetLength, elapsedTime / shrinkGrowDuration);
            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }
        rayLength = targetLength; // Ensure it ends exactly at the target size

        // --- WAIT PHASE ---
        yield return new WaitForSeconds(holdDuration);

        // --- GROW PHASE ---
        startLength = rayLength; // Current (shrunken) size
        elapsedTime = 0f;
        while (elapsedTime < shrinkGrowDuration)
        {
            // Interpolate from the shrunken size back to the original size
            rayLength = Mathf.Lerp(startLength, initialRayLength, elapsedTime / shrinkGrowDuration);
            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }
        rayLength = initialRayLength; // Ensure it ends exactly at the original size
    }

    // ... (Update and UpdateLightShape methods are unchanged) ...
    void Update()
    {
        UpdateLightShape();
    }

    void UpdateLightShape()
    {
        lightShapeController.spline.Clear();
        lightShapeController.spline.InsertPointAt(0, Vector3.zero);
        lightShapeController.spline.SetHeight(0, 0);

        float angleStep = lightAngle / (rayCount - 1);
        float startAngle = -lightAngle / 2;

        for (int i = 0; i < rayCount; i++)
        {
            float currentAngle = startAngle + i * angleStep;
            Vector3 rayDirection = Quaternion.Euler(0, 0, currentAngle) * transform.up;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDirection, rayLength, obstacleLayer);
            Vector3 hitPoint;
            if (hit.collider != null)
            {
                hitPoint = hit.point;
                if (hit.transform.tag == "Player")
                {
                    player = hit.collider.GetComponent<MouseFollower>();
                    player.OnDeath();
                }
            }
            else
            {
                hitPoint = transform.position + rayDirection * rayLength;
            }
            lightShapeController.spline.InsertPointAt(i + 1, transform.InverseTransformPoint(hitPoint));
            lightShapeController.spline.SetHeight(i + 1, 0);
        }
    }
}