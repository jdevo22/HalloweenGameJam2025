using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;

public class TokenManager : MonoBehaviour
{
    public GameObject particleEffectPrefab;
    public static event Action OnTokenCollected;
    //public GameObject particleEffectPrefab;
    private int goalScore;
    private int currentScore;
    private List<GameObject> tokens;
    [SerializeField] private AudioClip collectSound;
    private AudioSource audioSource;
    private Vector3 mouseWorldPosition;
    private bool particlesActive = false;
    private GameObject particles;

    // ... (Awake method is unchanged) ...
    void Awake()
    {
        tokens = new List<GameObject>();
        mouseWorldPosition = new Vector3();
        foreach (Transform child in transform)
        {
            tokens.Add(child.gameObject);
            child.gameObject.SetActive(true);
        }
        goalScore = tokens.Count;
        currentScore = 0;
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        Debug.Log("Token Manager initialized. Goal Score: " + goalScore);
    }
    private void Update()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;
        mouseScreenPosition.z = Camera.main.WorldToScreenPoint(transform.position).z;

        // Convert mouse position from screen to world space
        mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
        particleEffectPrefab.transform.position = mouseWorldPosition; //gets positions for particle
        //var mainModule = particleEffectPrefab.main;
        if (particlesActive == false) 
        {
            particles = Instantiate(particleEffectPrefab, mouseWorldPosition, Quaternion.identity); //creates the clone
            particles.GetComponent<ParticleSystem>().Play();

            particlesActive = true;

        }
    }

    //timer to reset particlesActive

    public void CollectToken(GameObject token)
    {
        token.SetActive(false);
        var mainModule = particleEffectPrefab;
        mainModule.playOnAwake = true;

       //    var mainModule = targetParticleSystem.main; //access particle system
       //    mainModule.playOnAwake = true; //sets view on
        currentScore++;
        Debug.Log("Token collected! Score: " + currentScore + "/" + goalScore);

        // 1. ADD THIS 'IF' CHECK
        // Only invoke the event if there are still tokens left to collect.
        if (currentScore < goalScore)
        {
            OnTokenCollected?.Invoke();
           
            //make loop for if enabled set to mouse world position
           

        }

        if (collectSound != null)
        {
            audioSource.PlayOneShot(collectSound);
        }

        // The check for level completion happens after the event call.
        if (currentScore >= goalScore)
        {
            CompleteLevel();
        }
    }

    // ... (OnDeath and CompleteLevel methods are unchanged) ...
    public void OnDeath()
    {
        Debug.Log("Player died. Resetting tokens.");
        currentScore = 0;
        foreach (GameObject token in tokens)
        {
            token.SetActive(true);
        }
    }

    private void CompleteLevel()
    {
        Debug.Log("Level Complete! Loading next level.");
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        if (nextSceneIndex >= SceneManager.sceneCountInBuildSettings)
        {
            nextSceneIndex = 0;
        }
        SceneManager.LoadScene(nextSceneIndex);
    }
}