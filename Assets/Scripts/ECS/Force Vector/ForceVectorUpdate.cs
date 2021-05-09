using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

/// <summary>
/// Update each entity's force vector on frame.
/// </summary>
public class ForceVectorUpdate : SystemBase
{
    protected override void OnUpdate()
    {
        // prepare data
        EntityQuery targets = GetEntityQuery(ComponentType.ReadOnly<AttackTarget>(), ComponentType.ReadOnly<Translation>());
        var tarPoses = targets.ToComponentDataArray<Translation>(Allocator.TempJob);
        var tarTags = targets.ToComponentDataArray<AttackTarget>(Allocator.TempJob);

        EntityQuery others = GetEntityQuery(ComponentType.ReadOnly<VectorCollision>(), ComponentType.ReadOnly<Translation>());
        var otherEntities = others.ToEntityArray(Allocator.TempJob);
        var otherCDs = others.ToComponentDataArray<VectorCollision>(Allocator.TempJob);
        var otherPoses = others.ToComponentDataArray<Translation>(Allocator.TempJob);

        // update
        Entities.ForEach((Entity e, ref ForceVector vec, in Translation translation, in VectorCollision cd, in Rotation rotation,in AttackTask task, in RangedWeapon weapon, in ActiveTag verdict) => {
            // caluclate traction force
            float3 targetDir = float3.zero;
            float3 tractionForce = float3.zero;

            // find target position
            bool found = false;
            for (int i = 0; i < tarPoses.Length; i++) {
                if (tarTags[i].tag == task.tag) {
                    targetDir = tarPoses[i].Value - translation.Value;
                    found = true;
                    break;
                }
            }
            if (!found) return;

            // update target direction
            vec.targetDirection = targetDir;

            // calculate base force vector (traction) as traction force towards the range
            float traction = math.length(targetDir);
            traction -= weapon.range - 0.1f;
            if (math.abs(traction) <= 0.1f) traction = 0;

            // don't handle zero traction situation: targets will crash drones when they come too close
            if (!targetDir.Equals(float3.zero)) {
                tractionForce = math.normalize(targetDir) * traction;
            }
            
            // calculte repulsion forces
            float3 repulsionForce = float3.zero;
            
            // find the collective force vector from other colliders
            for (int i = 0; i < otherCDs.Length; i++) {
                if (otherEntities[i] == e)
                    continue;

                float3 fromOther = translation.Value - otherPoses[i].Value;
                
                // skip non-colliding ones
                if (math.length(fromOther) >= otherCDs[i].radius + cd.radius) {
                    continue;
                }

                // skip objects in the back
                if (math.dot(fromOther, tractionForce) < 0) {
                    continue;
                }

                // calculate the repulsion force
                float newRepulsion = otherCDs[i].radius + cd.radius - math.length(fromOther);

                // handle zero difference
                if (fromOther.Equals(float3.zero)) {
                    fromOther = math.normalize(math.cross(new float3(0, 1, 0), tractionForce)) * newRepulsion;
                }
                else {
                    fromOther = math.normalize(fromOther) * newRepulsion;
                }

                // add the new repulsion force
                repulsionForce += fromOther;
            }

            // adding the 2 forces together and scale them
            vec.force = tractionForce;
            vec.slides = repulsionForce;
        })
            .WithDisposeOnCompletion(tarPoses)
            .WithDisposeOnCompletion(tarTags)
            .WithDisposeOnCompletion(otherCDs)
            .WithDisposeOnCompletion(otherPoses)
            .WithDisposeOnCompletion(otherEntities)
            .Schedule();
    }
}
