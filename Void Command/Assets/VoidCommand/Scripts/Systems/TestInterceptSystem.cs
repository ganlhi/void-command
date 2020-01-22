using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using VoidCommand.Components;

namespace VoidCommand.Systems
{
    [DisableAutoCreation] 
    public class TestInterceptSystem : JobComponentSystem
    { 
        private struct EntityWithPosition
        {
            public Entity entity;
            public float3 position;
        }
        
        [RequireComponentTag(typeof(Tag_Player))]
        [ExcludeComponent(typeof(InterceptTarget))]
        private struct SetTargetJob : IJobForEachWithEntity<Position>
        {
            [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<EntityWithPosition> targets;
            public EntityCommandBuffer.Concurrent ecb;
            
            public void Execute(Entity entity, int index, [ReadOnly] ref Position position)
            {
                var target = Entity.Null;
                var targetDistance = float.MaxValue;

                foreach (var ewp in targets)
                {
                    var dist = math.distance(ewp.position, position.Value);
                    if (dist >= targetDistance) continue;
                    
                    targetDistance = dist;
                    target = ewp.entity;
                }
                
                if (target != Entity.Null)
                {
                    // ecb.AddComponent(index, entity, new TargetToIntercept {
                    //     Target = target,
                    //     DistanceToDisengage = 1000
                    // });
                }
            }
        }
        
        private EndSimulationEntityCommandBufferSystem endSimulationEcbSystem;
        
        protected override void OnCreate()
        {
            base.OnCreate();
            endSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }
        
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var query = GetEntityQuery(typeof(Tag_Ennemy), ComponentType.ReadOnly<Position>());
            var targetEntities = query.ToEntityArray(Allocator.TempJob);
            var targetPositions = query.ToComponentDataArray<Position>(Allocator.TempJob);

            var targets = new NativeArray<EntityWithPosition>(targetEntities.Length, Allocator.TempJob);
            
            for (var i = 0; i < targetEntities.Length; i++)
            {
                targets[i] = new EntityWithPosition()
                {
                    entity = targetEntities[i],
                    position = targetPositions[i].Value
                };
            }

            targetEntities.Dispose();
            targetPositions.Dispose();

            var job = new SetTargetJob
            {
                targets = targets,
                ecb = endSimulationEcbSystem.CreateCommandBuffer().ToConcurrent()
            };

            var handle = job.Schedule(this, inputDeps);
            endSimulationEcbSystem.AddJobHandleForProducer(handle);

            return handle;
        }
    }

}