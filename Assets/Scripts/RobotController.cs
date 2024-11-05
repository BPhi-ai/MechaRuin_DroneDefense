using System.Collections;
using UnityEngine;

public class PlayerMovementWithAnimation : MonoBehaviour
{
    public GameObject bulletBlock;
    public Transform firePoint;
    public float bulletSpeed = 20f;
    public float timeFix = 0;


    public float moveSpeed = 5f;      // Speed of movement
    public float rotationSpeed = 720f; // Speed of rotation
    private Vector3 moveDirection;

    private Animator animator;        // Reference to the Animator component

    public Camera droneCamera;

    void Start()
    {
        // Get the Animator component attached to the GameObject
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Get input from WASD keys
        float moveX = Input.GetAxis("Horizontal");  // A and D keys
        float moveZ = Input.GetAxis("Vertical");    // W and S keys

        // Adding these so that movement is relative to the camera
        Vector3 camForward = droneCamera.transform.forward;
        Vector3 camRight = droneCamera.transform.right;

        camForward.y = 0;
        camRight.y = 0;

        Vector3 vectorMovementX = moveX * camRight;
        Vector3 vectorMovementZ = moveZ * camForward;

        // Calculate the movement direction
        moveDirection = (vectorMovementX + vectorMovementZ).normalized;

        // If there is movement input, move and rotate the character
        if (moveDirection != Vector3.zero)
        {
            MoveAndRotate();
            animator.SetBool("isMoving", true);  // Trigger the movement animation
        }
        else
        {
            animator.SetBool("isMoving", false); // Stop the movement animation (idle state)
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(Reload());
        }

        if (Input.GetKey(KeyCode.F))
        {
            timeFix += Time.deltaTime;
            animator.SetBool("isShooting", true);
            if(timeFix > 0.25)
            {
                // Spawn the bullet at the firePoint's position and rotation
                GameObject bullet = Instantiate(bulletBlock, firePoint.position, firePoint.rotation);

                // Apply velocity to the bullet's Rigidbody to make it move forward
                Rigidbody rb = bullet.GetComponent<Rigidbody>();
                rb.velocity = firePoint.forward * bulletSpeed;
                timeFix = 0;
            }
        } else { 
            animator.SetBool("isShooting", false);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(Jump());
        }
    }

    IEnumerator Reload()
    {
        animator.SetBool("isReloading", true);
        yield return new WaitForSeconds(2);
        animator.SetBool("isReloading",false);
    }

    IEnumerator Jump()
    {
        animator.SetBool("isJumping", true);
        yield return new WaitForSeconds(1);
        animator.SetBool("isJumping", false);
    }


    // Method to move and rotate the object based on input
    void MoveAndRotate()
    {
        // Move the GameObject
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);

        // Rotate the GameObject to face the direction of movement
        Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}
