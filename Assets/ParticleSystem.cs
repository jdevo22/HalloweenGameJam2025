using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;

public class ParticleSystem : MonoBehaviour
{
    public GameObject objectToClone;
    private Queue<GameObject> cloneQueue = new Queue<GameObject>();
    public GameObject particleEffectPrefab;
    private Vector3 mouseWorldPosition;
    private bool particlesActive = false;
    private GameObject particles;
    public int maxClones = 10;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mouseWorldPosition = new Vector3();

    }

    // Update is called once per frame

    public void SpawnClone()
    {
        if (cloneQueue.Count >= maxClones)
        {
            GameObject newClone = Instantiate(particleEffectPrefab, mouseWorldPosition, Quaternion.identity);
            cloneQueue.Enqueue(newClone);
            Debug.Log("There are this many clones mother*******" + cloneQueue.Count);
        }
    }
    public void DestroyOldestClone()
    {
        if (cloneQueue.Count > 0)
        {
            GameObject oldestClone = cloneQueue.Dequeue();
            Destroy(oldestClone);
            Debug.Log("Destroyed the oldest clone , this many targets remain: " + cloneQueue.Count);
        }
        else
        {
            Debug.Log("No clones to destroy ):");
        }
    }
    void Update()
    {

        SpawnClone();

        DestroyOldestClone();
    
    }
  //      Vector3 mouseScreenPosition = Input.mousePosition;
  //      mouseScreenPosition.z = Camera.main.WorldToScreenPoint(transform.position).z;

        // Convert mouse position from screen to world space
  //      mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
   //     particleEffectPrefab.transform.position = mouseWorldPosition; //gets positions for particle
                                                                      //var mainModule = particleEffectPrefab.main;
  //      if (particlesActive == true)
   //     {
   //         particles = Instantiate(particleEffectPrefab, mouseWorldPosition, Quaternion.identity); //creates the clone
    //        object value = particles.GetComponent<ParticleSystem>().Play();
}
