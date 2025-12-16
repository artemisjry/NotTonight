using UnityEngine;

public class JournalMenuController : MonoBehaviour
{
    [Header("UI References")]
    public RectTransform journalPanel;
    public GameObject openButton;

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

        if (openButton != null)
            openButton.SetActive(true);
    }

    void Update()
    {
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
                if (Vector2.Distance(journalPanel.anchoredPosition, hiddenPosition) <
                    Vector2.Distance(shownPosition, hiddenPosition) / 2f)
                {
                    PauseController.SetPaused(false);
                    if (openButton != null)
                        openButton.SetActive(true);
                }

                if (Vector2.Distance(journalPanel.anchoredPosition, hiddenPosition) < 0.1f)
                {
                    isClosing = false;
                    journalPanel.gameObject.SetActive(false);

                    
                }
            }
        }
    }

    public void OpenJournal()
    {
        if (journalPanel.gameObject.activeSelf) return;

        isOpen = true;
        isClosing = false;

        journalPanel.gameObject.SetActive(true);

        if (openButton != null)
            openButton.SetActive(false);

        PauseController.SetPaused(true);
    }

    public void CloseJournal()
    {
        isOpen = false;
        isClosing = true;

    }

    public void SetOpenButtonActive(bool active)
    {
        if (openButton != null)
            openButton.SetActive(active);
    }
}
