using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using Cursor = UnityEngine.Cursor;

public class Mover : MonoBehaviour
{
    public enum State
    {
        Idle,
        Move,
        Jump,
        Attack
    }

    private PlayerInput playerInput;
    private InputAction lookAction;
    private InputAction moveAction;
    private InputAction sprintAction;
    private Rigidbody rigi;
    private Animator ani;
    [SerializeField] private LayerMask obstacleLayers;
    [SerializeField] private Transform cam;
    [SerializeField] private Transform cameraArm;
    [SerializeField] private Transform model;
    private Vector3 currentVelocity;
    
    private Vector3 moveDirection;
    private Vector3 targetPosition;
    private bool onSprint;
    [SerializeField]
    private bool isFalling;
    private bool isGrounded;
    private bool wasGrounded;
    private float maxSpeedPercent;
    private float aniSpeed;
    
    [HideInInspector]
    public State curState;
    [SerializeField] protected float speed;
    [SerializeField] protected float jumpPower;
    [SerializeField, Range(0, 1)] protected float sensitivity;

    [SerializeField] private float radius = 0.21f;
    [SerializeField, Range(0, 360)] private int density = 90;
    
    private const float runSpeed = 0.75f;
    private const float sprintSpeed = 1.0f;

    public float groundCheckDistance;
    
    private void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        
        playerInput = GetComponent<PlayerInput>();
        lookAction = playerInput.currentActionMap.FindAction("Look");
        moveAction = playerInput.currentActionMap.FindAction("Move");
        sprintAction = playerInput.currentActionMap.FindAction("Sprint");
        curState = State.Idle;
        moveDirection = Vector3.zero;
        maxSpeedPercent = runSpeed;
        wasGrounded = true;
        rigi = GetComponent<Rigidbody>();
        ani = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        maxSpeedPercent = GetSpeedPercent();
        Move();
        ProcessLook();
        CheckFallAndLand();
    }

    private void ProcessLook()
    {
        Vector2 lookInput = lookAction.ReadValue<Vector2>() * sensitivity;
        Vector3 camAngle = cameraArm.rotation.eulerAngles;
        float x = camAngle.x - lookInput.y;
        x = x < 180.0f ? Mathf.Clamp(x, -1.0f, 70.0f) : Mathf.Clamp(x, 335.0f, 361.0f);
        
        cameraArm.rotation = Quaternion.Euler(x, camAngle.y + lookInput.x, camAngle.z);
        
        float targetDistance = 5.0f;
        Vector3 ori = transform.position + (transform.forward * -0.5f);
        Vector3 dir = cam.position - transform.position;
        if (Physics.SphereCast(ori, 0.25f, dir, out RaycastHit hit, 5.0f, obstacleLayers))
        {
            Debug.DrawLine(transform.position, hit.point, Color.magenta);
            targetDistance = Mathf.Clamp(hit.distance, 1.0f, 5.0f);
        }
        
        cam.localPosition = Vector3.SmoothDamp(cam.localPosition, targetDistance * Vector3.back, ref currentVelocity, 0.1f);
    }

    private void Move()
    {
        ani.SetFloat("Speed", aniSpeed);

        if (curState != State.Idle && curState != State.Move && curState != State.Jump)
            return;
        
        Vector2 inputVec2 = moveAction.ReadValue<Vector2>();
        Vector3 lookForward = new Vector3(cameraArm.forward.normalized.x, 0, cameraArm.forward.normalized.z).normalized;
        Vector3 lookRight = new Vector3(cameraArm.right.normalized.x, 0, cameraArm.right.normalized.z).normalized;
        aniSpeed = Mathf.Lerp(aniSpeed, inputVec2.sqrMagnitude * maxSpeedPercent, 0.05f);
        moveDirection = lookRight * inputVec2.x + lookForward * inputVec2.y;

        if (inputVec2 != Vector2.zero)
        {
            curState = State.Move;
        }
        else
        {
            curState = State.Idle;
            onSprint = false;
        }
        
        targetPosition = moveDirection + transform.position;
        targetPosition.y = transform.position.y;
        float distance = moveDirection.magnitude;
        if (distance < aniSpeed * speed * Time.deltaTime)
        {
            transform.position = targetPosition;
        }
        else
        {
            Vector3 direct = moveDirection.normalized;
            if (moveDirection != Vector3.zero)
                model.rotation = Quaternion.Lerp(model.rotation, Quaternion.LookRotation(direct), 10.0f * Time.deltaTime);

            Vector3 target = transform.position;
            target.y -= 0.5f;
            
            int blockCount = 0;
            for (int i = -density / 2; i <= density / 2; i++)
            {
                Vector3 checkPoint = Quaternion.AngleAxis(i, Vector3.up) * model.forward;
                checkPoint.y = 0;
                if (Physics.Raycast(target, checkPoint, radius, ~LayerMask.GetMask("Player")))
                    blockCount++;
            }

            if (blockCount <= density * 0.75f)
                transform.Translate(direct * (aniSpeed * speed * maxSpeedPercent * Time.deltaTime));
        }
    }

    private float GetSpeedPercent()
    {
        if (sprintAction.WasPressedThisFrame())
            onSprint = !onSprint;
        
        return onSprint ? sprintSpeed : runSpeed;
    }

    private bool IsGround()
    {
        RaycastHit hit;
        return Physics.SphereCast(transform.position, groundCheckDistance, Vector3.down, out hit, 1.0f, ~LayerMask.GetMask("Player"));
    }
    
    private void OnJump()
    {
        if (!IsGround() || curState == State.Jump)
            return;
        
        rigi.linearVelocity = Vector3.zero;
        rigi.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
        ani.SetTrigger("Jump");
        curState = State.Jump;
    }

    private void CheckFallAndLand()
    {
        isFalling = rigi.linearVelocity.y <= -0.1f;
        ani.SetBool("Fall", isFalling);

        isGrounded = IsGround();
        
        if (!wasGrounded && isGrounded)
        {
            ani.SetTrigger("Land");
            curState = State.Idle;
        }

        wasGrounded = isGrounded;
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        for (int i = -density / 2; i <= density / 2; i++)
        {
            Vector3 target = Quaternion.AngleAxis(i, Vector3.up) * model.forward;
            target.y = 0;
            Gizmos.DrawRay(transform.position, target * radius);
        }

        Gizmos.color = IsGround() ? Color.green : Color.red;
        Gizmos.DrawWireSphere(transform.position + Vector3.down, groundCheckDistance);
    }
}
