using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawMap : MonoBehaviour
{
    public Sprite visited;
    public Sprite current;
    public GameObject room;
    public GameObject horizontalConnection;
    public GameObject verticalConnection;
    public bool canViewMap = false;

    private Transform panel;
    private GameControl game;
    private Image orbIcon;
    private Text orbCount;
	private List<List<Image>> rooms;
	private List<Connection> connections;
    private int gridWidth = 6;
    private int gridHeight = 6;
    private int curRow = 0;
    private int curColumn = 2;

	void Awake ()
    {
        panel = GameObject.Find("Panel").transform;
        game = GameObject.Find("Character").GetComponent<GameControl>();
        orbIcon = GameObject.Find("OrbIcon").GetComponent<Image>();
        orbCount = GameObject.Find("OrbCount").GetComponent<Text>();
        InitializeRooms();
	}

    void Update()
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
        for (var i = 0; i < gridHeight; i++)
        {
            var curRow = new List<Image>();
            for (var j = 0; j < gridWidth; j++)
            {
                var curRoom = Instantiate(room);
                curRoom.transform.SetParent(panel);
                var roomImage = curRoom.GetComponent<Image>();
                roomImage.rectTransform.localPosition = GetRoomPosition(i, j);
                roomImage.enabled = false;
                curRow.Add(roomImage);
            }
            rooms.Add(curRow);
        }
        rooms[curRow][curColumn].sprite = current;
    }

    private void ToggleMap(bool enabled)
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
        rooms[curRow][curColumn].sprite = visited;
        rooms[endRow][endColumn].sprite = current;
        var newConnection = new Connection(curRow, curColumn, endRow, endColumn);
        if (!connections.Contains(newConnection))
        {
            connections.Add(newConnection);
            AddConnectionUI(newConnection);
        }
        curRow = endRow;
        curColumn = endColumn;
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
		StartRow = startRow;
		StartColumn = startColumn;
		EndRow = endRow;
		EndColumn = endColumn;
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
}
