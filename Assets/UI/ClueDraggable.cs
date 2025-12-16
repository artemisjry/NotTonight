using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(CanvasGroup))]
public class ClueDraggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Clue Settings")]
    public string slotId;      // Must match the slot in JournalPuzzleManager
    public string clueText;    // Text to fill in the slot
    public float snapDistance = 50f;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector3 originalPosition;

    private JournalPuzzleManager puzzleManager;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        originalPosition = rectTransform.anchoredPosition;

        puzzleManager = FindFirstObjectByType<JournalPuzzleManager>();
        if (puzzleManager == null)
            Debug.LogError("JournalPuzzleManager not found in the scene!");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 0.7f;
        canvasGroup.blocksRaycasts = false;

        // Play pick-up sound from manager
        if (puzzleManager.pickUpSound != null && puzzleManager.audioSource != null)
            puzzleManager.audioSource.PlayOneShot(puzzleManager.pickUpSound);
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / puzzleManager.passageTextTMP.canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        TrySnapToSlot();
    }

    private void TrySnapToSlot()
    {
        if (puzzleManager == null || puzzleManager.slotsData == null)
        {
            ResetPosition();
            PlayIncorrectSound();
            return;
        }

        var slot = Array.Find(puzzleManager.slotsData, s => s.slotId == slotId);
        if (slot == null || slot.filled)
        {
            ResetPosition();
            PlayIncorrectSound();
            return;
        }

        TMP_Text text = puzzleManager.passageTextTMP;
        int charIndex = slot.startIndex;

        if (charIndex >= text.textInfo.characterCount)
        {
            ResetPosition();
            PlayIncorrectSound();
            return;
        }

        // --- FIX: Correct snapping in UI space ---
        var charInfo = text.textInfo.characterInfo[charIndex];
        Vector3 worldSlotPos = text.transform.TransformPoint((charInfo.bottomLeft + charInfo.topRight) / 2f);
        Vector2 screenSlotPos = RectTransformUtility.WorldToScreenPoint(null, worldSlotPos);
        RectTransform parentRect = rectTransform.parent as RectTransform;
        Vector2 localSlotPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, screenSlotPos, null, out localSlotPos);

        if (Vector2.Distance(rectTransform.anchoredPosition, localSlotPos) <= snapDistance)
        {
            rectTransform.anchoredPosition = localSlotPos;
            puzzleManager.FillSlot(slotId, clueText);

            if (puzzleManager.snapCorrectSound != null && puzzleManager.audioSource != null)
                puzzleManager.audioSource.PlayOneShot(puzzleManager.snapCorrectSound);

            gameObject.SetActive(false);
        }
        else
        {
            ResetPosition();
            PlayIncorrectSound();
        }
    }

    private void ResetPosition()
    {
        rectTransform.anchoredPosition = originalPosition;
    }

    private void PlayIncorrectSound()
    {
        if (puzzleManager.snapIncorrectSound != null && puzzleManager.audioSource != null)
            puzzleManager.audioSource.PlayOneShot(puzzleManager.snapIncorrectSound);
    }
}
