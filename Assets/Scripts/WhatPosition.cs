using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

public class WhatPosition : MonoBehaviour
{
    public CarBehaviour carBehaviour;
    public int pos = 0;

    private void Start()
    {
        carBehaviour = GetComponent<CarBehaviour>();
    }
    
    private void FixedUpdate()
    {
        float reward = Mathf.Clamp01(1f - (pos / 6f));
        carBehaviour.AddReward((reward - 0.5f) * 0.1f);
    }
}
