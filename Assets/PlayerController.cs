using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    private Vector2 position;
    private Vector2 startPosition;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.transform.position = Mouse.current.position.value;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void OnDeath()
    {
        

    }
}
