//using Unity.Burst;
//using Unity.Collections;
//using Unity.Entities;
//using Unity.Jobs;
//using Unity.Mathematics;
//using Unity.Transforms;

//public class DummyTargetSystem : SystemBase
//{
//    protected override void OnUpdate()
//    {
//        // Assign values to local variables captured in your job here, so that it has
//        // everything it needs to do its work when it runs later.
//        // For example,
//        //     float deltaTime = Time.DeltaTime;

//        // This declares a new kind of job, which is a unit of work to do.
//        // The job is declared as an Entities.ForEach with the target components as parameters,
//        // meaning it will process all entities in the world that have both
//        // Translation and Rotation components. Change it to process the component
//        // types you want.

//        //EntityQuery drones = GetEntityQuery(typeof(Translation), ComponentType.ReadOnly<UnitNavigator>(), ComponentType.ReadOnly<ProtoChaseTag>());
//        //var droneEntities = drones.ToEntityArray(Allocator.TempJob);
//        //var navigators = drones.ToComponentDataArray<UnitNavigator>(Allocator.TempJob);
//        //var positions = drones.ToComponentDataArray<Translation>(Allocator.TempJob);
//        //var tags = drones.ToComponentDataArray<ProtoChaseTag>(Allocator.TempJob);

//        EntityQuery targets = GetEntityQuery(ComponentType.ReadOnly<Translation>(), ComponentType.ReadOnly<DummyTarget>());
//        var targetPoses = targets.ToComponentDataArray<Translation>(Allocator.TempJob);
//        var targetTags = targets.ToComponentDataArray<DummyTarget>(Allocator.TempJob);

//        Entities.ForEach((ref DummySteer steer) => {
//            int count = 0;
//            float3 targetPos = float3.zero;
//            for (int i = 0; i < targetPoses.Length; i++) {
//                if (targetTags[i].tag == steer.tag) {
//                    targetPos += targetPoses[i].Value;
//                }
//            }

//            if (count > 0) {
//                targetPos /= count;
//            }

//            steer.target = targetPos;
//        }).WithDisposeOnCompletion(targetPoses).WithDisposeOnCompletion(targetTags).Schedule();
//    }
//}
