using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class KeyboardMovement : SystemBase {
    protected override void OnUpdate() {
        bool wDown = InputAgent.wDown;
        bool sDown = InputAgent.sDown;
        bool dDown = InputAgent.dDown;
        bool aDown = InputAgent.aDown;

        float timeDelta = Time.DeltaTime;

        //foreach (var target in targetEntities) {
        //    Translation translation = EntityManager.GetComponentData<Translation>(target);

        //    if (wDown) {
        //        translation.Value += new float3(0, 0, timeDelta) * 100;
        //    }
        //    if (sDown) {
        //        translation.Value += new float3(0, 0, -timeDelta) * 100;
        //    }
        //    if (aDown) {
        //        translation.Value += new float3(-timeDelta, 0, 0) * 100;
        //    }
        //    if (dDown) {
        //        translation.Value += new float3(timeDelta, 0, 0) * 100;
        //    }

        //    EntityManager.SetComponentData(target, translation);
        //}
        //targetEntities.Dispose();

        Entities.ForEach((ref Translation translation, in KeyboardRcver rcver) => {
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
        })./*WithDisposeOnCompletion(targetEntities).*/Schedule();
    }
}