using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using VoidCommand.Components;
using VoidCommand.Shared;

namespace VoidCommand.Systems
{
    public class PredictMovementSystem: JobComponentSystem
    {
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var leadTime = SimulationSettings.Instance.predictionsLeadTime;
            
            return Entities.ForEach((ref PredictMovement predictMovement, in Position position, in Movement movement) =>
            {
                var future = PhysicsUtils.PredictFutureState(new PhysicalState()
                {
                    Position = position.Value,
                    Velocity = movement.Velocity,
                    Acceleration = movement.Acceleration
                }, leadTime);

                predictMovement.Position = future.Position;
                predictMovement.Velocity = future.Velocity;
            }).Schedule(inputDeps);
        }
    }
}