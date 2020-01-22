using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using VoidCommand.Components;
using VoidCommand.Shared;

namespace VoidCommand.Systems
{
    public class InterceptTargetSystem : JobComponentSystem
    {
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var handle = Entities.ForEach((ref MovementOrder movementOrder,
                in InterceptTarget interceptTarget, in Position position, in Movement movement,
                in MovementCapabilities movementCapabilities) =>
            {
                var intercept = PhysicsUtils.AccelerationToIntercept(
                    new PhysicalState {
                        Position = position.Value,
                        Velocity = movement.Velocity,
                        Acceleration = movement.Acceleration,
                    },
                    new PhysicalState {
                        Position = interceptTarget.Position,
                        Velocity = interceptTarget.Velocity,
                        Acceleration = interceptTarget.Acceleration,
                    },
                    PhysicsUtils.GeesToMs2(movementCapabilities.MaxAccelerationMagnitude)
                );
                        
                movementOrder.Heading = math.normalize(intercept.Acceleration);
                movementOrder.AccelerationMagnitude = movementCapabilities.MaxAccelerationMagnitude;
            }).Schedule(inputDeps);

            return handle;
        }
    }
    
    /*
    public class InterceptTargetSystem : JobComponentSystem
    {
        private struct EntityWithPositionAndMovement
        {
            public Entity entity;
            public float3 position;
            public float3 velocity;
            public float3 acceleration;
        }
        
        EndSimulationEntityCommandBufferSystem endSimulationEcbSystem;
        protected override void OnCreate()
        {
            base.OnCreate();
            
            endSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var query = GetEntityQuery(ComponentType.ReadOnly<Position>(), ComponentType.ReadOnly<Movement>());
            var targetEntities = query.ToEntityArray(Allocator.TempJob);
            var targetPositions = query.ToComponentDataArray<Position>(Allocator.TempJob);
            var targetMovements = query.ToComponentDataArray<Movement>(Allocator.TempJob);
            
            var targets = new NativeArray<EntityWithPositionAndMovement>(targetEntities.Length, Allocator.TempJob);
            
            for (var i = 0; i < targetEntities.Length; i++)
            {
                targets[i] = new EntityWithPositionAndMovement()
                {
                    entity = targetEntities[i],
                    position = targetPositions[i].Value,
                    velocity = targetMovements[i].Velocity,
                    acceleration = targetMovements[i].Acceleration,
                };
            }

            targetEntities.Dispose();
            targetPositions.Dispose();
            targetMovements.Dispose();
            
            var ecb = endSimulationEcbSystem.CreateCommandBuffer().ToConcurrent();

            var handle = Entities.ForEach((Entity entity, int entityInQueryIndex, ref MovementOrder movementOrder,
                in InterceptTarget targetToIntercept, in Position position, in Movement movement,
                in MovementCapabilities movementCapabilities) =>
            {
                foreach (var target in targets)
                {
                    if (target.entity == targetToIntercept.Target)
                    {
                        if (math.distance(target.position, position.Value) < targetToIntercept.DistanceToDisengage) 
                        {
                            movementOrder.Heading = float3.zero;
                            movementOrder.AccelerationMagnitude = 0;
                            ecb.RemoveComponent<InterceptTarget>(entityInQueryIndex, entity);
                        }
                        else 
                        {
                            var interceptVector = PhysicsUtils.AccelerationToIntercept(
                                new PhysicalState {
                                    Position = position.Value,
                                    Velocity = movement.Velocity,
                                    Acceleration = movement.Acceleration,
                                },
                                new PhysicalState {
                                    Position = target.position,
                                    Velocity = target.velocity,
                                    Acceleration = target.acceleration,
                                },
                                PhysicsUtils.GeesToMs2(movementCapabilities.MaxAccelerationMagnitude)
                            );
                        
                            movementOrder.Heading = math.normalize(interceptVector);
                            movementOrder.AccelerationMagnitude = movementCapabilities.MaxAccelerationMagnitude;
                        }
                    }
                }
            }).Schedule(inputDeps);

            targets.Dispose();
            endSimulationEcbSystem.AddJobHandleForProducer(handle);
            return handle;
        }
    }
    */
}