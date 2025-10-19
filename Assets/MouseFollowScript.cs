using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.InputSystem;
using System.Collections;

/// <summary>
/// This script makes a 2D GameObject continuously follow the mouse cursor's position in the game world.
/// </summary>
/// 
// tutorial used: https://www.youtube.com/watch?v=mRkFj8J7y_I
public class MouseFollower : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("The fastest speed when the mouse is close.")]
    [SerializeField] private float fastSpeed = 15f;

    [Tooltip("The slowest speed when the mouse is far.")]
    [SerializeField] private float slowSpeed = 4f;

    [Tooltip("The distance at which the object reaches its fastest speed.")]
    [SerializeField] private float nearDistanceThreshold = 1f;

    [Tooltip("The distance at which the object slows down to its slowest speed.")]
    [SerializeField] private float farDistanceThreshold = 10f;

    [Header("Line of Sight")]
    [Tooltip("Which layers should block the raycast between this object and the mouse.")]
    [SerializeField] private LayerMask blockingLayers;

    [Header("Sprite Assets")]
    [Tooltip("The sprite to use when the player is alive.")]
    [SerializeField] private Sprite liveSprite;

    [Tooltip("The sprite to use when the player is dead.")]
    [SerializeField] private Sprite deadSprite;

    [Header("Sprite Direction Rotation Settings")]
    [Tooltip("How fast the player sprite rotates. Higher = snappier.")]
    [SerializeField] private float rotationSpeed = 20f;

    [Tooltip("The 'dead zone' radius. Won't update rotation if mouse is this close.")]
    [SerializeField] private float minDistanceToUpdate = 0.1f;

    // A private variable to store the main camera. Caching is more efficient.
    private Camera mainCamera;

    private Vector2 startPos;
    
    // --- Kai's Destabilization Variables ---
    private Vector2 externalMovementOffset = Vector2.zero;
    private float destabilizeTimer = 0f;

    public Transform[] lightTransform;
    private Vector2 lightPos;

    private Quaternion targetRotation; // For Sprite Direction

    private bool isResurrected = false;
    public BearTrapManager beartraps = new BearTrapManager();
    private LightMovement[] light;
    public TokenManager tokenManager; //Drag into component in inspector

    /// <summary>
    /// Called when the script instance is being loaded.
    /// </summary>
    void Start()
    {
        light = new LightMovement[lightTransform.Length];
        startPos = transform.position;
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("MouseFollower Script Error: No main camera found. Tag a camera as 'MainCamera'.");
        }

        for (int i = 0; i < lightTransform.Length; i++)
        {
            light[i] = lightTransform[i].GetComponent<LightMovement>();
        }
    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    void Update()
    {
        if (mainCamera == null || !isResurrected) return;

        // Get the mouse position in world coordinates.
        Vector3 mouseWorldPosition = GetMouseWorldPosition();

        

        // --- NEW: RAYCAST LOGIC ---

        // 1. Check for a collider directly under the mouse cursor.
        Collider2D cursorHitCollider = Physics2D.OverlapPoint(mouseWorldPosition, blockingLayers);

        // 2. Perform a raycast from the object to the mouse position.
        Vector2 direction = (Vector2)mouseWorldPosition - (Vector2)transform.position;
        float distance = direction.magnitude;
        RaycastHit2D raycastHit = Physics2D.Raycast(transform.position, direction, distance, blockingLayers);

        // Kai messing with stuff
        direction += externalMovementOffset;

        // For debugging: Draw the ray in the Scene view. It will be green if clear, red if blocked.
        Color rayColor = raycastHit.collider == null ? Color.green : Color.red;
        //Debug.DrawRay(transform.position, direction, rayColor);

        // --- MODIFIED CONDITION ---
        // Only move if the cursor is not over a collider AND the raycast is not blocked.
        if (cursorHitCollider == null && raycastHit.collider == null)
        {
            // Calculate the speed based on distance.
            float currentDistance = Vector2.Distance(mouseWorldPosition, transform.position);
            float t = Mathf.InverseLerp(nearDistanceThreshold, farDistanceThreshold, currentDistance);
            float currentSpeed = Mathf.Lerp(fastSpeed, slowSpeed, t);

            // Move the object towards the mouse.
            transform.position = Vector3.MoveTowards(transform.position, mouseWorldPosition, currentSpeed * Time.deltaTime);
        }
        // If either check fails, the object stops moving.


        //Mouse sprite point direction

        // 1. Calculate Direction
        // Find the vector pointing from the player's position to the mouse's world position.
        Vector2 spriteDirection = new Vector2(
            mouseWorldPosition.x - transform.position.x,
            mouseWorldPosition.y - transform.position.y
        );

        // 3. --- NEW: DEAD ZONE CHECK ---
        // We check 'sqrMagnitude' because it's faster than 'magnitude' (avoids square root).
        // We only update the target angle if the mouse is outside our dead zone.
        if (spriteDirection.sqrMagnitude > minDistanceToUpdate * minDistanceToUpdate)
        {
            // 4. Calculate Angle
            float angle = Mathf.Atan2(spriteDirection.y, spriteDirection.x) * Mathf.Rad2Deg -90f;

            // 5. Store the Target Rotation
            // (We apply the 90-degree offset here if your sprite faces Up)
            // float angle = (Mathf.Atan2(spriteDirection.y, spriteDirection.x) * Mathf.Rad2Deg) - 90f;

            targetRotation = Quaternion.Euler(0, 0, angle);
        }

        // 6. --- NEW: SMOOTH ROTATION ---
        // Slerp (Spherical Linear Interpolation) smoothly moves from the
        // current rotation to the target rotation every frame.
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            Time.deltaTime * rotationSpeed
        );
    }
   

    /// <summary>
    /// Helper method to calculate the mouse position in world space at the object's Z-depth.
    /// </summary>
    /// <returns>The mouse position in world coordinates.</returns>
    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;
        mouseScreenPosition.z = mainCamera.WorldToScreenPoint(transform.position).z;

        // Convert mouse position from screen to world space
        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(mouseScreenPosition);

        // Apply destabilization effect (if active)
        mouseWorldPosition = GetDestabilizedTarget(mouseWorldPosition);

        return mouseWorldPosition;
    }

    public void OnDeath()
    {
        if (beartraps != null)
        {
            for (int i = 0; i < light.Length; i++)
            {
                light[i].OnReset();
            }

            if (beartraps != null)
            {
                beartraps.SwitchTraps();
            }
            transform.position = startPos;
            //this.GetComponent<BoxCollider2D>().enabled = true;
            isResurrected = false;
            Debug.Log("You die");
            this.GetComponent<BoxCollider2D>().isTrigger = true;

            if (tokenManager != null) //Call TokenManager OnDeath to reset tokens
            {
                tokenManager.OnDeath();
            }


            this.GetComponent<SpriteRenderer>().sprite = deadSprite;

        }
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        this.GetComponent<BoxCollider2D>().enabled = true;
        if (!context.started) return;
        var rayHit = Physics2D.GetRayIntersection(mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue()));
        if (!rayHit.collider) return;
        if (rayHit.collider.tag == "Player")
        {
            isResurrected = true;
            this.GetComponent<SpriteRenderer>().sprite = liveSprite;
        }

        for (int i = 0; i < light.Length; i++)
        {
            light[i].MouseClickedPlayer = true;
        }
        

    }

    // ================================
    // Kai's RedLight Destabilization Add-on
    // ================================

    [Header("Destabilization Settings")]
    [SerializeField] private float wiggleAmount = 0.5f;      // How far the input wobbles
    [SerializeField] private float wiggleSpeed = 8f;         // How fast the wobble oscillates
    [SerializeField] private float destabilizedDuration = 2f; // How long the effect lasts

    private bool isDestabilized = false;

    // This version modifies the *movement direction*, not the position itself
    private Vector3 GetDestabilizedTarget(Vector3 originalTarget)
    {
        if (!isDestabilized) return originalTarget;

        // Apply small sinusoidal offset to make the target "wiggle"
        float offsetX = Mathf.Sin(Time.time * wiggleSpeed) * wiggleAmount;
        float offsetY = Mathf.Cos(Time.time * wiggleSpeed * 1.5f) * wiggleAmount;
        return originalTarget + new Vector3(offsetX, offsetY, 0f);
    }

    // Call this when the player touches a RedLight trigger
    public void SetDestabilized(float customDuration = -1f, float customAmount = -1f, float customSpeed = -1f)
    {
        if (customAmount > 0) wiggleAmount = customAmount;
        if (customSpeed > 0) wiggleSpeed = customSpeed;

        float duration = (customDuration > 0) ? customDuration : destabilizedDuration;

        if (!isDestabilized)
            StartCoroutine(DestabilizeRoutine(duration));
    }

    private IEnumerator DestabilizeRoutine(float duration)
    {
        isDestabilized = true;
        yield return new WaitForSeconds(duration);
        isDestabilized = false;
    }



}
