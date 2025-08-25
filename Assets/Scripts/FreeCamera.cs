using UnityEngine;

[AddComponentMenu("Camera/Free Camera")]
public class FreeCamera : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 10f;
    public float sprintMultiplier = 2f;
    public float verticalSpeed = 5f;

    [Header("Mouse Look")]
    public float lookSensitivity = 2f;
    public bool lockCursor = true;

    private float rotationX = 0f;
    private float rotationY = 0f;

    void Start()
    {
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void Update()
    {
        // 1) Mouse look
        rotationX += Input.GetAxis("Mouse X") * lookSensitivity;
        rotationY -= Input.GetAxis("Mouse Y") * lookSensitivity;
        rotationY = Mathf.Clamp(rotationY, -90f, 90f);

        transform.localRotation = Quaternion.Euler(rotationY, rotationX, 0f);

        // 2) Move input
        float speed = moveSpeed * (Input.GetKey(KeyCode.LeftShift) ? sprintMultiplier : 1f);
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        Vector3 move = Vector3.zero;
        move += forward * Input.GetAxis("Vertical");
        move += right * Input.GetAxis("Horizontal");

        // Yukarý / Aþaðý
        if (Input.GetKey(KeyCode.Space)) move += Vector3.up * verticalSpeed;
        if (Input.GetKey(KeyCode.LeftControl)) move += Vector3.down * verticalSpeed;

        transform.position += move * speed * Time.deltaTime;
    }
}
