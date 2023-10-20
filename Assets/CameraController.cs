using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    public float flySpeed = 5.0f;

    private Vector3 moveDirection;

    private void Update()
    {
        // Handle camera movement using Unity Input System.
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        moveDirection = new Vector3(horizontalInput, 0, verticalInput).normalized;

        if (Input.GetKey(KeyCode.Space))
        {
            // If the Space key is held down, move the camera upward.
            moveDirection.y = 1.0f;
        }
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            // If the Left Control key is held down, move the camera downward.
            moveDirection.y = -1.0f;
        }

        // Move the camera based on input.
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.Self);
    }
}
