using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;

// TODO: maybe move this into a physics system?
public class SteeringSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float timeDelta = Time.DeltaTime;



        Entities.ForEach((ref Translation translation, ref Rotation rotation, /*ref PhysicsVelocity velocity,*/ in DroneNavigator steer, in LocalToWorld matrix) => {
            // forward alignment
            {
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

                float4 tempAxis = new float4(axis.x, axis.y, axis.z, 0);

                tempAxis = math.mul(math.inverse(matrix.Value), tempAxis);
                axis.x = tempAxis.x;
                axis.y = tempAxis.y;
                axis.z = tempAxis.z;

                // perform rotation
                rotation.Value = math.mul(rotation.Value, quaternion.AxisAngle(axis, actualRotation));
            }
        }).Schedule();
    }
}
