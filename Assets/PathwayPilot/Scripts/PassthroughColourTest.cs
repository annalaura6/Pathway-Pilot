using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassthroughColourTest : MonoBehaviour
{
    [SerializeField] private OVRPassthroughLayer _passthroughLayer1;
    [SerializeField] private OVRPassthroughLayer _passthroughLayer2;

    private bool isLayerActive = true;

    private void Update()
    {
        float triggerPressure = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger);

        if (isLayerActive)
        {
            _passthroughLayer1.SetBrightnessContrastSaturation(triggerPressure, 0,0);
        }
        else
        {
            _passthroughLayer2.SetBrightnessContrastSaturation(triggerPressure,0,0);
        }

        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            //ToggleCompositionDepth();
            ToggleEnableDisableLayer();
        }
    }

    private void ToggleEnableDisableLayer()
    {
        if (isLayerActive)
        {
            _passthroughLayer1.enabled = false;
            _passthroughLayer2.enabled = true;
        }
        else
        {
            _passthroughLayer1.enabled = true;
            _passthroughLayer2.enabled = false;
        }
        
        isLayerActive = !isLayerActive;
    }

    private void ToggleCompositionDepth()
    {
        int tempDepth = _passthroughLayer1.compositionDepth;
        _passthroughLayer1.compositionDepth = _passthroughLayer2.compositionDepth;
        _passthroughLayer2.compositionDepth = tempDepth;

        isLayerActive = !isLayerActive;
    }
}
