using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using VoidCommand;
using VoidCommand.Shared;

public class LinesManager : Singleton<LinesManager>
{
    #pragma warning disable 0649 
    [SerializeField] private LineRenderer prefab;
    #pragma warning restore 0649
    
    private readonly Dictionary<Entity, LineRenderer> futurePositionLines = new Dictionary<Entity, LineRenderer>();
    private readonly Dictionary<Entity, LineRenderer> headingLines = new Dictionary<Entity, LineRenderer>();
    private readonly Dictionary<Entity, LineRenderer> targetAccelerationLines = new Dictionary<Entity, LineRenderer>();

    private Dictionary<IDictionary<Entity, LineRenderer>, Color> colors;

    private void Start()
    {
        colors = new Dictionary<IDictionary<Entity, LineRenderer>, Color>()
        {
            {futurePositionLines, Color.white},
            {headingLines, Color.yellow},
            {targetAccelerationLines, Color.magenta},
        };
    }

    public void SetFuturePositionForEntity(Entity entity, float3 position, float3 futurePosition)
    {
        var posRenderer = EnsureRendererForEntity(entity, futurePositionLines);
        
        posRenderer.SetPosition(0, PhysicsToWorldVector(position));
        posRenderer.SetPosition(1, PhysicsToWorldVector(futurePosition));
    }

    public void SetHeadingForEntity(Entity entity, float3 position, float3 acceleration)
    {
        var accRenderer = EnsureRendererForEntity(entity, headingLines);
        
        accRenderer.SetPosition(0, PhysicsToWorldVector(position));
        accRenderer.SetPosition(1, PhysicsToWorldVector(position) + PhysicsToWorldVector(acceleration).normalized);
    }

    public void SetTargetAccelerationForEntity(Entity entity, float3 position, float3 acceleration)
    {
        var tgtAccRenderer = EnsureRendererForEntity(entity, targetAccelerationLines);
        
        tgtAccRenderer.SetPosition(0, PhysicsToWorldVector(position));
        tgtAccRenderer.SetPosition(1, PhysicsToWorldVector(position) + PhysicsToWorldVector(acceleration).normalized);
    }

    private LineRenderer EnsureRendererForEntity(Entity entity, IDictionary<Entity, LineRenderer> lines)
    {
        LineRenderer lr;
        if (lines.ContainsKey(entity))
        {
            lr = lines[entity];
        }
        else
        {
            var go = Instantiate(prefab, transform);
            lr = go.GetComponent<LineRenderer>();
            go.name = "PredictMovement_" + entity.Index;
            lines.Add(entity, lr);

            var color = colors[lines];
            lr.startColor = color;
            lr.endColor = color;
        }

        return lr;
    }

    private static Vector3 PhysicsToWorldVector(float3 v)
    {
        var vw = v * 1f / (1000f * SimulationSettings.Instance.kmPerWorldUnit);
        return new Vector3(vw.x, vw.y, vw.z);
    } 
}
