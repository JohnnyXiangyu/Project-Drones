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
            // zero force vector indicates no need to move
            if (vec.direction.Equals(float3.zero)) {
                return;
            } 
            
            // rotation params
            float finalAngle = 0;
            float3 finalAxis = new float3(0, 1, 0);

            // compute new angle
            float3 diff =  math.normalize(vec.direction);
            float3 forward = math.normalize(math.mul(rotation.Value, new float3(0, 0, 1)));

            float angle = math.acos(math.dot(forward, diff));
            float maxRotation = vec.angularVel * timeDelta;
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

            // linear movement
            float actualMovement = vec.linearVel * timeDelta;
            forward = math.mul(rotation.Value, new float3(0, 0, 1));
            translation.Value += forward * actualMovement;

            // adjustment movment
            float3 sideMovement = vec.slides - math.dot(vec.slides, forward) * forward;
            if (math.length(sideMovement) != 0)
                sideMovement = math.normalize(sideMovement) * vec.sideVel * timeDelta;
            translation.Value += sideMovement;
            
        }).Schedule();
    }
}
