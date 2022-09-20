using UnityEngine;

public class SpectatorCamera : MonoBehaviour //Daan
{
    [Header("Spectator Settings")]
    public bool cameraMovementActive = true;
    public float mouseSensitivity = 100f;
    public float movementSpeed = 5f;

    [Header("Private")]
    private float mouseX;
    private float mouseY;

    private void Awake()
    {
        if (PlayerPrefs.HasKey("sensitivity") == true)
            mouseSensitivity = PlayerPrefs.GetFloat("sensitivity");
        if (PlayerPrefs.HasKey("speed") == true)
            movementSpeed = PlayerPrefs.GetFloat("speed");
    }

    private void Update()
    {
        if (cameraMovementActive == true)
        {
            CameraMovement();
            MouseLook();
        }
    }

    private void CameraMovement()
    {
        //WASD for forwards, backwards, right and left camera movement.
        if (Input.GetKey(KeyCode.W))
            transform.position = transform.position + (transform.forward * movementSpeed * Time.deltaTime);
        if (Input.GetKey(KeyCode.S))
            transform.position = transform.position + (-transform.forward * movementSpeed * Time.deltaTime);
        if (Input.GetKey(KeyCode.A))
            transform.position = transform.position + (-transform.right * movementSpeed * Time.deltaTime);
        if (Input.GetKey(KeyCode.D))
            transform.position = transform.position + (transform.right * movementSpeed * Time.deltaTime);

        //RF or EQ or Space + ctrl for Up and down movement
        if (Input.GetKey(KeyCode.R) || Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.Space))
            transform.position = transform.position + (transform.up * movementSpeed * Time.deltaTime);
        if (Input.GetKey(KeyCode.F) || Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.LeftControl))
            transform.position = transform.position + (-transform.up * movementSpeed * Time.deltaTime);

        //LeftShift for a Speed boost
        if (Input.GetKeyDown(KeyCode.LeftShift))
            movementSpeed = movementSpeed * 2;
        if (Input.GetKeyUp(KeyCode.LeftShift))
            movementSpeed = movementSpeed / 2;
    }

    private void MouseLook()
    {
        //when right mouse button is pressed you can use to mouse to look around. it wil lock the mouse in the middle of the screen and make it invisible
        if (Input.GetMouseButton(1))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

             mouseX += Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
             mouseY += Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            transform.rotation = Quaternion.Euler(-mouseY, mouseX, 0f);
        }

        //unlock the mouse and makes it visible when rightmouse button is released
        if (Input.GetMouseButtonUp(1))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    } 

    public void SettingsUpdate()
    {
        movementSpeed = PlayerPrefs.GetFloat("speed");
        mouseSensitivity = PlayerPrefs.GetFloat("sensitivity");
    }
}
