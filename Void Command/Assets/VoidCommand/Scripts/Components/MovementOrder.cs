using System;
using Unity.Entities;
using Unity.Mathematics;

namespace VoidCommand.Components
{
    [Serializable]
    [GenerateAuthoringComponent]
    public struct MovementOrder : IComponentData
    {
        public float3 Heading;
        
        /**
         * Desired acceleration value, in g (e.g. 1 g which is about 9.81 m.s²) 
         */
        public float AccelerationMagnitude;
    }
}
