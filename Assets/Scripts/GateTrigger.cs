using UnityEngine;
using UnityEngine.InputSystem;

public class GateTrigger : MonoBehaviour
{
    [SerializeField] private Animator gateAnimator;
    [SerializeField] private GameObject objectToActivate;

    private bool playerInside;
    private bool hasTriggered;

    private static readonly int OpenGateHash = Animator.StringToHash("opengate");

    private void Update()
    {
        if (!playerInside || hasTriggered)
            return;

        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            gateAnimator.SetTrigger(OpenGateHash);

            if (objectToActivate != null)
                objectToActivate.SetActive(true);

            hasTriggered = true;
        }
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
