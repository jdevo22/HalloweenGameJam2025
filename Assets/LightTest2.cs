using UnityEngine;
using UnityEngine.U2D;
using System.Collections;
using NUnit.Framework;
using System.Collections.Generic;

public class LightTest2 : MonoBehaviour
{
    // ... (rayCount, rayLength, etc. are unchanged) ...
    public int rayCount = 50;
    public float rayLength = 10f;
    public float lightAngle = 360;
    public LayerMask obstacleLayer;
    public SpriteShapeController lightShapeController;

    // 1. ADD PUBLIC VARIABLES FOR TIMING
    [Header("Shrink Effect Settings")]
    [Tooltip("How long the shrink and grow animations take.")]
    public float shrinkGrowDuration = 0.25f;
    [Tooltip("How long the light stays small.")]
    public float holdDuration = 1f;

    private MouseFollower player;
    private float initialRayLength;
    private Coroutine shrinkEffectCoroutine; // 2. Variable to track the running coroutine

    private List<BearTrap> bearTrapList;

    private bool seesPlayer;

    private List<GameObject> otherLightTests = new List<GameObject>();

    void Awake()
    {
        initialRayLength = rayLength;
        bearTrapList = new List<BearTrap>();

        if(transform.name == "LightEnemy (1)")
        {
            try
            {
                otherLightTests.Add(GameObject.Find("LightEnemy (2)"));
            }
            catch
            {

            }
            try
            {
                otherLightTests.Add(GameObject.Find("LightEnemy (3)"));
            }
            catch
            {

            }
        }

        if (transform.name == "LightEnemy (2)")
        {
            try
            {
                otherLightTests.Add(GameObject.Find("LightEnemy (1)"));
            }
            catch
            {

            }
            try
            {
                otherLightTests.Add(GameObject.Find("LightEnemy (3)"));
            }
            catch
            {

            }
        }

        if (transform.name == "LightEnemy (3)")
        {
            try
            {
                otherLightTests.Add(GameObject.Find("LightEnemy (2)"));
            }
            catch
            {

            }
            try
            {
                otherLightTests.Add(GameObject.Find("LightEnemy (1)"));
            }
            catch
            {

            }
        }
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
        if (bearTrapList.Count > 0)
        {
            for (int i = 0; i < bearTrapList.Count; i++)
            {
                bearTrapList[i].HideTrap();
            }
            bearTrapList.Clear();
        }

        seesPlayer = false;
        
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
                    
                    seesPlayer = true;
                    player = hit.collider.GetComponent<MouseFollower>();
                    StartCoroutine(KillTimer());

                }

                if(hit.transform.tag == "trap")
                {
                    bearTrapList.Add(hit.transform.GetComponent<BearTrap>());
                }
            }
            else
            {
                hitPoint = transform.position + rayDirection * rayLength;
            }
            lightShapeController.spline.InsertPointAt(i + 1, transform.InverseTransformPoint(hitPoint));
            lightShapeController.spline.SetHeight(i + 1, 0);
        }
        for (int i = 0; i < bearTrapList.Count; i++)
        {
            bearTrapList[i].RevealTrap();
        }

        float normalizedSin = (Mathf.Sin(Time.time * 1) + 1f) / 2f;

        // Use Lerp (Linear Interpolation) to map the 0-1 value to our desired alpha range.
        float targetAlpha = Mathf.Lerp(100, 184, normalizedSin);

        Color currentColor = lightShapeController.spriteShapeRenderer.color;

        lightShapeController.spriteShapeRenderer.color = new Color(currentColor.r, targetAlpha, currentColor.b, currentColor.a);

        Debug.Log(lightShapeController.spriteShapeRenderer.color);

    }

    private IEnumerator KillTimer()
    {
        yield return new WaitForSeconds(0.5f);
        if (seesPlayer)
        {
            player.OnDeath();
            player.GetComponent<BoxCollider2D>().enabled = false;
            ResetRays(0);
            for (int i = 0; i < otherLightTests.Count; i++)
            {
                if (otherLightTests[i] != null)
                {
                    otherLightTests[i].GetComponent<LightTest2>().ResetRays(0);
                }
            }
            rayLength = initialRayLength;
        }


    }

    public void ResetRays(int id)
    {
        rayLength = initialRayLength;

        if(id == 1)
        {
            for (int i = 0; i < otherLightTests.Count; i++)
            {
                if (otherLightTests[i] != null)
                {
                    otherLightTests[i].GetComponent<LightTest2>().ResetRays(0);
                }
            }
        }
    }


}