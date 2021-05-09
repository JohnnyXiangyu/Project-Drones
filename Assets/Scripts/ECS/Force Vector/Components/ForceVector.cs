using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
[Serializable]
public struct ForceVector : IComponentData
{
    public float linearVel;
    public float angularVel;
    public float sideVel;

    public float3 direction;
    public float3 slides;
}
