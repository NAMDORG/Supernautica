using System;
using UnityEngine;

public enum MovementType { position, velocity, clinging };

public class PlayerControls : MonoBehaviour
{
    // Show fields in the unity inspector for tweaking and debugging
    [SerializeField] float mouseSensitivity = 2f;
    [SerializeField] float movementSpeed = 1f;
    [SerializeField] float jumpStrength = 0.0f;
    [SerializeField] Vector3 bodyVelocity = Vector3.zero;
    [SerializeField] float cameraFOV;

    public MovementType WASDType;
    public bool objectIsClingable, movingToCling, playerIsClinging, playerIsJumping, flashlightIsOn;

    //private Vector2 mouseMovementSum;
    private Vector3 WASD, firstPoint, nextPoint, movementDirection, closestNormal;
    private Rigidbody player;
    private Light flashlight;
    private RaycastHit bodyHit;
    public static GameObject nearestClingable;

    private void Start()
    {
        // Make cursor invisible on game start
        Cursor.visible = false;

        // Declare game objects to be referenced below
        player = GetComponent<Rigidbody>();
        flashlight = GetComponentInChildren<Light>();
        cameraFOV = 60.0f;

        // Player starts with the velocity movement type
        WASDType = MovementType.velocity;
    }

    private void Update()
    {
        // Process mouse controls
        MouseControls();

        // Process WASD Controls
        MovementControls();

        // Process other key commands
        OtherKeyControls();

        // TODO: Make conditional?
        AdjustCamerFOV();
    }

    //==========================================/
    // Inputs
    //==========================================/

    // Process all the functions controlled by mouse buttons and movement
    private void MouseControls()
    {
        // Convert mouse movement into mouselook
        Mouselook();
        MouseZero();
    }

    // Convert mouse movement in two dimensions into mouselook
    private void Mouselook()
    {
        Vector2 mouseMovementSum;

        // How much as the mouse moved in X and Y
        Vector2 mouseXY = new Vector2
            (Input.GetAxisRaw("Mouse X"),
                Input.GetAxisRaw("Mouse Y"));

        // Add new mouse movement to the variable holding camera movement (with mouse sensitivity multiplier)
        mouseMovementSum = mouseXY * mouseSensitivity;

        // Move object with the mouse - camera is a child of this object so it moves as well
        transform.Rotate(-mouseMovementSum.y, mouseMovementSum.x, 0f, Space.Self);
    }

