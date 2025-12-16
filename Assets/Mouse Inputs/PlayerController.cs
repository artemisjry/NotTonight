using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    const string IDLE = "Idle";
    const string WALK = "Walk";

    public InputSystem input;

    public NavMeshAgent agent;
    public Animator animator;

    [Header("Movement")]
    [SerializeField] ParticleSystem clickEffect;
    [SerializeField] LayerMask clickableLayers;

    public float lookRotationSpeed = 8f;

    private bool wasPausedLastFrame;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>(); 

        input = new InputSystem();
        AssignInputs();
    }

    void AssignInputs()
    {
        input.Main.Move.performed += ctx =>
        {
            if (!PauseController.IsGamePaused)
                ClickToMove();
        };
    }

    void ClickToMove()
    {
        if (PauseController.IsGamePaused)
        {
            return;
        }
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, clickableLayers))
        {
            agent.destination = hit.point;
            if (clickEffect != null)
            {
                var clickObj = Instantiate(clickEffect, hit.point += new Vector3(0, 0.1f, 0), clickEffect.transform.rotation);
            }
        }
    }

    void OnEnable()
    {
        input.Enable();
    }

    void OnDisable()
    {
        input.Disable();
    }

    void Update()
    {
        HandlePauseEdge();
        FaceTarget();
        SetAnimations();
    }

    void FaceTarget()
    {
        if (PauseController.IsGamePaused)
        {
            return;
        }
        Vector3 direction = (agent.destination - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * lookRotationSpeed);
    }

    void SetAnimations()
    {
        if(agent.velocity == Vector3.zero)
        {
            animator.Play(IDLE);
        }
        else
        {
            animator.Play(WALK);
        }
    }
    void HandlePauseEdge()
    {
        bool paused = PauseController.IsGamePaused;

        if (paused && !wasPausedLastFrame)
        {
            agent.isStopped = true;
            agent.ResetPath();
            agent.velocity = Vector3.zero;
            animator.Play(IDLE);
        }

        if (!paused && wasPausedLastFrame)
        {
            agent.isStopped = false;
        }

        wasPausedLastFrame = paused;
    }

}
