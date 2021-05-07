using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
[Serializable]
public struct ChaseTargetVer1 : IComponentData
{
    public int tag;
    public int count;
    
    // initializable parameters
    public bool initialized;
    public float3 lastPosition;
    
    public bool active;
    
    /// <summary>
    /// the center direction of the current movement object
    /// </summary>
    public float3 direction;

    /// <summary>
    /// the max distance into the attack range
    /// </summary>
    public float thickness;

    /// <summary>
    /// the angle to left and right of center direction
    /// </summary>
    public float halfAngle;
}
