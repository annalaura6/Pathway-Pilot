using UnityEngine;

public class HookManager : MonoBehaviour
{
    public static HookManager Instance { get; private set; }

    private HookController activeHook;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    
    public void RegisterHook(HookController hook)
    {
        activeHook = hook;
    }
    
    public void UnregisterHook(HookController hook)
    {
        if (activeHook == hook)
            activeHook = null;
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ReleaseHook();
        }
    }
    
    public void ReleaseHook()
    {
        if (activeHook != null)
        {
            activeHook.ReleaseObject();
        }
    }
}