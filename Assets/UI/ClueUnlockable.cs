using UnityEngine;
using TMPro;

public class ClueUnlockable : MonoBehaviour
{
    [Header("Linked Object")]
    public GameObject linkedObject;  // The object player must inspect
    public bool unlocked = false;

    private TMP_Text clueText;

    private void Awake()
    {
        clueText = GetComponent<TMP_Text>();

        if (clueText != null)
            clueText.enabled = false; // Hide initially

        // Optional: hide CanvasGroup if draggable
        var cg = GetComponent<CanvasGroup>();
        if (cg != null)
            cg.alpha = 0;
    }

    private void Update()
    {
        if (!unlocked && linkedObject != null)
        {
            // Check if the linked object has been inspected
            var inspectable = linkedObject.GetComponent<InspectableObject>();
            if (inspectable != null && inspectable.inspected)
            {
                UnlockClue();
            }
        }
    }

    private void UnlockClue()
    {
        unlocked = true;

        // Show the clue
        if (clueText != null)
            clueText.enabled = true;

        var cg = GetComponent<CanvasGroup>();
        if (cg != null)
            cg.alpha = 1;

        // Enable drag if using ClueDraggable
        var draggable = GetComponent<ClueDraggable>();
        if (draggable != null)
            draggable.enabled = true;
    }
}
