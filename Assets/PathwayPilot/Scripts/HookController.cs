using UnityEngine;

public class HookController : MonoBehaviour
{
    private GameObject hookedObject; // Reference to the currently hooked object

    // Called when the hook collides with something
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle") && hookedObject == null)
        {
            hookedObject = other.gameObject;
            // Attach the hooked object to the hook
            hookedObject.transform.SetParent(transform);
            hookedObject.GetComponent<Rigidbody>().isKinematic = true; // Disable physics on the object
        }
    }

    // Called to release the currently hooked object
    public void ReleaseObject()
    {
        if (hookedObject != null)
        {
            // Detach the hooked object from the hook
            hookedObject.transform.SetParent(null);
            hookedObject.GetComponent<Rigidbody>().isKinematic = false; // Enable physics on the object
            hookedObject = null; // Clear the reference to the hooked object
        }
    }
}