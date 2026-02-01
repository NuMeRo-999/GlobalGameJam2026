using UnityEngine;

public class VisionController : MonoBehaviour
{

    public bool hasMask = false;

    void Start()
    {
        // quita y bloquea el cursor al iniciar el juego
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
