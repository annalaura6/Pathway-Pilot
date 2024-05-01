using UnityEngine;

public class HookController : MonoBehaviour
{
    [SerializeField] private Transform hookPoint;
    [SerializeField] private float pickupRange = 2.0f;
    
    private GameObject _hookedObject;
    
    private void Start()
    {
        HookManager.Instance.RegisterHook(this);
    }

    private void OnDestroy()
    {
        HookManager.Instance.UnregisterHook(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle") && _hookedObject == null)
        {
            if (Vector3.Distance(hookPoint.position, other.transform.position) <= pickupRange)
            {
                _hookedObject = other.gameObject;
                _hookedObject.transform.position = hookPoint.position;  
                _hookedObject.transform.SetParent(hookPoint, true); 
                _hookedObject.GetComponent<Rigidbody>().isKinematic = true; 
            }
        }
    }
    
    public void ReleaseObject()
    {
        if (_hookedObject != null)
        {
            _hookedObject.transform.SetParent(null);
            var rb = _hookedObject.GetComponent<Rigidbody>();
            rb.isKinematic = false; 
            rb.detectCollisions = true; 
            var collider = _hookedObject.GetComponent<Collider>();
            collider.isTrigger = false; 
            _hookedObject = null;
        }
    }
}