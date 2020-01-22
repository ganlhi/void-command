using Unity.Entities;
using Unity.Jobs;
using VoidCommand.Components;

namespace VoidCommand.Systems
{
    public class MovementSystem : JobComponentSystem
    {
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var deltaTime = Time.DeltaTime;
            
            return Entities.ForEach((ref Position position, ref Movement movement) =>
            {
                position.Value += movement.Velocity * deltaTime;
                movement.Velocity += movement.Acceleration * deltaTime;
            }).Schedule(inputDeps);
        }
    }
}