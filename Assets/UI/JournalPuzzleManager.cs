using UnityEngine;
using TMPro;
using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Collections; // ✅ REQUIRED for IEnumerator

public class JournalPuzzleManager : MonoBehaviour
{
    [Header("Passage")]
    [TextArea(4, 10)]
    public string passageText;
    public TMP_Text passageTextTMP;

    [Header("Slots")]
    public PassageSlotData[] slotsData;

    [Header("Clue Sounds")]
    public AudioClip pickUpSound;
    public AudioClip snapCorrectSound;
    public AudioClip snapIncorrectSound;
    public AudioSource audioSource;

    [Header("Slot Display")]
    public char underlineCharacter = '_';

    // ---------------- NEW ----------------
    [Header("End Game")]
    public CanvasGroup fadeCanvas;      // Fullscreen black panel
    public TMP_Text endText;             // Center text
    public AudioClip puzzleSolvedSound;
    public float fadeSpeed = 1.2f;
    public float textDelay = 2f;
    // ------------------------------------

    void Awake()
    {
        GenerateSlotsFromPassage();
    }

    // --------------------------------------------------
    // SLOT GENERATION
    // --------------------------------------------------
    private void GenerateSlotsFromPassage()
    {
        if (passageTextTMP == null)
            return;

        List<PassageSlotData> slotList = new List<PassageSlotData>();
        string workingText = passageText;

        int offset = 0;
        Regex regex = new Regex(@"\{(.*?)\}");
        MatchCollection matches = regex.Matches(passageText);

        foreach (Match match in matches)
        {
            string slotText = match.Groups[1].Value;
            int startIndex = match.Index + offset;

            PassageSlotData slot = new PassageSlotData
            {
                slotId = slotText,
                startIndex = startIndex,
                length = slotText.Length,
                filled = false
            };

            slotList.Add(slot);

            string underline = new string(underlineCharacter, slotText.Length);
            workingText = workingText.Remove(startIndex, match.Length)
                                     .Insert(startIndex, underline);

            offset += underline.Length - match.Length;
        }

        slotsData = slotList.ToArray();
        passageTextTMP.text = workingText;
    }

    // --------------------------------------------------
    // SLOT FILLING
    // --------------------------------------------------
    public void FillSlot(string slotId, string displayText)
    {
        PassageSlotData slot = Array.Find(slotsData, s => s.slotId == slotId);
        if (slot == null || slot.filled)
            return;

        string oldText = passageTextTMP.text;
        passageTextTMP.text =
            oldText.Substring(0, slot.startIndex) +
            displayText +
            oldText.Substring(slot.startIndex + slot.length);

        slot.filled = true;

        int diff = displayText.Length - slot.length;
        foreach (var s in slotsData)
        {
            if (!s.filled && s.startIndex > slot.startIndex)
                s.startIndex += diff;
        }

        // 🔥 CHANGED PART 🔥
        if (Array.TrueForAll(slotsData, s => s.filled))
        {
            StartCoroutine(EndGameSequence());
        }
    }

    // --------------------------------------------------
    // END GAME SEQUENCE
    // --------------------------------------------------
    IEnumerator EndGameSequence()
    {
        PauseController.SetPaused(true);

        if (audioSource && puzzleSolvedSound)
            audioSource.PlayOneShot(puzzleSolvedSound);

        fadeCanvas.gameObject.SetActive(true);
        fadeCanvas.alpha = 0f;

        // Fade to black
        while (fadeCanvas.alpha < 1f)
        {
            fadeCanvas.alpha += Time.unscaledDeltaTime * fadeSpeed;
            yield return null;
        }

        endText.gameObject.SetActive(true);
        endText.text = "TO BE CONTINUED";
        endText.alpha = 1f;

        yield return new WaitForSecondsRealtime(textDelay);

        // Fade text out
        while (endText.alpha > 0f)
        {
            endText.alpha -= Time.unscaledDeltaTime * fadeSpeed;
            yield return null;
        }

        // Fade text in with new message
        endText.text = "Click Anywhere to Close";
        while (endText.alpha < 1f)
        {
            endText.alpha += Time.unscaledDeltaTime * fadeSpeed;
            yield return null;
        }

        // Wait for click
        while (!Input.GetMouseButtonDown(0))
            yield return null;

        Application.Quit();
    }
}
