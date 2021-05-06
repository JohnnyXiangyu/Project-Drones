using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;

// local avoidance iteration 3: SC-like collide-and-spin algorithm
public class SteeringSystem : SystemBase {
    protected override void OnUpdate() {
        float timeDelta = Time.DeltaTime;

        // get a list of positions here
        EntityQuery m_Group = GetEntityQuery(ComponentType.ReadOnly<UnitCollider>(), ComponentType.ReadOnly<Translation>());
        var otherEntities = m_Group.ToEntityArray(Allocator.TempJob);
        var otherUnits = m_Group.ToComponentDataArray<Translation>(Allocator.TempJob);
        var otherColliders = m_Group.ToComponentDataArray<UnitCollider>(Allocator.TempJob);

        Entities.ForEach((Entity e, ref Translation translation, ref Rotation rotation, ref UnitCollider cd, ref UnitNavigator steer, in LocalToWorld matrix) => {
            // rotation params
            float finalAngle = 0;
            float3 finalAxis = new float3(0, 1, 0);

            // if currently during another rotation
            if (steer.priorityRot > 0) {
                finalAxis = steer.priorityAxis;
                finalAngle = math.min(steer.angularVelocity * timeDelta, steer.priorityRot);
                steer.priorityRot -= finalAngle;
            }
            // otherwise try to rotate to target
            else {
                // compute angle
                float3 target = steer.target;
                float3 diff = math.normalize(target - translation.Value);
                float3 forward = math.normalize(math.mul(rotation.Value, new float3(0, 0, 1)));

                float angle = math.acos(math.dot(forward, diff));
                float maxRotation = steer.angularVelocity * timeDelta;
                float actualRotation = math.min(math.abs(angle), maxRotation);

                // compute rotation axis
                float3 axis = math.cross(forward, diff);
                if (axis.Equals(float3.zero)) {
                    axis = math.normalize(math.mul(rotation.Value, new float3(0, 1, 0)));
                }
                else {
                    axis = math.normalize(axis);
                }

                finalAxis = axis;
                finalAngle = actualRotation;
            }

            // perform rotation
            float4 tempAxis = new float4(finalAxis.x, finalAxis.y, finalAxis.z, 0);
            tempAxis = math.mul(math.inverse(matrix.Value), tempAxis);
            finalAxis.x = tempAxis.x;
            finalAxis.y = tempAxis.y;
            finalAxis.z = tempAxis.z;
            rotation.Value = math.mul(rotation.Value, quaternion.AxisAngle(finalAxis, finalAngle));

            // goal test
            if (math.distance(translation.Value, steer.target) < 10) return;

            // updated direction
            float3 myForward3 = math.mul(rotation.Value, new float3(0, 0, 1));

            // collision detection
            bool isColliding = false;
            float3 newCoord = translation.Value + myForward3 * steer.linearVelocity * timeDelta;
            float3 collisionPos = new float3();
            UnitCollider collisionCd = new UnitCollider();
            for (int i = 0; i < otherUnits.Length; i++) {
                float3 otherCoord = otherUnits[i].Value;

                float3 diff = otherCoord - newCoord;
                diff.y = 0;

                if (otherEntities[i] != e && math.dot(myForward3, diff) > 0 && math.length(diff) < math.distance(collisionPos, translation.Value)) {
                    // update closest opponent
                    collisionPos = otherCoord;
                    collisionCd = otherColliders[i];

                    isColliding = true;
                }
            }

            // distance from closest collision
            float collisionDist = math.distance(collisionPos, translation.Value);

            // perform movement
            //if (!isColliding || collisionDist > cd.concreteRadius + collisionCd.concreteRadius) {
            translation.Value = newCoord;
            steer.times = 0;
            //}

            // register new direction adjustment
            if (isColliding && collisionDist <= cd.detectionRadius + collisionCd.concreteRadius && steer.priorityRot <= 0) {
                float3 diff = collisionPos - translation.Value;

                if (steer.times <= 20) {
                    steer.priorityAxis = math.cross(diff, myForward3);
                    steer.priorityAxis = (math.length(steer.priorityAxis) > 0) ? math.normalize(steer.priorityAxis) : new float3(0, 1, 0);
                    steer.priorityRot = math.PI / 16;
                    steer.times++;
                }
                else {
                    steer.priorityAxis = new float3(0, 1, 0);
                    steer.priorityRot = math.PI;
                    steer.times = 0;
                }
            }

            //if (true) {
            //    //for (int i = 0; i < otherColliders.Length; i++) {
            //    //    float2 otherCoord = new float2(otherUnits[i].Value.x, otherUnits[i].Value.z);
            //    //    float2 diff = otherCoord - myCoord;
            //    //    float distance = math.length(diff);

            //    //    // if colliding in the front (direction to the target)
            //    //    if (otherEntities[i] != e
            //    //        && distance <= otherColliders[i].concreteRadius + cd.detectionRadius
            //    //        && math.dot(diff, myForward2) > 0
            //    //        && (closestDistance == -1 || closestDistance < distance)) {
            //    //        closestOpponent = otherCoord;
            //    //        closestDistance = distance;
            //    //    }
            //    //}

            //    // rotate to avoid
            //    if (closestDistance != -1) {
            //        float2 diff = closestOpponent - myForward2;
            //        //float2 diffForward = math.dot(diff, myForward2) * myForward2;
            //        //float2 diffSide = diff - diffForward;

            //        float3 avoidAngle = new float3(diff.x, translation.Value.y, diff.y);

            //        steer.priorityAxis = math.cross(avoidAngle, myForward3);
            //        steer.priorityAxis = (math.length(steer.priorityAxis) > 0) ? math.normalize(steer.priorityAxis) : new float3(0, 1, 0);
            //        steer.priorityRot = math.PI / 4;
            //    }
            //}


            //// perform movement
            //if (noCollision) {
            //    translation.Value = newCoord;
            //}
        }).WithDisposeOnCompletion(otherUnits).WithDisposeOnCompletion(otherColliders).WithDisposeOnCompletion(otherEntities).Schedule();
    }
}
