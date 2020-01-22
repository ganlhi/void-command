using System;
using Unity.Entities;
using Unity.Mathematics;

namespace VoidCommand.Components
{
    [Serializable]
    [GenerateAuthoringComponent]
    public struct Movement : IComponentData
    {
        public float3 Velocity;
        public float3 Acceleration;
    }
}
