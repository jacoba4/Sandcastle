using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GamePaused = false;
    public GameObject pauseMenuUI;
    public GameObject gameplayManager;
    GameplayManager Manager;

    private void Start()
    {
        gameplayManager = GameObject.FindGameObjectWithTag("Gameplaymanager");
        Manager = gameplayManager.GetComponent<GameplayManager>();
        Resume();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GamePaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GamePaused = false;
        foreach (PlayerControl players in Manager.players)
        {
            if (players != null)
            {
                players.EnablePlayerMovement();
            }
        }
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GamePaused = true;
        foreach (PlayerControl players in Manager.players)
        {
            if (players != null)
            {
                players.DisablePlayerMovement();
            }
        }
    }

    public void MainMenu ()
    {
        print(Manager);
        foreach (PlayerControl players in Manager.players)
        {
            if (players != null)
            {
                players.EnablePlayerMovement();
            }
        }
        SceneManager.LoadScene(0);
    }
}
