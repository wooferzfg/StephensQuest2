using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameControl : MonoBehaviour {

    public static bool gameStarted = false;
    public static bool newGamePlus = false;
    public static bool newGamePlusUnlocked = false;

    public int orbs = 0;
    public float startTime;
    public float endTime;
    public bool timerEnded = false;

    private Text timer;
    private Text finalTimer;
    private Image panel;
    private Text orbsCollected;
    private Image title;
    private Text startGame;
    private Text toggleMode;
    private CharacterControl player;

    void Awake()
    {
        timer = GameObject.Find("Timer").GetComponent<Text>();
        finalTimer = GameObject.Find("FinalTimer").GetComponent<Text>();
        panel = GameObject.Find("Panel").GetComponent<Image>();
        orbsCollected = GameObject.Find("OrbsGathered").GetComponent<Text>();
        player = GetComponent<CharacterControl>();
        title = GameObject.Find("Title").GetComponent<Image>();
        startGame = GameObject.Find("StartGame").GetComponent<Text>();
        toggleMode = GameObject.Find("ToggleMode").GetComponent<Text>();

        int ngPlusUnlock = PlayerPrefs.GetInt("NewGamePlusUnlocked", 0);
        if (ngPlusUnlock > 0)
        {
            newGamePlusUnlocked = true;
            toggleMode.text = "Press the Zip Button to Toggle Mode";
        }

        panel.color = new Color(0, 0, 0, 0.85f);

        if (gameStarted)
            StartGame();
    }

    void Update()
    {
        if (!gameStarted && Input.GetButtonDown("Jump"))
            StartGame();

        if (newGamePlusUnlocked && Input.GetButtonDown("Zip"))
            ToggleNewGamePlus();

        if (gameStarted)
        {
            if (Input.GetButtonDown("Restart"))
                SceneManager.LoadScene(0);

            float totalSeconds = 0;
            if (timerEnded)
                totalSeconds = endTime - startTime;
            else
                totalSeconds = Time.time - startTime;
            TimeSpan curTime = TimeSpan.FromSeconds(totalSeconds);
            string timeString = string.Format("{0:00}:{1:00}.{2:000}", curTime.Minutes, curTime.Seconds, curTime.Milliseconds);

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

    private void ToggleNewGamePlus()
    {
        if (timerEnded)
        {
            newGamePlus = !newGamePlus;
            if (newGamePlus)
                startGame.text = "Press the Restart Button to Start a New Game+";
            else
                startGame.text = "Press the Restart Button to Start a New Game";
        }
        else if (!gameStarted)
        {
            newGamePlus = !newGamePlus;
            if (newGamePlus)
                startGame.text = "Press the Jump Button to Start a New Game+";
            else
                startGame.text = "Press the Jump Button to Start a New Game";
        }
    }

    private void StartGame()
    {
        gameStarted = true;
        player.hasControl = true;
        startTime = Time.time;
        panel.color = new Color(0, 0, 0, 0f);
        title.color = new Color(0, 0, 0, 0f);
        startGame.text = "";
        toggleMode.text = "";

        if (newGamePlus)
            player.SetNewGamePlus();
    }

    public void CollectOrb()
    {
        orbs++;
    }

    public void CollectFinalOrb()
    {
        endTime = Time.time;
        timerEnded = true;
        player.hasControl = false;
        panel.color = new Color(0, 0, 0, 0.85f);

        if (newGamePlus)
            startGame.text = "Press the Restart Button to Start a New Game+!";
        else
            startGame.text = "Press the Restart Button to Start a New Game!";

        toggleMode.text = "Press the Zip Button to Toggle Mode";

        if (orbs == 4)
            orbsCollected.text = "All Orbs Collected";

        newGamePlusUnlocked = true;
        PlayerPrefs.SetInt("NewGamePlusUnlocked", 1);
    }
}
