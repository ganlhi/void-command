using System;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using VoidCommand.Components;
using VoidCommand.Shared;

namespace VoidCommand.Systems
{
    public class SteeringSystem : JobComponentSystem
    {
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var deltaTime = Time.DeltaTime;
            
            return Entities.ForEach((ref Rotation rotation, in MovementOrder movementOrder, in MovementCapabilities movementCapabilities) =>
            {
                if (math.length(movementOrder.Heading) == 0) return;

                var fwd = math.forward(rotation.Value);
                var angle = PhysicsUtils.AngleSigned(fwd, movementOrder.Heading);

                if (math.abs(angle) < float.Epsilon) return;
                
                var steerAngle = math.sign(angle) * math.radians(movementCapabilities.TurnRate * deltaTime);
                rotation.Value = math.mul(
                    math.normalize(rotation.Value),
                    quaternion.AxisAngle(math.up(), steerAngle )
                    );
            }).Schedule(inputDeps);
        }
    }
}