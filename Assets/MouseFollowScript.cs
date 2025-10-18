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

    

    [Header("Destabilization Settings")]
    [SerializeField] private float wiggleAmount = 0.5f;      // How far the input wobbles
    [SerializeField] private float wiggleSpeed = 8f;         // How fast the wobble oscillates
    [SerializeField] private float destabilizedDuration = 2f; // How long the effect lasts

    [Header("Line of Sight")]
    [Tooltip("Which layers should block the raycast between this object and the mouse.")]
    [SerializeField] private LayerMask blockingLayers;


    


    // A private variable to store the main camera. Caching is more efficient.
    private Camera mainCamera;

    private Vector2 startPos;
    
    // --- Kai's Destabilization Variables ---
    private Vector2 externalMovementOffset = Vector2.zero;
    private float destabilizeTimer = 0f;

    public Transform lightTransform;
    private Vector2 lightPos;

    private bool isResurrected = false;
    private LightMovement light = new LightMovement();
    public TokenManager tokenManager; //Drag into component in inspector

    /// <summary>
    /// Called when the script instance is being loaded.
    /// </summary>
    void Start()
    {
        startPos = transform.position;
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("MouseFollower Script Error: No main camera found. Tag a camera as 'MainCamera'.");
        }

        this.GetComponent<SpriteRenderer>().color = Color.red;

        light = lightTransform.GetComponent<LightMovement>();
    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    void Update()
    {
        lightPos = lightTransform.position;
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

        //Raycast to light for death check
        RaycastHit2D lightCheck = Physics2D.Raycast(this.transform.position, lightPos, 100, blockingLayers);

        Debug.DrawRay(transform.position, (Vector2)lightPos - (Vector2)transform.position, rayColor);
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
        light.OnReset();
        transform.position = startPos;
        isResurrected = false;
        Debug.Log("You die");
        this.GetComponent<BoxCollider2D>().isTrigger = true;
        //this.gameObject.layer = 0;

        if (tokenManager != null) //Call TokenManager OnDeath to reset tokens
        {
            tokenManager.OnDeath();
        }


        this.GetComponent<SpriteRenderer>().color = Color.red;
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        var rayHit = Physics2D.GetRayIntersection(mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue()));
        if (!rayHit.collider) return;
        Debug.Log("collider Hit");
        if (rayHit.collider.tag == "Player")
        {
            Debug.Log("player1");
            isResurrected = true;
            this.GetComponent<SpriteRenderer>().color = Color.white;

            //this.GetComponent<BoxCollider2D>().isTrigger = false;
            //this.gameObject.layer = 6;
        }
        
    }

    
    // Kai's RedLight Destabilization Add-on
    // ================================

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
