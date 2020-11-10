using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This is a behaviour for a gun object which is a child
// of another object. Other than the available variables here,
// also pay attention to correct setting of the two Vector3s that
// control the ADS and hip-fire locations of the weapon. It is
// designed to work alongside the FPSControl behaviour from this
// repository, but would work independently of it if references were
// removed.

public class GunControl : MonoBehaviour
{
    // External variables
    public float RefireRate = 0.1f; // The time in s between shots
    public int MaxAmmo = 25;
    public int CurrentAmmo; // Public so it can be accessible to the ammo counter
    public float Kickback = 5; // Vertical kick
    public float Sway = 3; // Horizontal kick
    public float ReturnRate = 15; // Adjusts the rate at which the weapon compensates sway/kick
    public float ADSSpeed = 10; // Adjusts the rate at which the weapon switches from hip to ADS
    public float ReloadTime = 2;
    public float Range = 20;
    public bool Reloading; // A boolean to track if the player is currently reloading or not
    public GameObject HitSpark; // The object spawned where the weapon hits a thing
    public GameObject MuzzleFlash; // The object spawned at the weapon when fired
    public AudioSource SFX; // The sound played when the gun is fired

    // Internal variables
    float RefireCooldown; // Timer for refiring a single shot
    float ReloadCooldown; // Timer for reloading the weapon
    FPSControl Control;

    // Initiate by setting ammo to max
    void Start()
    {
        CurrentAmmo = MaxAmmo;
        Cursor.visible = false;
        Control = GetComponentInParent<FPSControl>();
    }

    void Fire()
    {
        // Creating the muzzleflash effect
        Instantiate( MuzzleFlash, transform.position + ( transform.forward * 0.5f ), transform.rotation, transform );

        // Raycasting for the projectile, and instantiation of sparks on impact
        RaycastHit hit;
        if( Physics.Raycast( transform.position, transform.forward, out hit, Range ) )
        {
            Instantiate( HitSpark, hit.point - ( transform.forward * 0.1f ), Quaternion.identity );
        }

        SFX.PlayOneShot( SFX.clip, Random.Range(0.7f, 1.5f) );

        // Admin on the weapon - applying kick/sway, setting the cooldown and depreciating ammo
        transform.Rotate( new Vector3( -Kickback, Random.Range(-Sway, Sway), 0 ), Space.Self );
        RefireCooldown = RefireRate;
        CurrentAmmo = CurrentAmmo - 1;
    }

    void Update()
    {
        // Positions for the weapon at ADS/hip-fire modes
        Vector3 ADSPos = Vector3.zero;
        Vector3 HipPos = new Vector3(0.06f, -0.02f, 0.06f );

        // Moves the weapon between aiming positions depending on RMB
        if( Input.GetButton( "Fire2" ) )
            transform.localPosition = Vector3.Lerp( transform.localPosition, ADSPos, ADSSpeed * Time.deltaTime );
        else
            transform.localPosition = Vector3.Lerp( transform.localPosition, HipPos, ADSSpeed * Time.deltaTime );

        // Lerps the rotation of the weapon back towards neutral to counter kickback/sway
        transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.identity, ReturnRate * Time.deltaTime);

        RefireCooldown = RefireCooldown - Time.deltaTime; // Automatically reduces refire cooldown

        // Depreciates the cooldown if the weapon is being reloaded
        if( Reloading )
        {
            ReloadCooldown = ReloadCooldown - Time.deltaTime;
        }
        else
        {
            // Calling the function which fires weapon
            if(Input.GetButton( "Fire1" ) & CurrentAmmo > 0 & RefireCooldown <= 0 )
                Fire();

            // Activate loading loop if user hits the reload button and is not currently reloading
            // Sets a reload timer, and sets the Reloading boolean
            if( Input.GetButtonDown( "Reload" ) ) 
            {
                ReloadCooldown = ReloadTime;
                Reloading = true;
            }
        }
        // Once reloading is complete, reset ammo to max and de-flag reloading
        if( Reloading & ReloadCooldown <= 0 )
        {
            ReloadCooldown = 0;
            CurrentAmmo = MaxAmmo;
            Reloading = false;
        }
    }
}
