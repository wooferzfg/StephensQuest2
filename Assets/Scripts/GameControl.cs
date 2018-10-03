using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameControl : MonoBehaviour
{
    public bool gameStarted = false;

    public int orbs = 0;
    public float startTime;
    public float endTime;
    public bool timerEnded = false;

    private int totalOrbs = 20;

    private Text timer;
    private Text finalTimer;
    private Image panel;
    private Image title;
    private Text startGame;
    private Text orbCount;
    private CharacterControl player;
    private DrawMap map;

    private void Awake()
    {
        timer = GameObject.Find("Timer").GetComponent<Text>();
        finalTimer = GameObject.Find("FinalTimer").GetComponent<Text>();
        panel = GameObject.Find("Panel").GetComponent<Image>();
        player = GetComponent<CharacterControl>();
        title = GameObject.Find("Title").GetComponent<Image>();
        startGame = GameObject.Find("StartGame").GetComponent<Text>();
        orbCount = GameObject.Find("OrbCount").GetComponent<Text>();
        map = GameObject.Find("Main Camera").GetComponent<DrawMap>();

        UpdateOrbText();
    }

    private void Update()
    {
        if (!gameStarted && Input.GetButtonDown("Jump"))
            StartGame();

        if (gameStarted)
        {
            if (Input.GetButtonDown("Restart"))
                SceneManager.LoadScene(0);

            var totalSeconds = 0f;
            if (timerEnded)
                totalSeconds = endTime - startTime;
            else
                totalSeconds = Time.fixedTime - startTime;
            var curTime = TimeSpan.FromSeconds(totalSeconds);
            var timeString = string.Format("{0:00}:{1:00}.{2:000}", curTime.Minutes, curTime.Seconds, curTime.Milliseconds);

            if (timerEnded)
            {
                finalTimer.text = timeString;
                timer.text = "";
            }
            else
            {
                finalTimer.text = "";
                timer.text = timeString;
            }
        }
    }

    private void StartGame()
    {
        gameStarted = true;
        player.hasControl = true;
        startTime = Time.fixedTime;
        panel.enabled = false;
        title.enabled = false;
        startGame.text = "";
    }

    public void CollectOrb()
    {
        orbs++;
        UpdateOrbText();
        map.Collected(RoomType.Orb);
    }
    
    private void UpdateOrbText()
    {
        orbCount.text = orbs.ToString("D2") + "/" + totalOrbs;
    }

    public void CollectFinalOrb()
    {
        endTime = Time.time;
        timerEnded = true;
        player.hasControl = false;
        map.Collected(RoomType.FinalOrb);
        map.ToggleMap(true);
        panel.enabled = true;

        startGame.text = "Press the Restart Button to Start a New Game!";
    }
}
