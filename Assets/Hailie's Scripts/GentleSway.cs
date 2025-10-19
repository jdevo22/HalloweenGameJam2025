using UnityEngine;

public class GentleSway : MonoBehaviour
{
    [Header("Sway Movement Settings")]
    [Tooltip("How far the object moves from its starting position in X (left/right).")]
    public float swayMagnitudeX = 0.5f;

    [Tooltip("How fast the object moves back and forth in the X-axis.")]
    public float swaySpeedX = 1f;

    [Tooltip("How far the object moves from its starting position in Y (up/down).")]
    public float swayMagnitudeY = 0.5f;

    [Tooltip("How fast the object moves up and down in the Y-axis.")]
    public float swaySpeedY = 1f;

    [Tooltip("A phase shift to make the X and Y movement out of sync.")]
    public float swayPhaseOffset = 2f;

    private Vector3 startPosition;

    void Start()
    {
        // Store the object's initial position when the game starts.
        startPosition = transform.position;
    }

    void Update()
    {
        // Calculate the horizontal (X) sway offset
        // Mathf.Sin(Time.time * speed) returns a value between -1 and 1
        float xOffset = Mathf.Sin(Time.time * swaySpeedX) * swayMagnitudeX;

        // Calculate the vertical (Y) sway offset
        // Adding swayPhaseOffset makes the Y movement slightly out of sync with X, 
        // creating a more natural, gentle "figure-eight" or elliptical sway.
        float yOffset = Mathf.Sin(Time.time * swaySpeedY + swayPhaseOffset) * swayMagnitudeY;

        // Apply the new position. The Z position remains the same.
        transform.position = new Vector3(
            startPosition.x + xOffset,
            startPosition.y + yOffset,
            startPosition.z
        );
    }
}
