using UnityEngine;
using TMPro;
using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public class JournalPuzzleManager : MonoBehaviour
{
    [Header("Passage")]
    [TextArea(4, 10)]
    public string passageText;      // Text containing {slots}
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
        {
            Debug.LogError("Passage TMP_Text is missing.");
            return;
        }

        List<PassageSlotData> slotList = new List<PassageSlotData>();
        string workingText = passageText;

        int offset = 0;

        Regex regex = new Regex(@"\{(.*?)\}");
        MatchCollection matches = regex.Matches(passageText);

        foreach (Match match in matches)
        {
            string slotText = match.Groups[1].Value; // text inside {}
            int originalLength = match.Length;

            int startIndex = match.Index + offset;
            int underlineLength = slotText.Length;

            // Create slot
            PassageSlotData slot = new PassageSlotData
            {
                slotId = slotText,          // 🔑 slotId = text inside {}
                startIndex = startIndex,
                length = underlineLength,
                filled = false
            };
            slotList.Add(slot);

            // Replace {text} with _____
            string underline = new string(underlineCharacter, underlineLength);
            workingText = workingText.Remove(startIndex, originalLength)
                                     .Insert(startIndex, underline);

            offset += underlineLength - originalLength;
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
        string newText =
            oldText.Substring(0, slot.startIndex) +
            displayText +
            oldText.Substring(slot.startIndex + slot.length);

        passageTextTMP.text = newText;
        slot.filled = true;

        int diff = displayText.Length - slot.length;
        foreach (var s in slotsData)
        {
            if (!s.filled && s.startIndex > slot.startIndex)
                s.startIndex += diff;
        }

        if (Array.TrueForAll(slotsData, s => s.filled))
        {
            Debug.Log("Puzzle Solved!");
        }
    }
}
