using UnityEngine;

public class InteractablesManager : MonoBehaviour
{
    [Header("Camera")]
    public Camera cam;

    [Header("Layers")]
    public LayerMask selectableLayer;

    [Header("Colors")]
    public Color hoverColor = Color.yellow;

    [Header("Audio")]
    public AudioSource audioSource;   // Reference to an AudioSource
    public AudioClip hoverSound;      // Sound to play on hover
    public AudioClip clickSound;      // Sound to play on click

    private GameObject hoveredObject;
    private Color hoveredOriginalColor;

    private GameObject selectedObject;

    void Start()
    {
        if (!cam) cam = Camera.main;
    }

    void Update()
    {
        HandleHover();

        if (Input.GetMouseButtonDown(0))
            HandleClickSelection();
    }

    // ----------------------------------------------------
    //                     HOVER LOGIC
    // ----------------------------------------------------
    void HandleHover()
    {
        if (PauseController.IsGamePaused)
            return;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, selectableLayer))
        {
            GameObject obj = hit.collider.gameObject;

            if (hoveredObject != obj)
            {
                ClearHover();

                hoveredObject = obj;
                Renderer r = hoveredObject.GetComponent<Renderer>();
                hoveredOriginalColor = r.material.color;

                r.material.color = hoverColor;

                // Play hover sound once
                if (audioSource != null && hoverSound != null)
                    audioSource.PlayOneShot(hoverSound);
            }
        }
        else
        {
            ClearHover();
        }
    }

    void ClearHover()
    {
        if (hoveredObject != null)
        {
            hoveredObject.GetComponent<Renderer>().material.color = hoveredOriginalColor;
            hoveredObject = null;
        }
    }

    // ----------------------------------------------------
    //                   CLICK SELECTION LOGIC
    // ----------------------------------------------------
    void HandleClickSelection()
    {
        if (PauseController.IsGamePaused)
            return;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, selectableLayer))
        {
            selectedObject = hit.collider.gameObject;
            Debug.Log("Selected: " + selectedObject.name);

            // Play click sound
            if (audioSource != null && clickSound != null)
                audioSource.PlayOneShot(clickSound);

            // Mark object as inspected if it has an InspectableObject
            InspectableObject inspectable = selectedObject.GetComponent<InspectableObject>();
            if (inspectable != null)
                inspectable.Inspect();

            // Trigger dialogue via central DialogueController
            Interactable interactable = selectedObject.GetComponent<Interactable>();
            if (interactable != null && interactable.dialogueData != null)
            {
                DialogueController.Instance.StartDialogue(interactable.dialogueData);
            }
        }
    }
}
