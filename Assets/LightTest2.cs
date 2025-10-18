using UnityEngine;
using UnityEngine.U2D;

public class LightTest2 : MonoBehaviour
{
    [Tooltip("The number of rays to cast. Higher numbers are smoother.")]
    public int rayCount = 50;

    [Tooltip("The maximum length of the light rays.")]
    public float rayLength = 10f;

    [Tooltip("The total angle of the light cone in degrees.")]
    [Range(0, 360)]
    public float lightAngle = 90f;

    [Tooltip("The LayerMask that the light rays will collide with.")]
    public LayerMask obstacleLayer;

    public SpriteShapeController lightShapeController;

    private MouseFollower player;

    void Update()
    {
        UpdateLightShape();
    }

    void UpdateLightShape()
    {
        // 1. Prepare for the new shape by clearing the old one
        lightShapeController.spline.Clear();

        // 2. Add the origin point of the light (the center)
        // We use transform.position which is in world space, but the SpriteShapeController
        // handles the local space conversion automatically.
        lightShapeController.spline.InsertPointAt(0, Vector3.zero);
        lightShapeController.spline.SetHeight(0, 0);
        // 3. Calculate and cast rays
        float angleStep = lightAngle / (rayCount - 1);
        float startAngle = -lightAngle / 2;

        for (int i = 0; i < rayCount; i++)
        {
            float currentAngle = startAngle + i * angleStep;
            // Use transform.up as the forward direction for 2D, or adjust as needed.
            // Here, I'm using Quaternion.Euler to rotate transform.up by the current angle.
            Vector3 rayDirection = Quaternion.Euler(0, 0, currentAngle) * transform.up;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDirection, rayLength, obstacleLayer);

            Vector3 hitPoint;
            if (hit.collider != null)
            {
                // We hit something, so the point is the collision point.
                hitPoint = hit.point;

                if (hit.transform.tag == "Player")
                {
                    player = hit.collider.GetComponent<MouseFollower>();
                    player.OnDeath();
                }

            }
            else
            {
                // We didn't hit anything, so the point is at the max ray length.
                hitPoint = transform.position + rayDirection * rayLength;
            }

            // 4. Add the calculated point to the spline
            // We convert the world-space hit point to the local space of the SpriteShape object.
            lightShapeController.spline.InsertPointAt(i + 1, transform.InverseTransformPoint(hitPoint));
            lightShapeController.spline.SetHeight(i + 1, 0);
        }
    }
}
