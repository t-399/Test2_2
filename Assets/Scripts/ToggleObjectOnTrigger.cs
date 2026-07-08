using UnityEngine;
using UnityEngine.InputSystem;

public class ToggleObjectOnTrigger : MonoBehaviour
{
    [SerializeField] private GameObject objectToToggle;

    private bool playerInside;

    private void Update()
    {
        if (!playerInside)
            return;

        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            objectToToggle.SetActive(!objectToToggle.activeSelf);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInside = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInside = false;
    }
}
