using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VehiclePhysics;

public class Test : MonoBehaviour
{
    VPStandardInput m_input;
    void Start()
    {
        m_input = GetComponent<VPStandardInput>();
        float throttle = m_input.externalThrottle;
        float brake = m_input.externalBrake;
        float steering = m_input.externalSteer;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
