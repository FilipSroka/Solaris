using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 50f; 
    public float rotationSpeed = 2f; 
    public float maxRotationAngle = 360f; 
    public float slightForceAmount = 0.5f; // Adjust this for desired effect
    public float speedChangeStep = 5f; // How much speed changes per key press
    private float targetPitch = 0;
    private float targetRotationY = 0;


    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update() 
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            speed += speedChangeStep;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            speed -= speedChangeStep;
        }
        if (Input.GetKey(KeyCode.A)) 
        {
            targetRotationY -= rotationSpeed;
        }
        if (Input.GetKey(KeyCode.D))
        {
            targetRotationY += rotationSpeed;
        }
        if (Input.GetKey(KeyCode.W))
        {
            targetPitch -= rotationSpeed; 
        }
        if (Input.GetKey(KeyCode.S))
        {
            targetPitch += rotationSpeed; 
        }
        targetPitch = Mathf.Clamp(targetPitch, -maxRotationAngle, maxRotationAngle); 

        // Apply Rotation
        Quaternion targetRotation = Quaternion.Euler(targetPitch, targetRotationY, 0);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 5f); 

        // Forward Movement (Immediately based on updated facing direction)
        // Movement with Primarily Velocity & Slight Force
        Vector3 targetVelocity = transform.forward * speed; 

        // Slight Force Adjustment
        rb.AddForce(transform.forward * slightForceAmount, ForceMode.Force);

        // Apply most of the desired movement through velocity change
        rb.velocity = Vector3.Lerp(rb.velocity, targetVelocity, Time.deltaTime * 5f); 
    }
}













