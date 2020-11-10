using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This is a basic FPS control system. Its parameters are currently
// set up for use in a high-gravity environment, so probably won't work
// for many situations out-of-the-box, but should work with basic
// tweaking of the given variables.

public class FPSControl : MonoBehaviour 
{
    public Rigidbody body;
    public Transform cam;
    public float mouseSensitivity = 5f;
    public float strafeTilt = 0f;
    public float moveSpeed = 0.25f;
    public float jumpForce = 15f;
    public bool enableMovement = true;
    public bool grounded;
    bool jumping;

    void Start ()
    {
        body = GetComponent<Rigidbody>();
        cam = GetComponentInChildren<Camera>().transform;
    }

    void Update ()
    {

        // Checking to see if the player is on the ground by raycasting downward by a distance plus a margin (in this case 0.1)
        float groundedRange = 1.1f;
        grounded = Physics.Raycast( transform.position, -Vector3.up, groundedRange );

        // Camera controls
        // Independently calculating different rotation amounts
        float xrot = -Input.GetAxis("Mouse Y") * mouseSensitivity;
        float yrot = Input.GetAxis("Mouse X") * mouseSensitivity;
        float zrot = -Input.GetAxis("Horizontal") * strafeTilt;

        // Rotating the camera for looking up/down
        cam.localEulerAngles = new Vector3( cam.localEulerAngles.x + xrot, 0, zrot );

        // Rotating the player body for looking left/right
        body.MoveRotation( body.rotation * Quaternion.Euler( new Vector3( 0, yrot, 0 ) ) );
        
        // Jumping controls
        if( Input.GetButtonDown("Jump") & grounded )
            {
                jumping = true;
            }
    }

    void FixedUpdate()
    {
        // Turning input into a magnitude-clamped 2D vector
        Vector2 vector = Vector2.ClampMagnitude( new Vector2( Input.GetAxis("Horizontal"), Input.GetAxis("Vertical") ), 1f );
        Vector3 moving = ( transform.forward * vector.y * moveSpeed );
        Vector3 strafing = ( transform.right * vector.x * moveSpeed );
        
        // Applying movement to the player's position
        if( enableMovement )
        {
            body.MovePosition(transform.position + moving + strafing);
        }

        // Physics for jumping
        if( jumping )
            {
                body.velocity = new Vector3( body.velocity.x, jumpForce, body.velocity.z );
                jumping = false;
            }
    }
}