using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
[Serializable]
public struct UnitNavigator : IComponentData
{
    public float3 target;
    public float tag;
    
    public float angularVelocity;
    public float linearVelocity;
    
    public bool activate;

    public float priorityRot;
    public float3 priorityAxis;
    public int times;
}
