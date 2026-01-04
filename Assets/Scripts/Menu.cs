using UnityEngine;
using UnityEngine.InputSystem;

public class Menu : MonoBehaviour
{
    public InputActionAsset input;
    public InputActionReference openClose;
    public GameObject menu;
    protected bool opened;

    void OnEnable()
    {
        input.Enable();
    }

    void OnDisable()
    {
        input.Disable();
    }

    void Awake()
    {
        menu.SetActive(false);

        openClose.action.performed += OnOpenClose;
    }

    protected virtual void OnOpenClose(InputAction.CallbackContext context)
    {
        opened = !opened;

        if (opened)
        {
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            menu.SetActive(true);
        }
        else
        {
            Time.timeScale = 1;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            menu.SetActive(false);
        }
    }
}