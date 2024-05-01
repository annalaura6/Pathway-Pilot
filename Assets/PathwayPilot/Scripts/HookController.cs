using UnityEngine;

public class HookController : MonoBehaviour
{
    private GameObject hookedObject; // Reference to the currently hooked object

    private void Start()
    {
        HookManager.Instance.RegisterHook(this); // Register this hook with the manager
    }

    private void OnDestroy()
    {
        HookManager.Instance.UnregisterHook(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle") && hookedObject == null)
        {
            hookedObject = other.gameObject;
            hookedObject.transform.SetParent(transform);
            hookedObject.GetComponent<Rigidbody>().isKinematic = true; 
        }
    }
    
    public void ReleaseObject()
    {
        if (hookedObject != null)
        {
            hookedObject.transform.SetParent(null);
            var rb = hookedObject.GetComponent<Rigidbody>();
            rb.isKinematic = false;
            rb.detectCollisions = true;
            hookedObject = null;
        }
    }
}