using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class ChaseTargetDistribution : SystemBase
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
        //// DEBUG only, avoid null ref bug
        //if (SpawnDrone.instance == null) {
        //    return;
        //}
        
        //// only update target positions when there's a new drone
        //if (!SpawnDrone.instance.newDroneThisFrame && SpawnDrone.instance.newDroneTag != -1) {
        //    return;
        //}

        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();

        // update positions for aforementioned targets
        // TODO: remember to extend the above code to work with multiple tags

        //// find all drones
        //EntityQuery drones = GetEntityQuery(typeof(Translation), ComponentType.ReadOnly<UnitNavigator>(), ComponentType.ReadOnly<ProtoChaseTag>());
        //var droneEntities = drones.ToEntityArray(Allocator.TempJob);
        //var navigators = drones.ToComponentDataArray<UnitNavigator>(Allocator.TempJob);
        //var positions = drones.ToComponentDataArray<Translation>(Allocator.TempJob);
        //var tags = drones.ToComponentDataArray<ProtoChaseTag>(Allocator.TempJob);

        //// define parameters
        //int initialRadius = 5;
        //int initialLimit = 5;
        //int incrementRadius = 3;
        //int limitMultiplyer = 2;

        //// loop through targets
        //Entities.ForEach((ref ProtoChaseTarget target, in Translation translation) => {
        //    int count = 0;

        //    // count chasing drones
        //    for (int i = 0; i < droneEntities.Length; i++) {
        //        if (target.tag == tags[i].id) {
        //            // increment count
        //            count++;
        //        }
        //    }

        //    // setup initial parameters
        //    float separateAngle = math.PI * 2 / math.min(initialLimit, count);

        //    for (int i = 0; i < droneEntities.Length; i++) {
        //        // skip other drones
        //        if (target.tag == tags[i].id) {
        //            // get a local reference
        //            var pos = positions[i];
        //            var tag = tags[i];
        //            var ent = droneEntities[i];

        //            // increment count
        //            count++;
        //        }
        //    }
        //}).WithDisposeOnCompletion(droneEntities).WithDisposeOnCompletion(positions).WithDisposeOnCompletion(tags).Schedule();

        // TODO: new design is to use *DYNAMIC GROUPING*
        
    }
}
