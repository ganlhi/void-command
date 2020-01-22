using System;
using Unity.Mathematics;
using UnityEngine;

namespace VoidCommand
{
    [Serializable]
    [CreateAssetMenu]
    public class SimulationSettings: ScriptableObject
    {
        [Tooltip("Real seconds per game second")]
        public float timeScale;

        public float kmPerWorldUnit;

        [Tooltip("How many seconds in the future to predict movements")]
        public float predictionsLeadTime;

        [Tooltip("How many difference between actual and desired heading is acceptable to allow thrust (degrees)")]
        public float headingToleranceToAccelerate;

        public static SimulationSettings Instance;

        public SimulationSettings()
        {
            if (Instance != null)
            {
                Destroy(this);
            }

            Instance = this;
        }
    }
}