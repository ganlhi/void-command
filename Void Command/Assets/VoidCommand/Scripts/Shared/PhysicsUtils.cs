using Unity.Mathematics;

namespace VoidCommand.Shared
{
    public struct PhysicalState
    {
        public float3 Position;
        public float3 Velocity;
        public float3 Acceleration;
    }
    
    public static class PhysicsUtils
    {
        public static float3 Displacement(float3 velocity, float3 acceleration, float deltaTime)
        {
            return 0.5f * math.pow(deltaTime, 2) * acceleration + deltaTime * velocity;
        }

        public static PhysicalState PredictFutureState(PhysicalState state, float deltaTime)
        {
            return new PhysicalState
            {
                Position = state.Position + Displacement(state.Velocity, state.Acceleration, deltaTime),
                Velocity = state.Velocity + deltaTime * state.Acceleration,
                Acceleration = state.Acceleration
            };
        }

        public static float ClosestApproach(PhysicalState subject, PhysicalState target, float timeIncrement = 1f)
        {
            var closestDistance = float.MaxValue;
                
            while (true)
            {
                var dist = math.distance(subject.Position, target.Position);

                if (dist >= closestDistance)
                {
                    // done, we will not get any closer
                    return closestDistance;
                }
                else
                {
                    subject = PredictFutureState(subject, timeIncrement);
                    target = PredictFutureState(target, timeIncrement);
                    closestDistance = dist;
                }
            }
        }

        public static float3 AccelerationToIntercept(PhysicalState subject, PhysicalState target, float accMag, float leadTimeIncrement = 1f)
        {
            var leadTime = 0f;
            var accToIntercept = float3.zero;
            var closestDistance = float.MaxValue;

            while (true)
            {
                var targetPoint = PredictFutureState(target, leadTime).Position;
                var acc = math.normalize(subject.Position - targetPoint) * accMag;

                var cd = ClosestApproach(
                    new PhysicalState()
                    {
                        Position = subject.Position,
                        Velocity = subject.Velocity,
                        Acceleration = acc
                    },
                    target
                );

                if (cd >= closestDistance)
                {
                    // done, we will not get any closer
                    return accToIntercept;
                }
                else
                {
                    accToIntercept = acc;
                    closestDistance = cd;
                    leadTime += leadTimeIncrement;
                }
            }
        }
        
        /// <summary>Returns the angle in degrees from 0 to 180 between two float3s.</summary>
        public static float Angle(float3 from, float3 to)
        {
            return math.degrees(math.acos(math.dot(math.normalize(from), math.normalize(to))));
        }
        
        /// <summary>Returns the signed angle in degrees from 180 to -180 between two float3s.</summary>
        public static float AngleSigned(float3 from, float3 to)
        {
            var angle = math.acos(math.dot(math.normalize(from), math.normalize(to)));
            var cross = math.cross(from, to);
            angle *= math.sign(math.dot(math.up(), cross));
            return math.degrees(angle);
        }

        public static float GeesToMs2(float gees)
        {
            return 9.80665f * gees;
        }
        
    }
}