using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class KeyboardMovement : SystemBase {
    protected override void OnUpdate() {
        EntityQuery targets = GetEntityQuery(typeof(Translation), ComponentType.ReadOnly<KeyboardRcver>());
        var targetEntities = targets.ToEntityArray(Allocator.Temp);

        bool wDown = InputAgent.wDown;
        bool sDown = InputAgent.sDown;
        bool dDown = InputAgent.dDown;
        bool aDown = InputAgent.aDown;

        float timeDelta = Time.DeltaTime;

        foreach (var target in targetEntities) {
            Translation translation = EntityManager.GetComponentData<Translation>(target);

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

            EntityManager.SetComponentData(target, translation);

            //CameraFollow.instance.SetObjPosition(translation.Value);
        }
        targetEntities.Dispose();

        //var positions = targets.ToComponentDataArray<Translation>(Allocator.TempJob);
        //var tags = targets.ToComponentDataArray<KeyboardRcver>(Allocator.TempJob);
        //Entities.ForEach((ref UnitNavigator steer) => {
        //    for (int i = 0; i < positions.Length; i++) {
        //        if (steer.tag == tags[i].tag) {
        //            steer.target = positions[i].Value;
        //            steer.activate = true;
        //            break;
        //        }
        //    }
        //}).WithDisposeOnCompletion(positions).WithDisposeOnCompletion(tags).Schedule();
    }
}