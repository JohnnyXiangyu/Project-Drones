using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class CameraFollowSystem : SystemBase {
    protected override void OnUpdate() {
        EntityQuery targets = GetEntityQuery(ComponentType.ReadOnly<Translation>(), ComponentType.ReadOnly<FollowedByCamera>());
        var positions = targets.ToComponentDataArray<Translation>(Allocator.Temp);

        float3 newPosition = float3.zero;
        
        foreach (var position in positions) {
            newPosition += position.Value;
        }

        if (positions.Length > 0) {
            CameraFollow.instance.SetObjPosition(newPosition / positions.Length);
        }

        positions.Dispose();
    }
}