    // Process clicking the left mouse button (Grab object)
    private void MouseZero()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            GetComponentInChildren<PlayerCamera>().GrabObject();
        }
    }

    // Turn mouse movement into a Vector2, then decide which Movement Type to process
    private void MovementControls()
    {
        // Pressing a WASD key applies a vector in that direction on a 2d plane
        WASD = new Vector3
            (Input.GetAxisRaw("Horizontal"),
                Input.GetAxisRaw("Vertical"),
                    0f);

        // Select type of movement depending on game state
        switch (WASDType)
        {
            case MovementType.position:
                WASDPosition();
                break;
            case MovementType.velocity:
                WASDVelocity();
                break;
            case MovementType.clinging:
                WASDClinging();
                break;
        }
    }

    // Call other keypress functions
    private void OtherKeyControls()
    {
        Cling();
        Roll();
        Jump();
        Flashlight();
        PauseGame();
    }

    //=========================================/
    // Movement Functions
    //=========================================/

    // Movement Type
    // WASD moves player position forward, backward, left, right
    private void WASDPosition()
    {
        // TODO: Gravity movement
        // This is the starting point for an eventual feature of artificial gravity. Will need adjusted mouselook too.
        this.transform.Translate(WASD.x, 0f, WASD.y);
    }

    // Movement Type
    // WASD applies a force that creates a velocity forward, backward, left, right
    private void WASDVelocity()
    {
        WASD *= movementSpeed;

        //WASD creates a force that adds velocity to the player
        player.AddRelativeForce(WASD.x, 0f, WASD.y);
        bodyVelocity = player.velocity;
    }

    //Movement Type
    // WASD moves player up, down, left, and right relative to an object's surface
    private void WASDClinging()
    {
        MovementDirection();

        transform.position = Vector3.MoveTowards(transform.position, nextPoint, 3.0f * Time.deltaTime);
    }

    // Press Q and E to roll left and right
    private void Roll()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            transform.Rotate(0f, 0f, 1.0f);
        }
        else if (Input.GetKey(KeyCode.E))
        {
            transform.Rotate(0f, 0f, -1.0f);
        }
    }

    //==========================================/
    // Cling functions
    //==========================================/

    // Parent cling function
    private void Cling()
    {
        // Reset the movement direction variable each update, so that movement keys move the player player the right directions and distance
        movementDirection = transform.position;

        // Cast a sphere around the player that looks for objects with a clingable collider
        FindClingable();

        // Toggle clinging if key is pressed and surface is clingable
        ClingKey();

        // If player is close to surface and key is pressed, snap to surface
        PullToSurface();
    }

    private void FindClingable()
    {
        float distance, nearestDistance = float.MaxValue;

        // Draw a sphere outside of the player to check for clingable surfaces
        Collider[] clingables = Physics.OverlapSphere(movementDirection, 2.0f, 1 << 8);

        // If overlap sphere finds an eligible clingable
        if (clingables.Length > 0)
        {
            foreach (Collider surface in clingables)
            {
                distance = Vector3.Distance(transform.position, surface.ClosestPoint(transform.position));
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    firstPoint = surface.ClosestPoint(transform.position);
                    nearestClingable = surface.gameObject;
                }
            }

            objectIsClingable = true;
        }
        else
        {
            objectIsClingable = false;
        }
    }

    // Toggle cling with left shift
    private void ClingKey()
    {
        // TODO: Some visual cue that the player is near a clingable object? Would be nice for clinging when facing the other way.

        if (Input.GetKeyDown(KeyCode.Space) && (objectIsClingable) && (!playerIsClinging))
        {
            // If left shift is pressed and FindClingable found an eligible surface, start moving to cling
            if (!movingToCling)
            {
                movingToCling = true;
            }
            // If left shift is pressed and the player is already clinging or moving to cling, cancel all that
            else if (movingToCling)
            {
                ResetCling();
            }
        }
        else if ((Input.GetKeyDown(KeyCode.Space) && (!objectIsClingable)))
        {
            StopVelocity();
        }
    }

    // Change cling variables to false, movement type to velocity
    private void ResetCling()
    {
        objectIsClingable = false;
        movingToCling = false;
        playerIsClinging = false;
        WASDType = MovementType.velocity;
    }

    // Pull player toward clingable surface
    private void PullToSurface()
    {
        if (objectIsClingable && movingToCling)
        {
            transform.position = Vector3.MoveTowards(transform.position, firstPoint, 5.0f * Time.deltaTime);
        }
    }

    // If the player is moving to cling when they collide with a clingable surface, change movement type and stop velocity
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Clingable" && (movingToCling))
        {
            objectIsClingable = false;
            movingToCling = false;
            playerIsClinging = true;
            WASDType = MovementType.clinging;

            StopVelocity();
        }
    }

    private void MovementDirection()
    {
        FindClosestNormal();

        if (Input.GetAxisRaw("Horizontal") > 0)
        {
            Vector3 rightProjection = Vector3.ProjectOnPlane(transform.right, bodyHit.normal).normalized;
            movementDirection += rightProjection;
        }
        else if (Input.GetAxisRaw("Horizontal") < 0)
        {
            Vector3 leftProjection = Vector3.ProjectOnPlane(-transform.right, bodyHit.normal).normalized;
            movementDirection += leftProjection;
        }

        if (Input.GetAxisRaw("Vertical") > 0)
        {
            Vector3 forwardProjection = Vector3.ProjectOnPlane(transform.forward, bodyHit.normal).normalized;
            movementDirection += forwardProjection;
        }
        else if (Input.GetAxisRaw("Vertical") < 0)
        {
            Vector3 backwardProjection = Vector3.ProjectOnPlane(-transform.forward, bodyHit.normal).normalized;
            movementDirection += backwardProjection;
        }

        nextPoint = bodyHit.collider.ClosestPoint(movementDirection) + closestNormal;
    }

    private void FindClosestNormal()
    {
        nextPoint = firstPoint;
        Vector3 rayDir = nextPoint - movementDirection;

        if (Physics.Raycast(movementDirection, rayDir, out bodyHit, 1f))
        {
            closestNormal = bodyHit.normal * 0.5f;
            nextPoint += closestNormal;
        }
    }

    //=========================================/
    // Other key-press functions
    //=========================================/

    private void Jump()
    {
        if (playerIsClinging && (Input.GetKeyDown(KeyCode.Space)))
        {
            playerIsJumping = true;
        }

        JumpCharge();

        if (playerIsJumping)
        {

            // Jump direction is where the player is looking, and speed is determined by JumpCharge function
            Vector3 jumpVector = transform.forward * jumpStrength;

            // If space is released while looking away from surface, jump with charged up force
            if (Input.GetKeyUp(KeyCode.Space) && (!PlayerCamera.lookingAtSame))
            {
                ResetCling();

                player.AddForce(jumpVector, ForceMode.Impulse);

                // Reset jumpStrength to zero after finished jumping
                jumpStrength = 0.0f;
                playerIsJumping = false;
            }
            // If space is released while looking at surface, cancel jump and reset jumpStrength to 0;
            else if (Input.GetKeyUp(KeyCode.Space) && (PlayerCamera.lookingAtSame))
            {
                // Reset jumpStrength to zero after finished jumping
                jumpStrength = 0.0f;
                playerIsJumping = false;
            }
        }
    }

    private void JumpCharge()
    {
        // TODO: Delay between pressing space and charging, so players can let go of surface by tapping without pushing away from the surface.
        if ((playerIsClinging) && (Input.GetKey(KeyCode.Space)) && (!PlayerCamera.lookingAtSame))
        {
            // TODO: Jump strength tweaking
            jumpStrength = Mathf.Clamp(jumpStrength, 0.0f, 4.0f);

            // Charge up jump by holding space
            jumpStrength += (6.0f * Time.deltaTime);
            cameraFOV = Mathf.Lerp(60.0f, 66.0f, jumpStrength / 2.0f);
        }
        // If space is released, snap cameraFOV back to default
        else if (((!playerIsClinging) && (cameraFOV > 60.0f)) || ((playerIsClinging) && (PlayerCamera.lookingAtSame)))
        {
            float smoothVelocity = 0.0f;
            cameraFOV = Mathf.SmoothDamp(cameraFOV, 60.0f, ref smoothVelocity, 8.0f * Time.deltaTime);
        }

    }

    private void AdjustCamerFOV()
    {
        Camera.main.fieldOfView = cameraFOV;
    }

    private void StopVelocity()
    {
        player.velocity = Vector3.zero;
    }
    
    // Toggle flashlight with F key
    private void Flashlight()
    {
        if (flashlightIsOn)
        {
            flashlight.enabled = true;
            if (Input.GetKeyDown(KeyCode.F)) { flashlightIsOn = false; }
        }
        else if (!flashlightIsOn)
        {
            flashlight.enabled = false;
            if (Input.GetKeyDown(KeyCode.F)) { flashlightIsOn = true; }
        }
    }

    // Esc key immediately closes game
    private void PauseGame()
    {
        // TODO: Pause menu
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}
