using UnityEngine;
using TMPro;

public class ClueUnlockable : MonoBehaviour
{
    [Header("Linked Object")]
    public GameObject linkedObject;  
    public bool unlocked = false;

    private TMP_Text clueText;

    private void Awake()
    {
        clueText = GetComponent<TMP_Text>();

        if (clueText != null)
            clueText.enabled = false; 

        var cg = GetComponent<CanvasGroup>();
        if (cg != null)
            cg.alpha = 0;
    }

    private void Update()
    {
        if (!unlocked && linkedObject != null)
        {
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

        if (clueText != null)
            clueText.enabled = true;

        var cg = GetComponent<CanvasGroup>();
        if (cg != null)
            cg.alpha = 1;

        var draggable = GetComponent<ClueDraggable>();
        if (draggable != null)
            draggable.enabled = true;
    }
}
