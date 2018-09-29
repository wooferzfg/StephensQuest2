﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingSpikes : MonoBehaviour
{
    public float cycle;
    public List<Vector2> nodes;
    public List<float> times;

    private Vector2 currentRoom;
    private float totalTime;

    private CameraMovement cameraMovement;

    private void Awake()
    {
        cameraMovement = GameObject.Find("Main Camera").GetComponent<CameraMovement>();
        SetTotalTime();
    }

    private void Start ()
    {
        currentRoom = CreateCheckpoint.GetNearestRoom(transform.position);
    }

    private void Update()
    {
        if (nodes.Count > 1)
        {
            SetCycle();
            SetPosition();
        }    
	}

    private void SetTotalTime()
    {
        totalTime = 0;
        for (var i = 0; i < times.Count; i++)
        {
            totalTime += times[i];
        }
    }

    private void SetCycle()
    {
        var cameraTarget = cameraMovement.moveTarget;
        var cameraOrigin = cameraMovement.moveOrigin;
        if (cameraMovement.isMoving && !VectorsEqual(cameraOrigin, currentRoom))
        {
            cycle = 0;
        }
        else if (!cameraMovement.isMoving && VectorsEqual(cameraTarget, currentRoom))
        {
            cycle = cameraMovement.cycle;
        }
    }

    private void SetPosition()
    {
        var i = 0;
        var timeRemaining = cycle % totalTime;
        while (timeRemaining >= times[i])
        {
            timeRemaining -= times[i];
            i++;
        }

        Vector2 nextNode;
        if (i == nodes.Count - 1)
            nextNode = nodes[0];
        else
            nextNode = nodes[i + 1];

        transform.position = Vector3.Lerp(nodes[i], nextNode, timeRemaining);
    }

    private void OnDrawGizmos()
    {
        if (nodes.Count > 0)
        {
            var firstNode = nodes[0];
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(firstNode, 0.125f);
            var prevNode = firstNode;
            for (var i = 1; i < nodes.Count; i++)
            {
                var curNode = nodes[i];
                Gizmos.DrawSphere(curNode, 0.125f);
                Gizmos.DrawLine(prevNode, curNode);
                prevNode = curNode;
            }
            Gizmos.DrawLine(prevNode, firstNode);
        }
    }

    private bool VectorsEqual(Vector2 a, Vector2 b)
    {
        return (int)a.x == (int)b.x
            && (int)a.y == (int)b.y;
    }
}