using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateCheckpoint : MonoBehaviour
{
    public bool fromTop;
    public bool fromLeft;
    public bool fromRight;
    public bool fromBottom;

    private static Dictionary<CheckpointKey, Vector2> checkpoints;
    private static CameraMovement cameraMovement;

    private void Awake()
    {
        checkpoints = new Dictionary<CheckpointKey, Vector2>();
        GetComponent<SpriteRenderer>().sprite = null;
        if (cameraMovement == null)
            cameraMovement = GameObject.Find("Main Camera").GetComponent<CameraMovement>();
    }

    private void Start()
    {
        AddCheckpoint();
    }

    private void AddCheckpoint()
    {
        var targetPosition = GetNearestRoom(transform.position);

        var screenWidth = 2 * cameraMovement.cameraWidth;
        var screenHeight = 2 * cameraMovement.cameraHeight;
        var originPosition = targetPosition;
        if (fromTop)
            originPosition += new Vector2(0, screenHeight);
        else if (fromLeft)
            originPosition += new Vector2(-screenWidth, 0);
        else if (fromRight)
            originPosition += new Vector2(screenWidth, 0);
        else if (fromBottom)
            originPosition += new Vector2(0, -screenHeight);

        var key = new CheckpointKey(targetPosition.x, targetPosition.y, originPosition.x, originPosition.y);
        checkpoints.Add(key, transform.position);
    }

    public static Vector2 GetNearestRoom(Vector2 position)
    {
        var screenWidth = 2 * cameraMovement.cameraWidth;
        var screenHeight = 2 * cameraMovement.cameraHeight;
        var xPos = Mathf.Round(position.x / screenWidth) * screenWidth;
        var yPos = Mathf.Round(position.y / screenHeight) * screenHeight;
        return new Vector2(xPos, yPos);
    }

    public static Vector2 GetCurrentCheckpoint(float targetX, float targetY, float originX, float originY)
    {
        var key = new CheckpointKey(targetX, targetY, originX, originY);
        return checkpoints[key];
    }
}

public class CheckpointKey
{
    public int TargetX { get; private set; }
    public int TargetY { get; private set; }
    public int OriginX { get; private set; }
    public int OriginY { get; private set; }

    public CheckpointKey(float targetX, float targetY, float originX, float originY)
    {
        TargetX = (int)Mathf.Round(targetX);
        TargetY = (int)Mathf.Round(targetY);
        OriginX = (int)Mathf.Round(originX);
        OriginY = (int)Mathf.Round(originY);
    }

    public override bool Equals(object obj)
    {
        var other = obj as CheckpointKey;
        if (other == null)
        {
            return false;
        }
        return TargetX == other.TargetX
            && TargetY == other.TargetY
            && OriginX == other.OriginX
            && OriginY == other.OriginY;
    }

    public override int GetHashCode()
    {
        var result = TargetX;
        result = (result * 397) ^ TargetY;
        result = (result * 397) ^ OriginX;
        result = (result * 397) ^ OriginY;
        return result;
    }
}
