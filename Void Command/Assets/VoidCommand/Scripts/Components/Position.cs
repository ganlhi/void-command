using System;
using Unity.Entities;
using Unity.Mathematics;

namespace VoidCommand.Components
{
    [Serializable]
    [GenerateAuthoringComponent]
    public struct Position : IComponentData
    {
        public float3 Value;
    }
}
