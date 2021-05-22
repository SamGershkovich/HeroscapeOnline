using UnityEngine;

//This is a camera script made by Haravin (Daniel Valcour).
//This script is public domain, but credit is appreciated!

[RequireComponent(typeof(Camera))]
public class CameraSpectator : MonoBehaviour
{
    public float moveSpeed;
    public float shiftAdditionalSpeed;
    public float mouseSensitivity;
    public bool invertMouse;
    public bool autoLockCursor;

    private Camera cam;

    void Start()
    {
        cam = gameObject.GetComponent<Camera>();
        gameObject.name = "SpectatorCamera";
        Cursor.lockState = (autoLockCursor) ? CursorLockMode.Locked : CursorLockMode.None;
    }

    void Update()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            float speed = (moveSpeed + (Input.GetAxis("Fire3") * shiftAdditionalSpeed));
            gameObject.transform.Translate(Vector3.forward * speed * Input.GetAxis("Vertical"));
            gameObject.transform.Translate(Vector3.right * speed * Input.GetAxis("Horizontal"));
            gameObject.transform.Translate(Vector3.up * speed * (Input.GetAxis("Jump") + (Input.GetAxis("Fire1") * -1)));
            gameObject.transform.Rotate(Input.GetAxis("Mouse Y") * mouseSensitivity * ((invertMouse) ? 1 : -1), Input.GetAxis("Mouse X") * mouseSensitivity * ((invertMouse) ? -1 : 1), 0);
            gameObject.transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, 0);
        }

        if (Cursor.lockState == CursorLockMode.None && Input.GetMouseButtonDown(0) && !UIManager.isHoveringUIElement)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else if (Cursor.lockState == CursorLockMode.Locked && Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
