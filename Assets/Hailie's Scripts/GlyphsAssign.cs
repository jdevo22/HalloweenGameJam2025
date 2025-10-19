using UnityEngine;

public class GlyphsAssign : MonoBehaviour
{
    [Header("Image Layer Settings")]
    [Tooltip("Drag your available Sprite images here.")]
    public Sprite[] availableSprites;

    [Tooltip("The size multiplier for the randomly assigned image.")]
    public float imageScale = 0.5f; // Set a default smaller scale

    private GameObject imageLayer;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        // 1. Create a new child GameObject to hold the image layer
        imageLayer = new GameObject("Random Image Layer");
        imageLayer.transform.SetParent(transform);

        // Ensure the image layer is centered on the token
        imageLayer.transform.localPosition = Vector3.zero;

        // 2. Add a SpriteRenderer to the child object
        spriteRenderer = imageLayer.AddComponent<SpriteRenderer>();

        // 3. Assign the random image and set the scale
        AssignRandomImage();
    }

    void AssignRandomImage()
    {
        if (availableSprites.Length == 0)
        {
            Debug.LogWarning("The availableSprites array is empty. Please assign images in the Inspector.", this);
            return;
        }

        if (spriteRenderer == null) return; // Should not happen after Start, but a safe check

        // Select a random index
        int randomIndex = Random.Range(0, availableSprites.Length);

        // Assign the randomly selected Sprite
        spriteRenderer.sprite = availableSprites[randomIndex];

        // 4. Control the size of the image layer by setting its local scale
        imageLayer.transform.localScale = Vector3.one * imageScale;
    }
}