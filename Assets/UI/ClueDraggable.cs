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
        originalPosition = rectTransform.position;

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
        rectTransform.position += (Vector3)eventData.delta;
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

        Vector3 slotPos = text.transform.position;
        if (text.textInfo.characterCount > charIndex)
        {
            var charInfo = text.textInfo.characterInfo[charIndex];
            slotPos = (charInfo.bottomLeft + charInfo.topRight) / 2f;
            slotPos = text.transform.TransformPoint(slotPos);
        }

        if (Vector3.Distance(rectTransform.position, slotPos) <= snapDistance)
        {
            rectTransform.position = slotPos;
            puzzleManager.FillSlot(slotId, clueText);

            // Snap correct sound
            if (puzzleManager.snapCorrectSound != null && puzzleManager.audioSource != null)
                puzzleManager.audioSource.PlayOneShot(puzzleManager.snapCorrectSound);

            gameObject.SetActive(false);
            canvasGroup.blocksRaycasts = false;
        }
        else
        {
            ResetPosition();
            PlayIncorrectSound();
        }
    }

    private void ResetPosition()
    {
        rectTransform.position = originalPosition;
    }

    private void PlayIncorrectSound()
    {
        if (puzzleManager.snapIncorrectSound != null && puzzleManager.audioSource != null)
            puzzleManager.audioSource.PlayOneShot(puzzleManager.snapIncorrectSound);
    }
}
