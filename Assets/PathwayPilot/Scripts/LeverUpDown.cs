using Oculus.Interaction;
using UnityEngine;

public class LeverUpDown : MonoBehaviour
{
    public VRHelicopterController helicopterController;
    public Transform leverTransform; // Assign the actual lever transform here

    private float previousValue = 0f;
    private float minAngle = -45f; // Set these based on your actual lever's range of motion
    private float maxAngle = 45f;

    private void Update()
    {
        if (helicopterController.startItUp)
        {
            // Assuming the lever rotates around its X axis
            float currentAngle = leverTransform.localEulerAngles.x;

            // Normalize the angle to a value between -1 and 1
            float normalizedLeverValue = Mathf.InverseLerp(minAngle, maxAngle, currentAngle);
            normalizedLeverValue = normalizedLeverValue * 2f - 1f; // Remap from 0...1 to -1...1

            // Update the helicopter's vertical movement based on the lever's position
            helicopterController.MoveUpDown(new Vector2(0, normalizedLeverValue));

            // If the lever's value hasn't changed significantly, don't update the engine force
            if (Mathf.Approximately(previousValue, normalizedLeverValue))
                return;

            // Update previousValue for the next frame
            previousValue = normalizedLeverValue;

            // Optional: Set the EngineForce directly if it maps to altitude in your MoveUpDown function
            // This requires a public property or field 'MaxEngineForce' to be defined in VRHelicopterController
            if (helicopterController.EngineForce >= 50f && helicopterController.EngineForce <= helicopterController.MaxEngineForce)
            {
                helicopterController.EngineForce = Mathf.Lerp(50f, helicopterController.MaxEngineForce, (normalizedLeverValue + 1f) / 2f);
            }
        }
    }
}