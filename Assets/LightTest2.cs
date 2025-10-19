using UnityEngine;
using UnityEngine.U2D;
using System.Collections;
using System.Collections.Generic;

public class LightTest2 : MonoBehaviour
{
    [Header("Light Shape Settings")]
    public int rayCount = 50;
    public float rayLength = 10f;
    public float lightAngle = 360;
    public LayerMask obstacleLayer;
    public SpriteShapeController lightShapeController;

    [Header("Shrink Effect Settings")]
    public float shrinkGrowDuration = 0.25f;
    public float holdDuration = 1f;

    // --- ADDED: Looping Pulse Effect Settings ---
    [Header("Looping Pulse Effect")]
    public bool enablePulseEffect = true;
    public float pulseSpeed = 1f;
    [Range(0f, 2f)] public float minSizeMultiplier = 0.9f;
    [Range(0f, 2f)] public float maxSizeMultiplier = 1.1f;

    // --- Private Variables ---
    private Gradient pulseColor;
    private float initialRayLength;
    private Coroutine shrinkEffectCoroutine;
    private SpriteShapeRenderer spriteRenderer; // ADDED: To change color
    private bool isShrinkEffectActive = false; // ADDED: To manage effect priority

    // --- Your Existing Private Variables ---
    private MouseFollower player;
    private List<BearTrap> bearTrapList;
    private bool seesPlayer1;
    private int deathID;
    private List<GameObject> otherLightTests = new List<GameObject>();

    void Awake()
    {
        initialRayLength = rayLength;

        // --- ADDED: Get the renderer component ---
        spriteRenderer = lightShapeController.GetComponent<SpriteShapeRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("LightTest2 requires a SpriteShapeRenderer component on the same object as the SpriteShapeController.", this);
        }

        // --- 2. CALL A NEW METHOD TO BUILD THE GRADIENT ---
        InitializePulseGradient();

        // --- Your Existing Awake Logic (Unchanged) ---
        bearTrapList = new List<BearTrap>();

        if (transform.name == "LightEnemy (1)")
        {
            try { otherLightTests.Add(GameObject.Find("LightEnemy (2)")); } catch { }
            try { otherLightTests.Add(GameObject.Find("LightEnemy (3)")); } catch { }
        }

        if (transform.name == "LightEnemy (2)")
        {
            try { otherLightTests.Add(GameObject.Find("LightEnemy (1)")); } catch { }
            try { otherLightTests.Add(GameObject.Find("LightEnemy (3)")); } catch { }
        }

        if (transform.name == "LightEnemy (3)")
        {
            try { otherLightTests.Add(GameObject.Find("LightEnemy (2)")); } catch { }
            try { otherLightTests.Add(GameObject.Find("LightEnemy (1)")); } catch { }
        }
    }

    // --- 3. NEW METHOD TO CREATE THE GRADIENT IN CODE ---
    private void InitializePulseGradient()
    {
        pulseColor = new Gradient();

        // Define the COLOR keys for the gradient.
        // A GradientColorKey takes a (Color, time) pair. Time is from 0.0 to 1.0.
        GradientColorKey[] colorKeys = new GradientColorKey[2];
        colorKeys[0] = new GradientColorKey(new Color(1.0f, 0.9f, 0.2f), 0.0f); // Dim Yellow at the start
        colorKeys[1] = new GradientColorKey(new Color(1.0f, 0.5f, 0.0f), 1.0f); // Bright Orange at the peak

        // Define the ALPHA (opacity) keys for the gradient to create the fade.
        // A GradientAlphaKey takes an (alpha, time) pair.
        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[3];
        alphaKeys[0] = new GradientAlphaKey(0.3f, 0.0f);  // 30% opacity at the start
        alphaKeys[1] = new GradientAlphaKey(1.0f, 0.5f);  // 100% opacity at the middle of the pulse
        alphaKeys[2] = new GradientAlphaKey(0.3f, 1.0f);  // 30% opacity at the end

        // Apply the color and alpha keys to the gradient.
        pulseColor.SetKeys(colorKeys, alphaKeys);
    }

    // --- ADDED: Start method to launch the looping coroutine ---
    void Start()
    {
        if (enablePulseEffect && spriteRenderer != null)
        {
            StartCoroutine(LoopingPulseCoroutine());
        }
    }

    // --- Your Existing OnEnable/OnDisable (Unchanged) ---
    void OnEnable()
    {
        TokenManager.OnTokenCollected += ShrinkLightForDuration;
    }

    void OnDisable()
    {
        TokenManager.OnTokenCollected -= ShrinkLightForDuration;
    }

    // --- ADDED: New coroutine for the pulse effect ---
    private IEnumerator LoopingPulseCoroutine()
    {
        while (enablePulseEffect)
        {
            float pulseProgress = (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f;
            spriteRenderer.color = pulseColor.Evaluate(pulseProgress);

            if (!isShrinkEffectActive)
            {
                rayLength = Mathf.Lerp(initialRayLength * minSizeMultiplier, initialRayLength * maxSizeMultiplier, pulseProgress);
            }

            yield return null;
        }
    }

    // --- Your Existing ShrinkLightForDuration (Unchanged) ---
    private void ShrinkLightForDuration()
    {
        if (shrinkEffectCoroutine != null)
        {
            StopCoroutine(shrinkEffectCoroutine);
        }
        shrinkEffectCoroutine = StartCoroutine(ShrinkEffectCoroutine());
    }

    // --- MODIFIED: ShrinkEffectCoroutine now includes the priority flag ---
    private IEnumerator ShrinkEffectCoroutine()
    {
        isShrinkEffectActive = true; // Take control of the light's size

        float startLength = rayLength;
        float targetLength = initialRayLength * 0.5f;
        float elapsedTime = 0f;

        while (elapsedTime < shrinkGrowDuration)
        {
            rayLength = Mathf.Lerp(startLength, targetLength, elapsedTime / shrinkGrowDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        rayLength = targetLength;

        yield return new WaitForSeconds(holdDuration);

        startLength = rayLength;
        elapsedTime = 0f;
        while (elapsedTime < shrinkGrowDuration)
        {
            rayLength = Mathf.Lerp(startLength, initialRayLength, elapsedTime / shrinkGrowDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        rayLength = initialRayLength;

        isShrinkEffectActive = false; // Release control back to the looping pulse
    }

    // --- Your Existing Update (Unchanged) ---
    void Update()
    {
        UpdateLightShape();
    }

    // --- Your Existing UpdateLightShape (Unchanged) ---
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

        seesPlayer1 = false;

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
                    seesPlayer1 = true;
                    //player = hit.collider.GetComponent<MouseFollower>();
                    //StartCoroutine(KillTimer());
                }

                if (hit.transform.tag == "trap")
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
    }

    // --- ALL YOUR REMAINING METHODS (Unchanged) ---
    private IEnumerator KillTimer()
    {
        yield return new WaitForSeconds(0.5f);
        if (seesPlayer1)
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

    public void OnKill()
    {
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

    public void OnKillReset()
    {
        deathID = 0;
    }

    public void ResetRays(int id)
    {
        rayLength = initialRayLength;

        if (id == 1)
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