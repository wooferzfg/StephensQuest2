using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawMap : MonoBehaviour
{
    public Sprite visited;
    public Sprite visitedOrb;
    public Sprite visitedAbility;
    public Sprite current;
    public Sprite currentOrb;
    public Sprite currentAbility;
    public Sprite rainbow;
    public GameObject room;
    public GameObject horizontalConnection;
    public GameObject verticalConnection;
    public bool canViewMap = false;

    private int gridWidth = 6;
    private int gridHeight = 6;
    private int curRow = 0;
    private int curColumn = 2;

    private Transform panel;
    private GameControl game;
    private Image orbIcon;
    private Text orbCount;
    private List<List<Image>> rooms;
    private List<List<RoomType>> roomTypes;
    private List<Connection> connections;

    private void Awake ()
    {
        panel = GameObject.Find("Panel").transform;
        game = GameObject.Find("Character").GetComponent<GameControl>();
        orbIcon = GameObject.Find("OrbIcon").GetComponent<Image>();
        orbCount = GameObject.Find("OrbCount").GetComponent<Text>();
        InitializeRooms();
    }

    private void Update()
    {
        if (canViewMap && !game.timerEnded)
        {
            if (Input.GetButtonDown("Map"))
            {
                ToggleMap(true);
            }
            else if (Input.GetButtonUp("Map"))
            {
                ToggleMap(false);
            }
        }
    } 

    private void InitializeRooms()
    {
        connections = new List<Connection>();
        rooms = new List<List<Image>>();
        roomTypes = new List<List<RoomType>>();
        for (var i = 0; i < gridHeight; i++)
        {
            var curRow = new List<Image>();
            var curRoomTypeRow = new List<RoomType>();
            for (var j = 0; j < gridWidth; j++)
            {
                var curRoom = Instantiate(room);
                curRoom.transform.SetParent(panel);
                var roomImage = curRoom.GetComponent<Image>();
                roomImage.rectTransform.localPosition = GetRoomPosition(i, j);
                roomImage.enabled = false;
                curRow.Add(roomImage);
                curRoomTypeRow.Add(RoomType.None);
            }
            rooms.Add(curRow);
            roomTypes.Add(curRoomTypeRow);
        }
        UpdateRoomSprite(curRow, curColumn, true);
    }

    public void ToggleMap(bool enabled)
    {
        for (var i = 0; i < gridHeight; i++)
        {
            for (var j = 0; j < gridWidth; j++)
            {
                rooms[i][j].enabled = enabled;
            }
        }
        foreach (var connection in connections)
        {
            connection.UIElement.enabled = enabled;
        }
        orbIcon.enabled = enabled;
        orbCount.enabled = enabled;
        panel.GetComponent<Image>().enabled = enabled;
    }

    private Vector3 GetRoomPosition(int row, int column)
    {
        return new Vector3(-487.5f + column * 75, 187.5f - row * 75);
    }

    private Vector3 GetConnectionPosition(Connection connection)
    {
        return Vector3.Lerp(GetRoomPosition(connection.StartRow, connection.StartColumn),
                            GetRoomPosition(connection.EndRow, connection.EndColumn),
                            0.5f);
    }

    private void AddNewVisit(int endRow, int endColumn)
    {
        if (endRow >= 0 && endRow < gridHeight && endColumn >= 0 && endColumn < gridWidth)
        {
            UpdateRoomSprite(curRow, curColumn, false);
            UpdateRoomSprite(endRow, endColumn, true);
            var newConnection = new Connection(curRow, curColumn, endRow, endColumn);
            if (!connections.Contains(newConnection))
            {
                connections.Add(newConnection);
                AddConnectionUI(newConnection);
            }
            curRow = endRow;
            curColumn = endColumn;
        }
    }

    private void UpdateRoomSprite(int row, int column, bool currentRoom)
    {
        var roomImage = rooms[row][column];
        switch(roomTypes[row][column])
        {
            case RoomType.None:
                roomImage.sprite = currentRoom ? current : visited;
                break;
            case RoomType.Orb:
                roomImage.sprite = currentRoom ? currentOrb : visitedOrb;
                break;
            case RoomType.Ability:
                roomImage.sprite = currentRoom ? currentAbility : visitedAbility;
                break;
            case RoomType.FinalOrb:
                roomImage.sprite = rainbow;
                break;
        }
    }

    private void AddConnectionUI(Connection newConnection)
    {
        Image connectionUI;
        if (newConnection.EndRow != newConnection.StartRow)
            connectionUI = Instantiate(verticalConnection).GetComponent<Image>();
        else
            connectionUI = Instantiate(horizontalConnection).GetComponent<Image>();
        connectionUI.transform.SetParent(panel);
        connectionUI.transform.SetAsFirstSibling();
        connectionUI.rectTransform.localPosition = GetConnectionPosition(newConnection);
        connectionUI.enabled = orbCount.enabled;
        newConnection.UIElement = connectionUI;
    }

    public void MoveUp()
    {
        int newRow = curRow - 1;
        AddNewVisit(newRow, curColumn);
    }

    public void MoveLeft()
    {
        int newColumn = curColumn - 1;
        AddNewVisit(curRow, newColumn);
    }

    public void MoveRight()
    {
        int newColumn = curColumn + 1;
        AddNewVisit(curRow, newColumn);
    }

    public void MoveDown()
    {
        int newRow = curRow + 1;
        AddNewVisit(newRow, curColumn);
    }

    public void Collected(RoomType item)
    {
        roomTypes[curRow][curColumn] = item;
        UpdateRoomSprite(curRow, curColumn, true);
    }
}

public class Connection
{
    public int StartRow { get; private set; }
    public int StartColumn { get; private set; }
    public int EndRow { get; private set; }
    public int EndColumn { get; private set; }
    public Image UIElement { get; set; }

    public Connection(int startRow, int startColumn, int endRow, int endColumn)
    {
        if (startRow < endRow || startColumn < endColumn)
        {
            StartRow = startRow;
            StartColumn = startColumn;
            EndRow = endRow;
            EndColumn = endColumn;
        }
        else
        {
            StartRow = endRow;
            StartColumn = endColumn;
            EndRow = startRow;
            EndColumn = startColumn;
        }
    }

    public override bool Equals(object obj)
    {
        var other = obj as Connection;
        if (other == null)
        {
            return false;
        }
        return StartRow == other.StartRow
            && StartColumn == other.StartColumn
            && EndRow == other.EndRow
            && EndColumn == other.EndColumn;
    }

    public override int GetHashCode()
    {
        var result = StartRow;
        result = (result * 397) ^ StartColumn;
        result = (result * 397) ^ EndRow;
        result = (result * 397) ^ EndColumn;
        return result;
    }
}

public enum RoomType
{
    None, Orb, Ability, FinalOrb
}
