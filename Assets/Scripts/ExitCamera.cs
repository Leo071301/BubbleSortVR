using UnityEngine;
using UnityEngine.InputSystem; 

public class ExitCamera : MonoBehaviour
{
    void Update()
    {
        // Check if the keyboard exists and if any key was just hit
        if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
        {
            gameObject.SetActive(false);
        }
    }
}