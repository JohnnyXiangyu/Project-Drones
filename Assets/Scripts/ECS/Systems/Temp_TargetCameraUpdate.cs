using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class Temp_TargetCameraUpdate : SystemBase {
    protected override void OnUpdate() {
        EntityQuery targets = GetEntityQuery(typeof(Translation), ComponentType.ReadOnly<TargetTag>());
        var positions = targets.ToComponentDataArray<Translation>(Allocator.TempJob);
        var tags = targets.ToComponentDataArray<TargetTag>(Allocator.TempJob);
        var targetEntities = targets.ToEntityArray(Allocator.Temp);

        bool wDown = InputAgent.wDown;
        bool sDown = InputAgent.sDown;
        bool dDown = InputAgent.dDown;
        bool aDown = InputAgent.aDown;

        float timeDelta = Time.DeltaTime;

        if (positions.Length > 0) {
            Translation translation = EntityManager.GetComponentData<Translation>(targetEntities[0]);

            if (wDown) {
                translation.Value += new float3(0, 0, timeDelta) * 100;
            }
            if (sDown) {
                translation.Value += new float3(0, 0, -timeDelta) * 100;
            }
            if (aDown) {
                translation.Value += new float3(-timeDelta, 0, 0) * 100;
            }
            if (dDown) {
                translation.Value += new float3(timeDelta, 0, 0) * 100;
            }

            EntityManager.SetComponentData(targetEntities[0], translation);

            CameraFollow.instance.SetObjPosition(translation.Value);
        }

        targetEntities.Dispose();

        Entities.ForEach((ref DroneNavigator steer) => {
            for (int i = 0; i < positions.Length; i++) {
                if (steer.tag == tags[i].tag) {
                    steer.target = positions[i].Value;
                    steer.activate = true;
                    break;
                }
            }
        }).WithDisposeOnCompletion(positions).WithDisposeOnCompletion(tags).Schedule();
    }
}