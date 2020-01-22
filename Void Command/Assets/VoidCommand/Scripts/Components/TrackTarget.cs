using System;
using Unity.Entities;

namespace VoidCommand.Components
{
    [Serializable]
    public struct TrackTarget : IComponentData
    {
        public Entity Target;
        public float DistanceToDisengage;
    }
}
