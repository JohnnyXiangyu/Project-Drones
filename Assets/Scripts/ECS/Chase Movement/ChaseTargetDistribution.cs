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
        EntityQuery targets = GetEntityQuery(typeof(Translation), ComponentType.ReadOnly<UnitNavigator>());

        var positions = targets.ToComponentDataArray<Translation>(Allocator.TempJob);
        var navitagors = targets.ToComponentDataArray<UnitNavigator>(Allocator.TempJob);
        
        //Entities.ForEach((ref UnitNavigator steer) => {
        //    // skip unchanged tags
            
        //    // TODO: traverse all objects, calculate the count of objects needing positioning
        //    // TODO: setup a function for repositioning
        //    // TODO: for each index in the object, set a new 

        //    for (int i = 0; i < positions.Length; i++) {
        //        if (steer.tag == tags[i].tag) {
        //            steer.target = positions[i].Value;
        //            steer.activate = true;
        //            break;
        //        }
        //    }
        //}).WithDisposeOnCompletion(positions).WithDisposeOnCompletion(tags).Schedule();
    }
}
