using System.Collections;
using UnityEngine;

public class InteractablesManager : MonoBehaviour
{
    public static InteractablesManager Instance;

    [Header("Camera")]
    public Camera cam;

    [Header("Layers")]
    public LayerMask selectableLayer;

    [Header("Colors")]
    public Color hoverColor = Color.yellow;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip hoverSound;
    public AudioClip clickSound;

    [Header("Interaction Cooldown")]
    [SerializeField] private float interactionCooldown = 0.3f;

    private bool canInteract = true;
    private float cooldownTimer;

    private GameObject hoveredObject;
    private Color hoveredOriginalColor;
    private GameObject selectedObject;

    [SerializeField] PlayerController playerController;
    [SerializeField] float distanceToInteract = 3f;

    void Start()
    {
        if (!cam) cam = Camera.main;
    }

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Update()
    {
        UpdateCooldown();

        if (PauseController.IsGamePaused || !canInteract || DialogueController.Instance.IsDialogueActive)
        {
            ClearHover();
            return;
        }

        HandleHover();

        if (Input.GetMouseButtonDown(0))
            HandleClickSelection();
    }


    void UpdateCooldown()
    {
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.unscaledDeltaTime;

            if (cooldownTimer <= 0f)
                canInteract = true;
        }
    }

    public void TriggerInteractionCooldown()
    {
        canInteract = false;
        cooldownTimer = interactionCooldown;
    }

    void HandleHover()
    {
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

    void HandleClickSelection()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, selectableLayer))
        {
            selectedObject = hit.collider.gameObject;
            Debug.Log("Selected: " + selectedObject.name);

            if (audioSource != null && clickSound != null)
                audioSource.PlayOneShot(clickSound);

            InspectableObject inspectable = selectedObject.GetComponent<InspectableObject>();
            if (inspectable != null)
                inspectable.Inspect();

            Interactable interactable = selectedObject.GetComponent<Interactable>();
            playerController.agent.destination = hit.point;

            if (interactable != null && interactable.dialogueData != null)
            {
                StartCoroutine(WaitTillArrive(hit.point, interactable));
            }
        }
    }

    IEnumerator WaitTillArrive(Vector3 goal, Interactable interactable)
    {
        yield return null;

        while (Vector3.Distance(playerController.transform.position, goal) > distanceToInteract)
        {
            if (Input.GetMouseButtonDown(0))
                yield break;

            yield return null;
        }

        DialogueController.Instance.StartDialogue(interactable.dialogueData);
    }
}
