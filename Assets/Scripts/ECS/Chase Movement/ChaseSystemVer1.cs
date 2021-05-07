using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

// TODO: new design is to use *DYNAMIC GROUPING*

public class ChaseSystemVer1 : SystemBase
{
    EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;

    protected override void OnCreate() {
        base.OnCreate();
        // Find the ECB system once and store it for later usage
        m_EndSimulationEcbSystem = World
            .GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {        
        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();

        EntityQuery drones = GetEntityQuery(ComponentType.ReadOnly<ChaseTagVer1>(), ComponentType.ReadOnly<Translation>());
        NativeArray<ChaseTagVer1> droneTags = drones.ToComponentDataArray<ChaseTagVer1>(Allocator.TempJob);
        NativeArray<Translation> dronePoses = drones.ToComponentDataArray<Translation>(Allocator.TempJob);

        // part 1: loop all targets, let them count the number of drones in them, then adjust the parameters
        Entities.ForEach((ref ChaseTargetVer1 target, in Translation translation) => {
            // initialization
            if (target.initialized == false) {
                target.lastPosition = translation.Value;
                target.initialized = true;
            }
            
            // count drones in this task
            int count = 0;
            float3 closest = translation.Value;
            for (int i = 0; i < droneTags.Length; i ++) {
                if (droneTags[i].id == target.tag) {
                    count++;
                    if (closest.Equals(translation.Value) || math.distance(closest, translation.Value) > math.distance(dronePoses[i].Value, translation.Value)) {
                        closest = dronePoses[i].Value;
                    }
                }
            }
            target.count = count;

            // recompute the angle (need special behavior for initialization)
            if (count == 0) {
                target.active = false;
            }
            else if (target.active == false) {
                // handle first come
                if (math.distance(closest, translation.Value) <= 30) {
                    // initialize direction
                    target.direction = math.normalize(closest - translation.Value);
                    target.active = true;
                }
            }

            // calculate delta direction of the target itself
            float3 deltaDir = translation.Value - target.lastPosition;
            target.lastPosition = translation.Value;

            // recompute area
            if (target.active == true) {
                // update angle
                if (!deltaDir.Equals(float3.zero)) {
                    // calculate angle
                    float angle = math.acos(math.dot(math.normalize(deltaDir), math.normalize(target.direction)));

                    // move out of way
                    if (angle < math.PI / 4) {
                        float3 tempAxis = math.cross(deltaDir, target.direction);
                        // new assemble direction is the closer 90 deg from current
                        target.direction = math.normalize(-deltaDir);
                        //target.direction = math.mul(quaternion.AxisAngle(tempAxis.Equals(float3.zero)? (new float3(0, 1, 0)) : math.normalize(tempAxis), math.PI/2), math.normalize(deltaDir));
                    }
                }

                // updated area size
                float mult = math.sqrt(count);
                target.halfAngle = math.min(2 * math.PI / 120 * count / 4, math.PI * 2);
                target.thickness = math.min(1f + mult/5, 7);
            }
        }).WithDisposeOnCompletion(droneTags).WithDisposeOnCompletion(dronePoses).Schedule();

        
        // part 2: update drones
        EntityQuery targets = GetEntityQuery(ComponentType.ReadOnly<ChaseTargetVer1>(), ComponentType.ReadOnly<Translation>());
        var targetTags = targets.ToComponentDataArray<ChaseTargetVer1>(Allocator.TempJob);
        var targetPoses = targets.ToComponentDataArray<Translation>(Allocator.TempJob);
        Random random = new Random((uint)(Time.ElapsedTime * 1000) + 1);

        
        // TODO: change it to using relative positions
        Entities.ForEach((ref DummySteer steer, ref ChaseTagVer1 task) => {
            for (int i = 0; i < targetTags.Length; i++) {
                // var target = targetTags[i];

                // will only do it for the first target
                if (targetTags[i].tag == task.id) {
                    if (targetTags[i].active == false) {
                        // steer.target = targetPoses[i].Value;
                        task.relativePosition = float3.zero;
                    }
                    else {
                        // in range test
                        float3 relativePos = task.relativePosition;
                        float relativeAngle = math.acos(math.dot(math.normalize(relativePos), math.normalize(targetTags[i].direction)));

                        // if current point no longer in task range, find another random point
                        if (math.length(relativePos) > 20 || math.length(relativePos) < (20 - targetTags[i].thickness) || relativeAngle > targetTags[i].halfAngle || !steer.activate) {
                            float angle = random.NextFloat(-targetTags[i].halfAngle, targetTags[i].halfAngle);
                            float thickness = random.NextFloat(0, targetTags[i].thickness);

                            float3 newPosition = math.mul(quaternion.AxisAngle(new float3(0, 1, 0), angle), math.normalize(targetTags[i].direction));
                            newPosition = math.normalize(newPosition) * (20 - thickness);
                            task.relativePosition = newPosition;

                            //steer.target = newPosition;

                            steer.activate = true;
                        }
                    }

                    // update actual target based on current relative position
                    steer.target = targetPoses[i].Value + task.relativePosition;

                    break;
                }

                
            }
        }).WithDisposeOnCompletion(targetTags).WithDisposeOnCompletion(targetPoses).Schedule();
    }
}
