using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

/// <summary>
/// the first type of task: chase an object
/// this implementation only support a single type of drone (which means no variation of range)
/// use it with a non-collision drone
/// </summary>
[GenerateAuthoringComponent]
[Serializable]
public struct ChaseTagVer1 : IComponentData
{
    /// <summary>
    /// id number of the target it's chasing: it's shared by all targets of all types so don't mismatch
    /// </summary>
    public int id;

    /// <summary>
    /// set it to false so the system knows when to obtain a new target
    /// </summary>
    public bool initialized;

    public float3 relativePosition;
}
