using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class ChaseTargetDistribution : SystemBase
{
    protected override void OnUpdate()
    {
        // DEBUG only, avoid null ref bug
        if (SpawnDrone.instance == null) {
            return;
        }
        
        // only update target positions when there's a new drone
        if (!SpawnDrone.instance.newDroneThisFrame && SpawnDrone.instance.newDroneTag != -1) {
            return;
        }
        
        // update positions for aforementioned targets
        // TODO: remember to extend the above code to work with multiple tags
        
        // find all drones
        EntityQuery drones = GetEntityQuery(typeof(Translation), ComponentType.ReadOnly<UnitNavigator>(), ComponentType.ReadOnly<ChaseTag>());
        //var droneEntities = drones.ToEntityArray(Allocator.TempJob);
        //var positions = drones.ToComponentDataArray<Translation>(Allocator.TempJob);
        //var tags = drones.ToComponentDataArray<ChaseTag>(Allocator.TempJob);

        // loop through targets
        Entities.ForEach((ref DistributiveTarget target, in Translation translation) => {
            
        })/*.WithDisposeOnCompletion(droneEntities).WithDisposeOnCompletion(positions).WithDisposeOnCompletion(tags)*/.Schedule();
    }
}
