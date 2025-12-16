using System;

[System.Serializable]
public class PassageSlotData
{
    public string slotId;      // Unique slot ID
    public int startIndex;     // Start index in the text
    public int length;         // Length of placeholder
    public bool filled;        // Whether the slot has been filled
}
