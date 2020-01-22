using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using VoidCommand.Shared;

namespace VoidCommand.Testing
{
    [Serializable]
    public struct ClosestApproachResult
    {
        public float distance;
        public float tti;
        public float3 subjectPosition;
        public float3 targetPosition;
        public float distCheck;
    }
    
    public class DebugSceneController : MonoBehaviour
    {
        public Transform Subject;
        public Transform Target;
        public Text Text;

        public float maxAccMag = 1;
        public float3 subjectVelocity; 
        public float3 subjectAcceleration;
        public float3 targetVelocity; 
        public float3 targetAcceleration;

        public List<InterceptData> intercepts = new List<InterceptData>();
        public List<ClosestApproachResult> closestApproaches = new List<ClosestApproachResult>();

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.KeypadPlus))
            {
                var maxSteps = closestApproaches.Count + 1;
                var ca = PhysicsUtils.ClosestApproach(
                    new PhysicalState
                    {
                        Position = fromVector3(Subject.position),
                        Velocity = subjectVelocity,
                        Acceleration = subjectAcceleration
                    },
                    new PhysicalState
                    {
                        Position = fromVector3(Target.position),
                        Velocity = targetVelocity,
                        Acceleration = targetAcceleration
                    },
                    1.0f,
                    maxSteps
                    );

                var dist = ca.x;
                var tti = ca.y;
                
                var fSubject = PhysicsUtils.PredictFutureState(new PhysicalState()
                {
                    Position = Subject.position,
                    Velocity = subjectVelocity,
                    Acceleration = subjectAcceleration
                }, tti).Position;
                
                var fTarget = PhysicsUtils.PredictFutureState(new PhysicalState()
                {
                    Position = Target.position,
                    Velocity = targetVelocity,
                    Acceleration = targetAcceleration
                }, tti).Position;

                var diffDist = math.distance(fSubject, fTarget);
                
                closestApproaches.Add(new ClosestApproachResult()
                {
                    distance = dist,
                    tti = tti,
                    subjectPosition = fSubject,
                    targetPosition = fTarget,
                    distCheck = diffDist
                });
                
                Text.text += "\nCD: " + dist + " / DD: " + diffDist;
            }
            
            if (Input.GetKeyDown(KeyCode.Space))
            {
                var lastTti = -1f;
                for (var maxSteps = 1; maxSteps < 10; maxSteps++) {
                
                    var intercept = PhysicsUtils.AccelerationToIntercept(
                        new PhysicalState
                        {
                            Position = fromVector3(Subject.position),
                            Velocity = subjectVelocity,
                            Acceleration = subjectAcceleration
                        },
                        new PhysicalState
                        {
                            Position = fromVector3(Target.position),
                            Velocity = targetVelocity,
                            Acceleration = targetAcceleration
                        },
                        maxAccMag,
                        1.0f,
                        maxSteps
                    );

                    // if (intercept.TimeToIntercept - lastTti < float.Epsilon) break;
                    lastTti = intercept.TimeToIntercept;
                    
                    intercepts.Add(intercept);
                }
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                intercepts.Clear();
                closestApproaches.Clear();
                Text.text = "";
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawLine(Subject.position, Subject.position + toVector3(subjectVelocity));
            Gizmos.DrawLine(Target.position, Target.position + toVector3(targetVelocity));
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(Subject.position, Subject.position + toVector3(subjectAcceleration));
            Gizmos.DrawLine(Target.position, Target.position + toVector3(targetAcceleration));

            
            closestApproaches.ForEach((ca) =>
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(Subject.position, ca.subjectPosition);
                Gizmos.DrawLine(Target.position, ca.targetPosition);
            });
            
            intercepts.ForEach((d) =>
            {
                Gizmos.color = Color.cyan;    
                Gizmos.DrawLine(Subject.position, toVector3(d.SubjectPos));
                Gizmos.DrawLine(Target.position, toVector3(d.TargetPos));

                Gizmos.color = Color.magenta;
                Gizmos.DrawLine(Subject.position, Subject.position + toVector3(d.Acceleration).normalized * 10);
            });
        }

        private static Vector3 toVector3(float3 v)
        {
            return new Vector3(v.x, v.y, v.z);
        }
        
        private static float3 fromVector3(Vector3 v)
        {
            return new float3(v.x, v.y, v.z);
        }
    }
}
