using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class JournalPuzzleManager : MonoBehaviour
{
    [Header("Passage")]
    public TMP_Text passageTextTMP;

    [Header("Slots (Auto-Generated)")]
    public List<PassageSlot> slots = new();

    [Header("End Game UI")]
    public CanvasGroup screenFadeCanvasGroup; // Full-screen black image
    public TMP_Text endGameTextTMP;           // Text in center
    public float fadeDuration = 1.5f;
    public float messageDelay = 2f;

    private bool endingTriggered = false;

    void Start()
    {
        BuildSlotsFromText();
    }

    // ----------------------------------------------------
    // AUTO SLOT BUILDER
    // ----------------------------------------------------
    void BuildSlotsFromText()
    {
        slots.Clear();

        string text = passageTextTMP.text;
        int i = 0;

        while (i < text.Length)
        {
            if (text[i] == '{')
            {
                int end = text.IndexOf('}', i);
                if (end == -1) break;

                string id = text.Substring(i + 1, end - i - 1);

                slots.Add(new PassageSlot
                {
                    slotId = id,
                    startIndex = i,
                    length = end - i + 1,
                    filled = false
                });

                i = end + 1;
            }
            else i++;
        }

        // Replace placeholders with same-length blanks
        foreach (var slot in slots)
        {
            string blanks = new string('_', slot.length);
            passageTextTMP.text = passageTextTMP.text.Replace(
                "{" + slot.slotId + "}",
                blanks
            );
        }
    }

    // ----------------------------------------------------
    // CLUE PLACEMENT
    // ----------------------------------------------------
    public bool TryPlaceClue(string slotId, string displayText)
    {
        PassageSlot slot = slots.Find(s => s.slotId == slotId && !s.filled);
        if (slot == null)
            return false;

        string text = passageTextTMP.text;

        passageTextTMP.text =
            text.Substring(0, slot.startIndex) +
            displayText +
            text.Substring(slot.startIndex + slot.length);

        int diff = displayText.Length - slot.length;
        slot.length = displayText.Length;
        slot.filled = true;

        foreach (var s in slots)
        {
            if (!s.filled && s.startIndex > slot.startIndex)
                s.startIndex += diff;
        }

        if (!endingTriggered && AllSlotsFilled())
        {
            endingTriggered = true;
            StartCoroutine(EndGameSequence());
        }

        return true;
    }

    bool AllSlotsFilled()
    {
        foreach (var s in slots)
            if (!s.filled)
                return false;
        return true;
    }

    // ----------------------------------------------------
    // END GAME SEQUENCE
    // ----------------------------------------------------
    IEnumerator EndGameSequence()
    {
        PauseController.SetPaused(true);

        screenFadeCanvasGroup.gameObject.SetActive(true);
        screenFadeCanvasGroup.alpha = 0f;

        endGameTextTMP.gameObject.SetActive(true);
        endGameTextTMP.alpha = 0f;

        // Fade to black
        yield return StartCoroutine(FadeCanvasGroup(
            screenFadeCanvasGroup, 0f, 1f, fadeDuration));

        // Show "TO BE CONTINUED"
        endGameTextTMP.text = "TO BE CONTINUED";
        yield return StartCoroutine(FadeText(
            endGameTextTMP, 0f, 1f, fadeDuration));

        yield return new WaitForSecondsRealtime(messageDelay);

        // Show click message
        endGameTextTMP.text = "CLICK ANYWHERE TO END GAME";
        endGameTextTMP.alpha = 0f;
        yield return StartCoroutine(FadeText(
            endGameTextTMP, 0f, 1f, fadeDuration));

        // Wait for click
        while (!Input.GetMouseButtonDown(0))
            yield return null;

        Application.Quit();
    }

    IEnumerator FadeCanvasGroup(CanvasGroup cg, float from, float to, float duration)
    {
        float t = 0f;
        cg.alpha = from;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            cg.alpha = Mathf.Lerp(from, to, t / duration);
            yield return null;
        }

        cg.alpha = to;
    }

    IEnumerator FadeText(TMP_Text text, float from, float to, float duration)
    {
        float t = 0f;
        text.alpha = from;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            text.alpha = Mathf.Lerp(from, to, t / duration);
            yield return null;
        }

        text.alpha = to;
    }
}
