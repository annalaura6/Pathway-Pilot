using UnityEngine;

public class JoystickController : MonoBehaviour
{
    public VRHelicopterController helicopterController;
    public float maxTiltAngle = 30f; // Maximum tilt angle of the joystick

    private bool isGrabbed = false;
    private Vector3 startRotation;
    private Vector3 currentRotation;

    private void Start()
    {
        // Set the handleTransform to this GameObject's transform
        startRotation = this.transform.localEulerAngles;
    }

    private void Update()
    {
        if (isGrabbed)
        {
            // Use this GameObject's current rotation
            currentRotation = this.transform.localEulerAngles;

            // Calculate the tilt of the joystick and map it to helicopter movement
            Vector2 joystickTilt = CalculateJoystickTilt();
            helicopterController.PitchRoll(joystickTilt);
        }
    }

    public void OnGrabStart()
    {
        isGrabbed = true;
        // Handle any logic needed when grabbing starts
    }

    public void OnGrabEnd()
    {
        isGrabbed = false;
        // Handle any logic needed when grabbing ends
    }

    private Vector2 CalculateJoystickTilt()
    {
        Vector2 tilt = new Vector2(
            Mathf.DeltaAngle(startRotation.x, currentRotation.x),
            Mathf.DeltaAngle(startRotation.z, currentRotation.z)
        );
        return Vector2.ClampMagnitude(tilt / maxTiltAngle, 1.0f);
    }

    // These methods can be called by the TouchHandGrab script when grab interaction occurs
}