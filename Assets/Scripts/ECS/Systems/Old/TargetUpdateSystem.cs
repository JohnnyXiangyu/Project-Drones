//using Unity.Burst;
//using Unity.Collections;
//using Unity.Entities;
//using Unity.Jobs;
//using Unity.Mathematics;
//using Unity.Transforms;

//public class TargetUpdateSystem : SystemBase
//{
   
//    protected override void OnUpdate()
//    {
//        EntityQuery targets = GetEntityQuery(ComponentType.ReadOnly<Translation>(), ComponentType.ReadOnly<TargetTag>());
//        var positions = targets.ToComponentDataArray<Translation>(Allocator.TempJob);
//        var tags = targets.ToComponentDataArray<TargetTag>(Allocator.TempJob);

//        bool wDown = InputAgent.wDown;
//        bool sDown = InputAgent.sDown;
//        bool dDown = InputAgent.dDown;
//        bool aDown = InputAgent.aDown;

//        float timeDelta = Time.DeltaTime;

//        Entities.ForEach((ref DroneNavigator steer) => {
//            for (int i = 0; i < positions.Length; i++) {
//                if (steer.tag == tags[i].tag) {
//                    steer.target = positions[i].Value;
//                    steer.activate = true;
//                    break;
//                }
//            }
//        }).WithDisposeOnCompletion(positions).WithDisposeOnCompletion(tags).Schedule();

//        // process user input
//        Entities.ForEach((ref Translation translation, in TargetTag tag) => {
//            if (wDown) {
//                translation.Value += new float3(0, 0, timeDelta);
//            }
//            if (sDown) {
//                translation.Value += new float3(0, 0, -timeDelta);
//            }
//            if (aDown) {
//                translation.Value += new float3(-timeDelta, 0, 0);
//            }
//            if (dDown) {
//                translation.Value += new float3(timeDelta, 0, 0);
//            }
//        }).ScheduleParallel();
//    }
//}
