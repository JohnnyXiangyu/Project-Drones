//using Unity.Burst;
//using Unity.Collections;
//using Unity.Entities;
//using Unity.Jobs;
//using Unity.Mathematics;
//using Unity.Transforms;
//using Unity.Physics;

//public class FlockMovement : SystemBase
//{
//    protected override void OnUpdate()
//    {
//        var friends = GetEntityQuery(ComponentType.ReadOnly<Translation>(), ComponentType.ReadOnly<AvoidanceController>());
//        var positions = friends.ToComponentDataArray<Translation>(Allocator.TempJob);

//        float timeDelta = Time.DeltaTime;

//        Entities.ForEach((/*ref PhysicsVelocity velocity, */ref Translation translation, in Rotation rotation, in AvoidanceController flock, in DroneNavigator steer) => {
//            float3 finalDirection = float3.zero;
            
//            // TODO: engine velocity
//            if (steer.activate && math.length(steer.target - translation.Value) > 10) {
//                // move forward
//                float3 forward = math.mul(rotation.Value, new float3(0, 0, 1));
//                finalDirection += math.normalize(forward) * steer.linearVelocity;
//            }
//            else if (steer.activate && math.length(steer.target - translation.Value) <= 5) {
//                // move backward
//                float3 forward = math.mul(rotation.Value, new float3(0, 0, -1));
//                finalDirection += math.normalize(forward) * steer.linearVelocity;
//            }

//            // TODO: flock adjustment
//            //// iteration2: calculate the cumulative position of nearby objects and move
//            //float3 adjustment = float3.zero;
//            //int count = 0;
//            //for (int i = 0; i < positions.Length; i++) {
//            //    count++;
//            //    float3 diff = translation.Value - positions[i].Value;
//            //    if (math.length(diff) < flock.proximity) {
//            //        adjustment += diff;
//            //    }
//            //}

//            //if (math.length(adjustment) > 0) {
//            //    finalDirection += math.normalize(adjustment) * flock.sideVelocity;
//            //}

//            // iteration3: subtract direction
//            float3 adjustment = float3.zero;
//            for (int i = 0; i < positions.Length; i++) {
//                float3 diff = translation.Value - positions[i].Value;
//                if (math.length(diff) < flock.proximity && math.length(diff) > 0) {
//                    adjustment += math.normalize(diff) / math.length(diff);
//                }
//            }
//            if (math.length(adjustment) > 0) {
//                finalDirection += math.normalize(adjustment) * flock.sideVelocity;
//            }

//            // stop movement
//            for (int i = 0; i < positions.Length; i++) {
//                float3 diff = positions[i].Value - translation.Value;

//                if (math.length(diff) <= flock.proximity && math.length(diff) != 0) {
//                    finalDirection -= math.normalize(diff) * math.max(0, math.dot(diff, finalDirection));
//                }
//            }

//            //// remove jittering
//            //if (steer.activate && math.length(steer.target - translation.Value) > 10) {
//            //    if (math.dot(finalDirection, math.mul(rotation.Value, new float3(0, 0, 1))) < 0) {
//            //        finalDirection += math.dot(finalDirection, math.mul(rotation.Value, new float3(0, 0, 1))) * math.mul(rotation.Value, new float3(0, 0, 1));
//            //    }
//            //}
//            //else if (steer.activate && math.length(steer.target - translation.Value) <= 5) {
//            //    if (math.dot(finalDirection, math.mul(rotation.Value, new float3(0, 0, 1))) > 0) {
//            //        finalDirection -= math.dot(finalDirection, math.mul(rotation.Value, new float3(0, 0, 1))) * math.mul(rotation.Value, new float3(0, 0, 1));
//            //    }
//            //}

//            translation.Value += finalDirection * timeDelta;
//        }).WithDisposeOnCompletion(positions).Schedule();
//    }
//}
