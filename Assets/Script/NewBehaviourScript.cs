using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Transform cameraTransform;

    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpHeight = 2f;

    [SerializeField] private float groundDistance = .2f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private Animator _animator;

    private Vector3 velocity;
    private bool isGrounded;

/*************  ✨ Windsurf Command ⭐  *************/
    /// <summary>
    /// Handles player movement and jumping based on user input.
    /// Checks if the player is grounded for jumping logic and applies gravity.
    /// Moves the player character relative to the camera's orientation.
    /// </summary>

/*******  01a4813c-06c1-49e7-ae99-d18c2958d3ca  *******/
    void Update()
    {
        // Ground Check
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Input
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 inputDir = new Vector3(horizontal, 0f, vertical).normalized;

        //WASD to face
        if (inputDir.magnitude >= 0.1f)//only move if magnitude large other wise just rotate
        {
            float targetAngle = Mathf.Atan2(inputDir.x, inputDir.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;//movement dir rel to camera
            Quaternion targetRotation = Quaternion.Euler(0f, targetAngle, 0f);//rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            characterController.Move(moveDir.normalized * moveSpeed * Time.deltaTime);
            _animator.SetFloat("Speed", (moveDir * moveSpeed * Time.deltaTime).magnitude);
        }
        else _animator.SetFloat("Speed", 0f);

        // Jump
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }
}

