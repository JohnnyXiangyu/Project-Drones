//using Unity.Burst;
//using Unity.Collections;
//using Unity.Entities;
//using Unity.Jobs;
//using Unity.Mathematics;
//using Unity.Transforms;
//using Unity.Physics;

//public class FlockSystem : SystemBase {
//    protected override void OnUpdate() {
//        var friends = GetEntityQuery(ComponentType.ReadOnly<Translation>(), ComponentType.ReadOnly<DroneNavigator>());
//        var positions = friends.ToComponentDataArray<Translation>(Allocator.TempJob);

//        float timeDelta = Time.DeltaTime;

//        Entities.WithNone<PhysicsVelocity>().ForEach((ref Translation translation, in Rotation rotation, in FlockHub flock) => {
//            // TODO: add flocking algorithm
//            // iteration1: my fake flocking algorithm
//            float3 finalAdjustment = float3.zero;
//            for (int i = 0; i < positions.Length; i++) {
//                // TODO: force distance
//                if (math.length(positions[i].Value - translation.Value) < flock.proximity) {
//                    float3 newAdjustment = positions[i].Value - translation.Value;

//                    if (!newAdjustment.Equals(float3.zero)) {
//                        newAdjustment /= math.pow(math.length(newAdjustment), 2);
//                        finalAdjustment -= newAdjustment;
//                    }
//                    else {
//                        finalAdjustment += math.mul(rotation.Value, new float3(0, 0, -1));
//                    }
//                }
//            }
//            if (!finalAdjustment.Equals(float3.zero)) {
//                translation.Value += math.normalize(finalAdjustment) * flock.sideVelocity * timeDelta;
//            }
//        }).WithDisposeOnCompletion(positions).Schedule();
//    }
//}
