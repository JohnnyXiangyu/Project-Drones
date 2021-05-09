using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class ForceVectorMovement : SystemBase
{
    protected override void OnUpdate()
    {
        float timeDelta = Time.DeltaTime;
        
        // TODO: execute the movement here
        Entities.ForEach((ref Translation translation, ref Rotation rotation, in ForceVector vec, in LocalToWorld matrix) => {
            // if (!vec.force.Equals(float3.zero)) {
                // rotation params
                float finalAngle = 0;
                float3 finalAxis = new float3(0, 1, 0);

                // compute new angle
                // if currently no force, rotate to the target
                float3 targetDir = vec.force.Equals(float3.zero) ? math.normalize(vec.targetDirection) : math.normalize(vec.force);
                float3 forward = math.normalize(math.mul(rotation.Value, new float3(0, 0, 1)));

                float angle = math.acos(math.dot(forward, targetDir));
                float maxRotation = vec.angularVel * timeDelta;
                float actualRotation = math.min(math.abs(angle), maxRotation);

                // compute rotation axis
                float3 axis = math.cross(forward, targetDir);
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

                // linear movement: only move when drone is not too far from desired direction
                if (angle <= 30) {
                    float actualMovement = math.min(vec.linearVel * timeDelta, math.length(vec.force));
                    forward = math.mul(rotation.Value, new float3(0, 0, 1));
                    translation.Value += forward * actualMovement;
                }
            //}
            //else {                
                // side slides: if the drone is not moving forward, adjust side
                float3 mov_forward = math.mul(rotation.Value, new float3(0, 0, 1));

                // adjustment movment
                float3 sideMovement = float3.zero;
                sideMovement = vec.slides - math.dot(vec.slides, mov_forward) * mov_forward;
                //if (vec.direction.Equals(float3.zero)) {
                //    sideMovement = vec.slides;
                //}
                //else {
                //    sideMovement = vec.slides - math.dot(vec.slides, mov_forward) * mov_forward;
                //}

                if (math.length(sideMovement) != 0)
                    sideMovement = sideMovement * timeDelta;
                translation.Value += sideMovement;
            //}

        }).Schedule();
    }
}
