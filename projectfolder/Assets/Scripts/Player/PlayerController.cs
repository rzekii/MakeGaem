using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float jumpForce = 6.5f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float deceleration = 15f;

    [Header("Dodge Settings")]
    [SerializeField] private float dodgeSpeed = 12f;
    [SerializeField] private float dodgeDuration = 0.2f;
    [SerializeField] private float dodgeCooldown = 0.75f;

    private bool isDodging = false;
    private bool canDodge = true;

    [Header("Camera Settings")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Vector3 cameraOffset = new Vector3(0, 2, 0);
    [SerializeField] private float distanceFromPlayer = 5f;
    [SerializeField] private float mouseSensitivity = 100f;
    [SerializeField] private float smoothSpeed = 0.125f;
    [SerializeField] private float zoomSpeed = 5f;
    [SerializeField] private float minZoomDistance = 2f;
    [SerializeField] private float maxZoomDistance = 10f;
    [SerializeField] private LayerMask cameraCollisionMask;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody rb;
    private bool isJumping = false;
    private bool isInventoryOpen = false;
    private float xRotation = 0f;
    private float yRotation = 0f;
    private bool isCameraFrozen = false;
    private Vector3 frozenPosition;
    private Quaternion frozenRotation;
    private Vector3 savedVelocity;

    // Jumping Mechanics
    private float coyoteTime = 0.2f;
    private float coyoteTimeCounter;
    private float gravityMultiplier = 6f;
    private float groundCheckRadius = 0.5f;
    private float groundCheckDistance = 0.3f;
    private float groundCheckBuffer = 0.05f;

    private bool lastGroundedState = false;
    private Vector3 currentVelocity;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        rb.mass = 1f;
        rb.drag = 0f;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (!isInventoryOpen)
        {
            if (!isDodging)
            {
                HandleMovement();
                HandleJump();
            }
            HandleCameraRotation();
            HandleZoom();

            if (Input.GetKeyDown(KeyCode.LeftShift) && canDodge)
            {
                StartCoroutine(PerformDodge());
            }
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleInventory();
        }
    }

    private void LateUpdate()
    {
        if (isCameraFrozen)
        {
            cameraTransform.position = frozenPosition;
            cameraTransform.rotation = frozenRotation;
            return;
        }

        UpdateCameraPosition();
    }

    // 🎯 **Smooth Player Movement**
    private void HandleMovement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        Vector3 inputDirection = new Vector3(moveX, 0, moveZ).normalized;

        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;
        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 movement = (cameraForward * inputDirection.z + cameraRight * inputDirection.x).normalized;

        if (movement.magnitude > 0.1f)
        {
            rb.velocity = new Vector3(movement.x * moveSpeed, rb.velocity.y, movement.z * moveSpeed);

            Quaternion targetRotation = Quaternion.LookRotation(movement);

            // ✅ FIXED: Prevents uncontrolled spinning when stopping movement
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
        else
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);

            // ✅ FIXED: Stops rotation when movement stops
            rb.angularVelocity = Vector3.zero;
        }
    }



    // 🎯 **Dodge Mechanic**
    private IEnumerator PerformDodge()
    {
        canDodge = false;
        isDodging = true;

        Vector3 moveDirection = rb.velocity.normalized;
        if (moveDirection.magnitude < 0.1f)
        {
            moveDirection = transform.forward; // Default dodge forward if no movement input
        }

        rb.velocity = new Vector3(moveDirection.x * dodgeSpeed, rb.velocity.y, moveDirection.z * dodgeSpeed);

        yield return new WaitForSeconds(dodgeDuration);

        isDodging = false;
        rb.velocity = Vector3.zero; // Stop movement after dodge

        yield return new WaitForSeconds(dodgeCooldown);
        canDodge = true;
    }

    // 🎯 **Jumping**
    private void HandleJump()
    {
        bool grounded = IsGrounded();

        if (grounded && rb.velocity.y <= 0)
        {
            coyoteTimeCounter = coyoteTime;
            isJumping = false;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Space) && (coyoteTimeCounter > 0f || grounded) && !isJumping)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
            isJumping = true;
            coyoteTimeCounter = 0f;
        }

        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (gravityMultiplier - 1) * Time.deltaTime;
        }
    }

    // 🎯 **Ground Detection**
    private bool IsGrounded()
    {
        Vector3 spherePosition = transform.position + Vector3.down * (groundCheckDistance);
        return Physics.CheckSphere(spherePosition, groundCheckRadius, groundLayer);
    }

    // 🎯 **Camera Rotation**
    private void HandleCameraRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        yRotation += mouseX;
        xRotation = Mathf.Clamp(xRotation - mouseY, -80f, 80f);

        cameraTransform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
    }

    // 🎯 **Camera Zoom**
    private void HandleZoom()
    {
        distanceFromPlayer = Mathf.Clamp(
            distanceFromPlayer - Input.GetAxis("Mouse ScrollWheel") * zoomSpeed,
            minZoomDistance,
            maxZoomDistance
        );
    }

    // 🎯 **Smooth Camera Follow**
    private void UpdateCameraPosition()
    {
        Quaternion rotation = Quaternion.Euler(xRotation, yRotation, 0);
        Vector3 desiredPosition = rotation * new Vector3(0, 0, -distanceFromPlayer) + transform.position + cameraOffset;
        cameraTransform.position = Vector3.Lerp(cameraTransform.position, desiredPosition, smoothSpeed);
        cameraTransform.LookAt(transform.position + cameraOffset);
    }

    // 🎯 **Inventory Handling**
    private void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;
        isCameraFrozen = isInventoryOpen;

        if (isInventoryOpen)
        {
            frozenPosition = cameraTransform.position;
            frozenRotation = cameraTransform.rotation;
            savedVelocity = rb.velocity;
            rb.velocity = Vector3.zero;
            rb.isKinematic = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            rb.isKinematic = false;
            rb.velocity = savedVelocity;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
