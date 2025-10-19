using UnityEngine;
using UnityEngine.InputSystem;

public class MouseLock : MonoBehaviour
{
    private Camera mainCam;
    private Vector2 targetPos;

    private void Awake()
    {
        mainCam = Camera.main;
    }


    public void Start()
    {
        targetPos = this.transform.position;
        Vector2 screenPos = mainCam.WorldToScreenPoint(targetPos);
        if(Mouse.current != null)
        {
            Mouse.current.WarpCursorPosition(screenPos);
        }
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        targetPos = this.transform.position;
        Vector2 screenPos = mainCam.WorldToScreenPoint(targetPos);
        if (Mouse.current != null)
        {
            Mouse.current.WarpCursorPosition(screenPos);
        }
    }


}
