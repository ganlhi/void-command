using System;
using Unity.Entities;
using Unity.Mathematics;

namespace VoidCommand.Components
{
    [Serializable]
    [GenerateAuthoringComponent]
    public struct MovementCapabilities : IComponentData
    {
        /**
         * Max rotation speed, in degrees per second
         */
        public float TurnRate;
        
        /**
         * Maximum acceleration value, in g (e.g. 1 g which is about 9.81 m.s²) 
         */
        public float MaxAccelerationMagnitude;
    }
}
