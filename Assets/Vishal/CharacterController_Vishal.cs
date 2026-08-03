using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


public class CharacterController_Vishal : MonoBehaviour
{
    private enum PlayerState
    {
        idle,
        walk,
        sprint,
        jump
    }
    private enum PlayerTeam
    {
        Attacker,
        Builder
    }
    [Header("Input System references")]
    [SerializeField] InputActionReference move;
    [SerializeField] InputActionReference sprint;
    [SerializeField] InputActionReference jump;
    [SerializeField] InputActionReference shrink;

    [Header("Player Movement")]
    [SerializeField] float walkSpeed,runSpeed,rotationSpeed,jumpForce;
    [SerializeField] float groundChkStart, groundChkDistance;
    bool canMove = true;
    float isSprint;
    float jumpPressed;
    bool hasInput;
    bool isGrounded;
    bool isShrinked;
    Vector2 moveDirection;

    [Header("Camera")]
    [SerializeField] CinemachineCamera camera;
    Transform cameraTransform;

    [Header("Player References")]
    Animator animator;
    Rigidbody rb;

    [Header("States and team")]
    PlayerState currentState;
    PlayerTeam currentTeam;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        cameraTransform=camera.transform;
    }
    private void OnEnable()
    {
        jump.action.started += Jump;
        shrink.action.started += Shrink;
    }
    void Update()
    {
        if (hasInput)
        {
            if (isSprint > 0)
            {
                ChangeState(PlayerState.sprint);
            }
            if (isSprint == 0)
            {
                ChangeState(PlayerState.walk);
            }
        }
        else
        {
            ChangeState(PlayerState.idle);
        }
        jumpPressed = jump.action.ReadValue<float>();
        moveDirection =move.action.ReadValue<Vector2>();
        isSprint=sprint.action.ReadValue<float>();
        hasInput = moveDirection.magnitude > 0;
        HandleRotation();

    }
    private void FixedUpdate()
    {
        HandleStates();
    }

    void Move(float speed)
    {
        if(hasInput)
            rb.MovePosition(rb.position+transform.forward * speed * Time.deltaTime);
    }

    void HandleRotation()
    {
        if (hasInput)
        {

            Vector3 direc = new Vector3(moveDirection.x, 0 , moveDirection.y);
            Vector3 cam = new Vector3(cameraTransform.forward.x, 0, cameraTransform.forward.z);
            direc = Quaternion.LookRotation(cam) * direc;
            Quaternion desiredRotation = Quaternion.LookRotation(direc);
            transform.rotation = Quaternion.RotateTowards(transform.rotation,desiredRotation,Time.deltaTime*rotationSpeed);
        }
    }

    void Idle()
    {

    }
    void Walk()
    {
        Move(walkSpeed);
    }
    void Sprint()
    {
        Move(runSpeed);
    }
    void Jump(InputAction.CallbackContext obj)
    {
        if (Physics.Raycast(transform.position + Vector3.down * groundChkStart, Vector3.down, groundChkDistance))
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        
    }
    void Shrink(InputAction.CallbackContext obj)
    {
        if (!isShrinked)
            transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        else
            transform.localScale = Vector3.one;
        isShrinked = !isShrinked;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawLine(transform.position + Vector3.down * groundChkStart, transform.position + Vector3.down * groundChkStart + Vector3.down * groundChkDistance);
    }
    void HandleStates()
    {
        switch (currentState)
        {
            case PlayerState.idle:
                Idle();
                break;
            case PlayerState.walk:
                Walk();
                break;
            case PlayerState.sprint:
                Sprint();
                break;
        }
    }

    void ExitState(PlayerState state)
    {
        switch (state)
        {
            case PlayerState.idle:
                animator.SetBool("IsIdle", false);
                break;
            case PlayerState.walk:
                animator.SetBool("IsWalking", false);
                break;
            case PlayerState.sprint:
                animator.SetBool("IsRunning", false);
                break;
            default:
                break;
        }
    }
    void EnterState(PlayerState state)
    {
        switch (state)
        {
            case PlayerState.idle:
                animator.SetBool("IsIdle", true);
                break;
            case PlayerState.walk:
                animator.SetBool("IsWalking", true);
                break;
            case PlayerState.sprint:
                animator.SetBool("IsRunning", true);
                break;
            default:
                break;
        }
    }

    void ChangeState(PlayerState state)
    {
        if (currentState == state)
            return;
        ExitState(currentState);
        currentState = state;
        EnterState(currentState);
    }

    private void OnDisable()
    {
        jump.action.started -= Jump;
        shrink.action.started -= Shrink;
    }

}
