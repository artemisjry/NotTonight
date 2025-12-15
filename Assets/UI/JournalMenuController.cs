using UnityEngine;

public class JournalMenuController : MonoBehaviour
{
    [Header("UI References")]
    public RectTransform journalPanel;
    public GameObject openButton; // Open journal button

    [Header("Positions")]
    public Vector2 hiddenPosition;
    public Vector2 shownPosition;

    [Header("Animation")]
    public float lerpSpeed = 8f;

    private bool isOpen = false;
    private bool isClosing = false;

    void Start()
    {
        journalPanel.anchoredPosition = hiddenPosition;
        journalPanel.gameObject.SetActive(false);

        // Open button starts active
        if (openButton != null)
            openButton.SetActive(true);
    }

    void Update()
    {
        // Animate journal panel if active or closing
        if (journalPanel.gameObject.activeSelf || isClosing)
        {
            Vector2 target = isOpen ? shownPosition : hiddenPosition;
            journalPanel.anchoredPosition = Vector2.Lerp(
                journalPanel.anchoredPosition,
                target,
                Time.deltaTime * lerpSpeed
            );

            if (isClosing)
            {
                // Midway: unpause the game
                if (Vector2.Distance(journalPanel.anchoredPosition, hiddenPosition) <
                    Vector2.Distance(shownPosition, hiddenPosition) / 2f)
                {
                    PauseController.SetPaused(false);
                }

                // Fully closed
                if (Vector2.Distance(journalPanel.anchoredPosition, hiddenPosition) < 0.1f)
                {
                    isClosing = false;
                    journalPanel.gameObject.SetActive(false);

                    // Reactivate open button
                    if (openButton != null)
                        openButton.SetActive(true);
                }
            }
        }
    }

    // Open button clicked
    public void OpenJournal()
    {
        if (journalPanel.gameObject.activeSelf) return;

        isOpen = true;
        isClosing = false;

        journalPanel.gameObject.SetActive(true);

        // Hide open button
        if (openButton != null)
            openButton.SetActive(false);

        // Pause the game
        PauseController.SetPaused(true);
    }

    // Back button clicked
    public void CloseJournal()
    {
        isOpen = false;
        isClosing = true;

        // Open button will reactivate when fully closed
    }

    // Optional: manually toggle open button
    public void SetOpenButtonActive(bool active)
    {
        if (openButton != null)
            openButton.SetActive(active);
    }
}
