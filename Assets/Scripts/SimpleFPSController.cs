using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SimpleFPSController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public float mouseSensitivity = 2f;

    public Transform cameraTransform;

    private Rigidbody rb;
    private float xRotation = 0f;
    private bool isGrounded;
    bool canLookAround;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        canLookAround = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (canLookAround) { HandleMouseLook(); }
        HandleJump();
    }

    public void SetCameraState(bool state)
    {
        canLookAround = state;
        Cursor.lockState = !state ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = !state;
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        float moveX = 0f;
        float moveZ = 0f;

        // Input.GetKey movement
        if (Input.GetKey(KeyCode.W)) moveZ += 1f;
        if (Input.GetKey(KeyCode.S)) moveZ -= 1f;
        if (Input.GetKey(KeyCode.A)) moveX -= 1f;
        if (Input.GetKey(KeyCode.D)) moveX += 1f;

        Vector3 move = transform.right * moveX + transform.forward * moveZ;

        Vector3 velocity = rb.linearVelocity;
        Vector3 targetVelocity = move * moveSpeed;

        // Preserve Y velocity (gravity/jumping)
        rb.linearVelocity = new Vector3(targetVelocity.x, velocity.y, targetVelocity.z);
    }

    void HandleJump()
    {
        if (Input.GetKey(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * 100f * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * 100f * Time.deltaTime;

        // Vertical rotation (camera only)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Horizontal rotation (player body)
        transform.Rotate(Vector3.up * mouseX);
    }

    private void OnCollisionStay(Collision collision)
    {
        // Simple ground check
        if (collision.contacts.Length > 0)
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                if (contact.normal.y > 0.5f)
                {
                    isGrounded = true;
                    return;
                }
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }
}