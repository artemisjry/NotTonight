using UnityEngine;

public class Interactable : MonoBehaviour
{
    public InspectDialogue dialogueData;

    public void StartDialogue()
    {
        DialogueController.Instance.StartDialogue(dialogueData);
    }
}
