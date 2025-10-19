using System.Collections;
using UnityEngine;

public class AnimationCycle : MonoBehaviour
{
    public Sprite[] images;
    private int currentImage;
    public float speed;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(AnimationLoop());
    }

    private IEnumerator AnimationLoop()
    {
        yield return new WaitForSeconds(speed);
        currentImage++;

        if (currentImage == images.Length)
        {
            currentImage = 0;
        }

        
        spriteRenderer.sprite = images[currentImage];

        Start();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
