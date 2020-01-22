using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using VoidCommand.Components;

namespace VoidCommand.Systems
{
    [AlwaysSynchronizeSystem]
    public class RenderMovementLinesSystem: JobComponentSystem
    {
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            Entities.WithoutBurst().ForEach((Entity e, in Position position, in Rotation rotation, in PredictMovement predictMovement) =>
            {
                LinesManager.Instance.SetFuturePositionForEntity(e, position.Value, predictMovement.Position);
                LinesManager.Instance.SetHeadingForEntity(e, position.Value, math.forward(rotation.Value));
            }).Run();

            return default;
        }
    }
}