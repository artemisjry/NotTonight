using System.Collections;
using TMPro;
using UnityEngine;

public class DialogueController : MonoBehaviour
{
    public static DialogueController Instance;

    [Header("UI")]
    public GameObject dialoguePanel;
    public TMP_Text dialogueText;
    public GameObject openJournalButton; // Reference to the open journal button

    [Header("Typing Settings")]
    public float typingSpeed = 0.05f;
    public AudioClip typingSound;
    public AudioSource audioSource;

    private string[] currentLines;
    private int dialogueIndex;
    private bool isTyping;

    public bool IsDialogueActive { get; private set; }

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void StartDialogue(InspectDialogue dialogue)
    {
        if (IsDialogueActive)
            return;

        IsDialogueActive = true;
        PauseController.SetPaused(true);

        if (openJournalButton != null)
            openJournalButton.SetActive(false);

        currentLines = dialogue.dialogueLines;
        dialogueIndex = 0;

        dialoguePanel.SetActive(true);
        StartCoroutine(TypeLine());
    }

    public void NextLine()
    {
        if (isTyping)
        {
            StopAllCoroutines();
            dialogueText.SetText(currentLines[dialogueIndex]);
            isTyping = false;
            return;
        }

        dialogueIndex++;

        if (dialogueIndex < currentLines.Length)
        {
            StartCoroutine(TypeLine());
        }
        else
        {
            EndDialogue();
        }
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        dialogueText.SetText("");

        foreach (char letter in currentLines[dialogueIndex])
        {
            dialogueText.text += letter;

            if (typingSound != null && audioSource != null)
                audioSource.PlayOneShot(typingSound);

            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    void EndDialogue()
    {
        StopAllCoroutines();
        IsDialogueActive = false;

        dialogueText.SetText("");
        dialoguePanel.SetActive(false);

        PauseController.SetPaused(false);

        // Reactivate open journal button when dialogue ends
        if (openJournalButton != null)
            openJournalButton.SetActive(true);
    }

    void Update()
    {
        if (!IsDialogueActive)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            NextLine();
        }
    }
}
