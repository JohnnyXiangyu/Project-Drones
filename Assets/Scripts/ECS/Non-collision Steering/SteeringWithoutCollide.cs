using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class SteeringWithoutCollide : SystemBase {
    protected override void OnUpdate() {
        float timeDelta = Time.DeltaTime;

        Entities.ForEach((Entity e, ref Translation translation, ref Rotation rotation, in DummySteer steer, in LocalToWorld matrix) => {
            // rotation params
            float finalAngle = 0;
            float3 finalAxis = new float3(0, 1, 0);

            // compute new angle
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

            // perform rotation
            float4 tempAxis = new float4(finalAxis.x, finalAxis.y, finalAxis.z, 0);
            tempAxis = math.mul(math.inverse(matrix.Value), tempAxis);
            finalAxis.x = tempAxis.x;
            finalAxis.y = tempAxis.y;
            finalAxis.z = tempAxis.z;
            rotation.Value = math.mul(rotation.Value, quaternion.AxisAngle(finalAxis, finalAngle));

            // goal test
            if (math.distance(translation.Value, steer.target) > 1) {
                float moveDistance = timeDelta * steer.linearVelocity;

                //if (finalAngle != 0) {
                //    moveDistance /= 2;
                //}

                translation.Value += math.mul(rotation.Value, new float3(0, 0, moveDistance));
            }
        }).Schedule();
    }
}
