using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class SpiderMovement : MonoBehaviour
{
    private Camera mainCam;
    [SerializeField] private LayerMask blockingLayers;
    [SerializeField] private LayerMask lightLayer;
    [SerializeField] private float rotationSpeed = 15f; // Controls how fast the spider turns
    private Vector2 initialPos;
    private Vector3 lastMousePosition; // Stores the mouse position from the previous frame
    private MouseLock mouseLock;

    private bool isBlocked;
    private bool isDying;

    public UnityEvent onDeath;
    public UnityEvent onEscapeDeath;

    RaycastHit2D lightHit;



    private void Awake()
    {
        mainCam = Camera.main;
        initialPos = transform.position;
        mouseLock = GetComponent<MouseLock>();
    }

    private IEnumerator DeathTimer()
    {
        Debug.Log("close to dead");
        yield return new WaitForSeconds(0.5f);
        Debug.Log("almost dead");
        if(isDying)
        {
            this.transform.position = initialPos;
            mouseLock.Start();
            onDeath.Invoke();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null)
        {
            Debug.Log("collision");
            if (collision.transform.tag == "trap")
            {
                Debug.Log("hmm");
                if (isDying == false)
                {
                    StartCoroutine(DeathTimer());
                    isDying = true;
                }
                
            }
        }

        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision != null)
        {
            if (collision.transform.tag == "trap")
            {
                Debug.Log("not dying");
                isDying = false;
                onEscapeDeath.Invoke();
            }
        }
    }

    private void Update()
    {
        Vector3 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D raycastHit = Physics2D.Raycast(mousePos, new Vector2(0.0f, 0), 0.1f, blockingLayers);
        lightHit = Physics2D.Raycast(mousePos, new Vector2(0.0f, -0.1f), 0.1f, lightLayer);
        Vector2 targetPos;

        

        if (raycastHit.collider == null && !isBlocked)
        {
            targetPos = mousePos;
            this.gameObject.transform.position = targetPos;
            // --- REVISED ROTATION LOGIC ---

            // 1. Calculate the direction the mouse has moved since the last frame.
            Vector2 movementDirection = mousePos - lastMousePosition;

            // 2. Only update rotation if the mouse has actually moved.
            // This prevents the spider from snapping to a default rotation when the mouse is still.
            if (movementDirection.sqrMagnitude > 0.001f)
            {
                // 3. Calculate the angle from the movement direction.
                float angle = Mathf.Atan2(movementDirection.y, movementDirection.x) * Mathf.Rad2Deg;
                Quaternion targetRotation = Quaternion.AngleAxis(angle - 90f, Vector3.forward);

                // 4. Smoothly rotate towards the target.
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            // 5. Update the last mouse position for the next frame.
            lastMousePosition = mousePos;
            return;
        }

        if (raycastHit.collider != null)
        {
            if (raycastHit.collider.tag == "obstacle")
            {
                Debug.Log("blocked = true");
                isBlocked = true;
            }

            if (raycastHit.collider.tag == "trap")
            {
                Debug.Log("Dieing");
                this.transform.position = initialPos;
                mouseLock.Start();
                onDeath.Invoke();

            }

            if(raycastHit.collider.tag == "token")
            {
                Token token = raycastHit.collider.GetComponent<Token>();
                token.HitByPlayer();
            }
        }

        

        
        
        if(isBlocked)
        {
            Vector3 direction = mousePos - transform.position;

            Vector2 dir = direction; // NO Z ALLLOWED - JACKSON

            RaycastHit2D clearanceTest = Physics2D.Raycast(mousePos, dir.normalized, 0.2f, blockingLayers);
            if (clearanceTest.collider == null && Vector3.Distance(mousePos, transform.position) < 10.005f)
            {
                isBlocked = false;
            }
            else
            {
                return;
            }
        }

        targetPos = mousePos;
        this.gameObject.transform.position = targetPos;
    }




}
