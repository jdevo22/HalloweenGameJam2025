using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AnimationCycle : MonoBehaviour
{
    public Sprite[] images;
    private int currentImage;
    public float speed;
    private SpriteRenderer spriteRenderer;
    private Image image;

    bool isImage = false;

    private void Awake()
    {
        try
        {
            image = GetComponent<Image>();
            isImage = true;
        }
        catch 
        { 
            spriteRenderer = GetComponent<SpriteRenderer>(); 
            isImage =false;
        }
        
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(isImage)
        {
            StartCoroutine(AnimationLoopImage());
        }
        else
        {
            StartCoroutine(AnimationLoopSprite());
        }
            
    }

    private IEnumerator AnimationLoopSprite()
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

    private IEnumerator AnimationLoopImage()
    {
        yield return new WaitForSeconds(speed);
        currentImage++;

        if (currentImage == images.Length)
        {
            currentImage = 0;
        }


        image.sprite= images[currentImage];

        Start();

    }
}
