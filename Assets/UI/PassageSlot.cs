using System;

[Serializable]
public class PassageSlot
{
    public string slotId;     // Unique identifier
    public int startIndex;    // Index in TMP text
    public int length;        // Current length of the slot
    public bool filled;       // Has this slot been solved?
}
