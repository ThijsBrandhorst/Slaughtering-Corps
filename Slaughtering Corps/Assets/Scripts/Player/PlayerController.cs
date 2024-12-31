using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(AudioSource))]
public class PlayerController : MonoBehaviour {
    // --------------------
    //     References
    // --------------------
    [Header("References")]
    public Camera playerCamera;
    private CharacterController controller;
    private AudioSource audioSource;

    private PlayerControls playerControls;

    // --------------------
    //   Movement Settings
    // --------------------
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 8f;
    public float crouchSpeed = 2.5f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;

    [Tooltip("CharacterController height when crouching.")]
    public float crouchHeight = 1f;
    private float defaultHeight; 

    // --------------------
    //     Look Settings
    // --------------------
    [Header("Look Settings")]
    public float lookSensitivity = 1f;
    public float verticalLookLimit = 80f;

    private float cameraPitch = 0f;  
    private Vector2 moveInput;     
    private Vector2 lookInput;    
    private bool isSprinting = false;
    private bool isCrouching = false;
    private float verticalVelocity = 0f; 
    private float moveSpeed;            

    // --------------------
    //     Audio Clips
    // --------------------
    [Header("Audio Clips")]
    public AudioClip jumpSound;
    public AudioClip landSound;

    // --------------------
    //   Camera Bobbing
    // --------------------
    [Header("Camera Bobbing")]
    public float walkBobSpeed = 10f;
    public float walkBobAmount = 0.05f;
    public float sprintBobSpeed = 14f;
    public float sprintBobAmount = 0.08f;
    public float crouchBobSpeed = 7f;
    public float crouchBobAmount = 0.03f;

    private float defaultCamPosY;
    private float bobTimer = 0f;

    // =====================================================================
    //                          Unity Methods
    // =====================================================================
    private void Awake() {
        controller = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();
        if (!playerCamera) {
            Debug.LogWarning("Player Camera is not assigned in the inspector!");
        }

        defaultHeight = controller.height;

        playerControls = new PlayerControls();

        playerControls.Player.Move.performed += ctx => OnMove(ctx);
        playerControls.Player.Move.canceled += ctx => OnMove(ctx);
        playerControls.Player.Look.performed += ctx => OnLook(ctx);
        playerControls.Player.Look.canceled += ctx => OnLook(ctx);

        playerControls.Player.Jump.performed += ctx => OnJump(ctx);
        playerControls.Player.Crouch.performed += ctx => OnCrouch(ctx);
        playerControls.Player.Sprint.performed += ctx => OnSprint(ctx);
    }

    private void OnEnable() {
        if (playerControls != null)
            playerControls.Enable();
    }

    private void OnDisable() {
        if (playerControls != null)
            playerControls.Disable();
    }

    private void Start() {
        if (playerCamera)
            defaultCamPosY = playerCamera.transform.localPosition.y;
    }

    private void Update() {
        HandleLook();
        HandleMovement();
        HandleCameraBobbing();
    }

    // =====================================================================
    //                      Input Action Callbacks
    // =====================================================================
    private void OnMove(InputAction.CallbackContext context) {
        moveInput = context.ReadValue<Vector2>();
    }

    private void OnLook(InputAction.CallbackContext context) {
        lookInput = context.ReadValue<Vector2>();
    }

    private void OnJump(InputAction.CallbackContext context) {
        if (context.performed && controller.isGrounded) {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            PlaySound(jumpSound);
        }
    }

    private void OnSprint(InputAction.CallbackContext context) {
        if (context.performed) {
            isSprinting = !isSprinting;
        }
    }

    private void OnCrouch(InputAction.CallbackContext context) {
        // Toggle Crouching
        if (context.performed) {
            isCrouching = !isCrouching;
            controller.height = isCrouching ? crouchHeight : defaultHeight;
        }
    }

    // =====================================================================
    //                      Movement & Camera Methods
    // =====================================================================
    private void HandleLook() {
        if (Time.timeScale == 0) return;

        transform.Rotate(Vector3.up * (lookInput.x * lookSensitivity));

        cameraPitch -= (lookInput.y * lookSensitivity);
        cameraPitch = Mathf.Clamp(cameraPitch, -verticalLookLimit, verticalLookLimit);

        if (playerCamera) {
            playerCamera.transform.localEulerAngles = new Vector3(cameraPitch, 0f, 0f);
        }
    }

    private void HandleMovement() {
        if (isCrouching) {
            isSprinting = false;
            moveSpeed = crouchSpeed;
        } else if (isSprinting) {
            isCrouching = false;
            moveSpeed = sprintSpeed;
        } else {
            moveSpeed = walkSpeed;
        }

        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        move *= moveSpeed;

        if (controller.isGrounded && verticalVelocity < 0f) {
            verticalVelocity = -2f;
        }
        verticalVelocity += gravity * Time.deltaTime;
        move.y = verticalVelocity;

        controller.Move(move * Time.deltaTime);
    }

    private void HandleCameraBobbing() {
        if (moveInput.magnitude < 0.1f || !controller.isGrounded) {
            bobTimer = 0f;
            Vector3 camPos = playerCamera.transform.localPosition;
            camPos.y = Mathf.Lerp(camPos.y, defaultCamPosY, Time.deltaTime * 5f);
            playerCamera.transform.localPosition = camPos;
            return;
        }

        float currentBobSpeed;
        float currentBobAmount;

        if (isSprinting) {
            currentBobSpeed = sprintBobSpeed;
            currentBobAmount = sprintBobAmount;
        } else if (isCrouching) {
            currentBobSpeed = crouchBobSpeed;
            currentBobAmount = crouchBobAmount;
        } else {
            currentBobSpeed = walkBobSpeed;
            currentBobAmount = walkBobAmount;
        }

        bobTimer += Time.deltaTime * currentBobSpeed;

        float bobOffset = Mathf.Sin(bobTimer) * currentBobAmount;

        Vector3 newCamPos = playerCamera.transform.localPosition;
        newCamPos.y = defaultCamPosY + bobOffset;
        playerCamera.transform.localPosition = newCamPos;
    }

    // =====================================================================
    //                           Utility Methods
    // =====================================================================
    private void PlaySound(AudioClip clip) {
        if (clip && audioSource) {
            audioSource.PlayOneShot(clip);
        }
    }
}
