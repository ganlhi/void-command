using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using VoidCommand.Components;

namespace VoidCommand.Systems
{
    public class PhysicsToWorldSystem: JobComponentSystem
    {
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var mPerUnit = SimulationSettings.Instance.kmPerWorldUnit * 1000f;

            return Entities.ForEach((ref Translation translation, in Position position) =>
                {
                    translation.Value = position.Value * 1f / mPerUnit;
                }).Schedule(inputDeps);
        }
    }
}