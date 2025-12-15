using UnityEngine;

[CreateAssetMenu(fileName = "NewInspectDialogue", menuName = "Inspect Dialogue")]
public class InspectDialogue : ScriptableObject
{
    [TextArea(4, 8)]
    public string[] dialogueLines;
}
