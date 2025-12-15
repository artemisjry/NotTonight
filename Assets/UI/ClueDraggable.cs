using UnityEngine;
using UnityEngine.EventSystems;

public class ClueDraggable : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Clue Data")]
    public string slotId;        // MUST match a PassageSlot.slotId
    public string displayText;   // Text inserted into the passage

    private Vector3 startPos;
    private Canvas canvas;
    private JournalPuzzleManager puzzle;

    void Start()
    {
        startPos = transform.position;
        canvas = GetComponentInParent<Canvas>();
        puzzle = FindFirstObjectByType<JournalPuzzleManager>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        startPos = transform.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position += (Vector3)eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!puzzle.TryPlaceClue(slotId, displayText))
        {
            // ❌ Wrong slot → snap back
            transform.position = startPos;
        }
        else
        {
            // ✅ Correct slot → disappear
            gameObject.SetActive(false);
        }
    }
}
