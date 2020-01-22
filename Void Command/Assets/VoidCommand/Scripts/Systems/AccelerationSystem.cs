using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using VoidCommand.Components;
using VoidCommand.Shared;

namespace VoidCommand.Systems
{
    public class AccelerationSystem : JobComponentSystem
    {
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var headingTolerance = SimulationSettings.Instance.headingToleranceToAccelerate;

            return Entities.ForEach((ref Rotation rotation, ref Movement movement, in MovementOrder movementOrder) =>
            {
                // Accelerate only if orientation is within tolerance
                var fwd =  math.forward(rotation.Value);
                var angle = PhysicsUtils.Angle(fwd, movementOrder.Heading);
                if (angle <= headingTolerance)
                {
                    movement.Acceleration =
                        math.normalize(fwd) * PhysicsUtils.GeesToMs2(movementOrder.AccelerationMagnitude);
                }

            }).Schedule(inputDeps);
        }
    }
}