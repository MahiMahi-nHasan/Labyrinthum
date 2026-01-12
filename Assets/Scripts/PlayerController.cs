
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    InputActions input;

    CharacterController _cc;
    Vector2 movementDir;
    Vector2 lookAngles;

    public float speed = 5f;
    public Transform cam;
    public Transform camAnchor;
    public float camDistance = 6.5f;
    public float camClipDistance = 0.5f;

    public float yLookMin = -50f;
    public float yLookMax = 80f;

    public float pickupDistance = 3f;
    public LayerMask pickupLayer;
    public AudioClip[] footstepSounds;
    public float minTimeBetweenFootsteps = 0.3f; 
    public float maxTimeBetweenFootsteps = 0.6f;
    private float timeSinceLastFootstep;

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        input = new();
        _cc = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        input.Player.Pickup.performed += OnPickup;
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
        UpdateInput();

        Move();
        Look();
        if (Input.GetKey(KeyCode.W) ||Input.GetKey(KeyCode.A)||Input.GetKey(KeyCode.S)||Input.GetKey(KeyCode.D))
        {
            if (Time.time - timeSinceLastFootstep >= 0.7)
            {
                // Play a random footstep sound from the array
                int rand = Random.Range(0,4);
                AudioClip footstepSound = footstepSounds[rand];
                audioSource.PlayOneShot(footstepSound);

                timeSinceLastFootstep = Time.time; // Update the time since the last footstep sound
            }
        }
    }
    

    void UpdateInput()
    {
        // Update global variables with new input
        movementDir = input.Player.Move.ReadValue<Vector2>();

        Vector2 lookDelta = input.Player.Look.ReadValue<Vector2>();
        lookAngles += lookDelta * new Vector2(1, -1);
        lookAngles.y = Mathf.Clamp(lookAngles.y, yLookMin, yLookMax);
    }

    void Move()
    {
        Vector3 movement = new(
            movementDir.x,
            0,
            movementDir.y
        );
        movement *= speed * Time.deltaTime;
        movement = camAnchor.rotation * movement;

        _cc.Move(movement);
    }

    void Look()
    {
        float yDist = Mathf.Sin(Mathf.Deg2Rad * lookAngles.y);
        float zDist = Mathf.Cos(Mathf.Deg2Rad * lookAngles.y);

        Vector3 cameraPosition = new(
            0,
            yDist,
            -zDist
        );
        if (Physics.Raycast(camAnchor.position, -cam.forward, out RaycastHit hit, camDistance + camClipDistance))
            cameraPosition *= hit.distance - camClipDistance;
        else
            cameraPosition *= camDistance;

        cam.localPosition = cameraPosition;

        camAnchor.rotation = Quaternion.Euler(0, lookAngles.x, 0);
        cam.localRotation = Quaternion.Euler(lookAngles.y, 0, 0);
    }

    // Detects whether the player is looking at a pickup
    bool TestForInteractable(out RaycastHit hit) => Physics.Raycast(camAnchor.position, cam.transform.forward, out hit, pickupDistance, pickupLayer);

    void OnPickup(InputAction.CallbackContext context)
    {
        if (TestForInteractable(out RaycastHit hit))
        {
            // Interacts with the interactable
            hit.collider.gameObject.GetComponent<Interactable>().Interact();
        }
    }
}
