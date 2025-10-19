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
    private Vector2 initialPos;
    private MouseLock mouseLock;

    private bool isBlocked;
    private bool isDying;

    public UnityEvent onDeath;
    public UnityEvent onEscapeDeath;

    RaycastHit2D lightHit;

    public float deathTime;



    private void Awake()
    {
        mainCam = Camera.main;
        initialPos = transform.position;
        mouseLock = GetComponent<MouseLock>();
    }

    private IEnumerator DeathTimer()
    {
        Debug.Log("close to dead");
        yield return new WaitForSeconds(deathTime);
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
            return;
        }
        Debug.Log("wow")
;
        if (raycastHit.collider != null && !isBlocked)
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
                Debug.Log("token");
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
