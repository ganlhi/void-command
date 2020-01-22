using System;
using Unity.Mathematics;
using UnityEngine;

namespace VoidCommand.Shared
{
    public struct PhysicalState
    {
        public float3 Position;
        public float3 Velocity;
        public float3 Acceleration;
    }
    
    [Serializable]
    public struct InterceptData
    {
        public float3 Acceleration;
        public float TimeToIntercept;
        public float ClosestDistance;
        public float3 SubjectPos;
        public float3 TargetPos;
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

        public static float2 ClosestApproach(PhysicalState subject, PhysicalState target, float timeIncrement = 1f, int maxSteps = 3600)
        {
            var closestDistance = math.distance(subject.Position, target.Position);
            var closestDistanceTime = 0f;
            var totalTime = 0f;
            var steps = 0;
            
            while (steps < maxSteps && closestDistance > float.Epsilon)
            {
                steps += 1;
                
                subject = PredictFutureState(subject, timeIncrement);
                target = PredictFutureState(target, timeIncrement);
                totalTime += timeIncrement;
                
                var dist = math.distance(subject.Position, target.Position);

                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    closestDistanceTime = totalTime;
                }
            }
            
            return new float2(closestDistance, closestDistanceTime);
        }

        public static InterceptData AccelerationToIntercept(PhysicalState subject, PhysicalState target, float accMag, float leadTimeIncrement = 1f, int maxSteps = 3600)
        {
            var leadTime = 0f;
            var steps = 0;

            var intercept = new InterceptData
            {
                Acceleration = float3.zero,
                ClosestDistance = math.distance(subject.Position, target.Position),
                TimeToIntercept = 0,
                SubjectPos = subject.Position,
                TargetPos = target.Position
            };
            
            while (steps < maxSteps && intercept.ClosestDistance > float.Epsilon)
            {
                steps += 1;
                
                var targetPoint = PredictFutureState(target, leadTime).Position;
                var acc = math.normalize(targetPoint - subject.Position) * accMag;

                var ca = ClosestApproach(
                    new PhysicalState()
                    {
                        Position = subject.Position,
                        Velocity = subject.Velocity,
                        Acceleration = acc
                    },
                    target
                );

                var cd = ca.x;
                var tti = ca.y;

                if (cd < intercept.ClosestDistance)
                {
                    intercept = new InterceptData
                    {
                        Acceleration = acc,
                        ClosestDistance = cd,
                        TimeToIntercept = tti,
                        SubjectPos = PredictFutureState(new PhysicalState()
                        {
                            Position = subject.Position,
                            Velocity = subject.Velocity,
                            Acceleration = acc
                        }, tti).Position,
                        TargetPos = PredictFutureState(target, tti).Position
                    };
                }
                
                leadTime += leadTimeIncrement;
            }

            return intercept;
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