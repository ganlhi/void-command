using System;
using Unity.Entities;
using Unity.Mathematics;

namespace VoidCommand.Components
{
    [Serializable]
    [GenerateAuthoringComponent]
    public struct PredictMovement : IComponentData
    {
        public float3 Position;
        public float3 Velocity;
    }
}
