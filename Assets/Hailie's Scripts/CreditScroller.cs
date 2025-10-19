using UnityEngine;
using TMPro; // Required for using TextMeshPro components

public class CreditScroller : MonoBehaviour
{
    [Header("UI Setup")]
    [Tooltip("Drag the TextMeshPro UGUI component here.")]
    public TextMeshProUGUI creditsText;

    [Header("Content and Speed")]
    [Tooltip("Enter all the names/credits you want to display, one per line.")]
    [TextArea(5, 10)] // Makes the string field a large, multiline text area in the Inspector
    public string creditsList = "Director:\nName 1\n\nArtist:\nName 2\n\nDeveloper:\nName 3";

    [Tooltip("The speed at which the credits scroll up (e.g., 20 to 50 is a good range).")]
    public float scrollSpeed = 30f;

    [Tooltip("The time (in seconds) the credits pause once they've finished scrolling.")]
    public float endPauseDuration = 5f;

    private RectTransform rectTransform;
    private float totalHeight;
    private float timer;
    private bool scrollingFinished = false;

    void Start()
    {
        // Check for required components
        if (creditsText == null)
        {
            Debug.LogError("CreditsText is not assigned. Please drag a TextMeshProUGUI component to the slot in the Inspector.");
            enabled = false; // Disable script if setup is incomplete
            return;
        }

        // Assign the input text to the TextMeshPro component
        creditsText.text = creditsList;

        // Get the RectTransform for movement
        rectTransform = creditsText.GetComponent<RectTransform>();

        // Wait one frame to let TextMeshPro calculate the text layout and bounds
        StartCoroutine(InitializeScroll());
    }

    System.Collections.IEnumerator InitializeScroll()
    {
        yield return null;

        // Use the height of the bounding box to know how far to scroll
        totalHeight = creditsText.preferredHeight;

        // Start the credits just below the bottom of the screen (assuming a vertical layout)
        rectTransform.anchoredPosition = new Vector2(
            rectTransform.anchoredPosition.x,
            -rectTransform.rect.height // Start off-screen below the viewport
        );
    }

    void Update()
    {
        if (scrollingFinished)
        {
            // After scrolling is done, start the pause timer
            timer += Time.deltaTime;
            if (timer >= endPauseDuration)
            {
                // Example action: load back to main menu or quit the game
                Debug.Log("Credits finished. Loading Main Menu or Quitting...");
                // SceneManager.LoadScene("MainMenu"); 
                enabled = false; // Stop updating
            }
            return;
        }

        // The vertical distance scrolled in this frame: speed * time
        float scrollDistance = scrollSpeed * Time.deltaTime;

        // Move the credits up
        rectTransform.anchoredPosition += new Vector2(0, scrollDistance);

        // Check if the bottom of the credits block has scrolled past the top of the text's original position
        if (rectTransform.anchoredPosition.y >= totalHeight)
        {
            scrollingFinished = true;
            timer = 0; // Reset timer for the end pause
            Debug.Log("Credits scroll complete.");
        }
    }
}
