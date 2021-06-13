using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelBehaviour : MonoBehaviour
{
    [Header("Monkeys")]
    [SerializeField]
    int maxMonkeys = 10;
    [SerializeField]
    float percentToSucceed = .1f;

    [Header("Pause")]
    private int leftMonkeys = 10;
    private int spawnMonkeys = 0;
    private int passedMonkeys = 0;
    private int minToWin;
    private int currentScene;

    public static LevelBehaviour instance { get; private set; } = null;

    public int LeftMonkeys => leftMonkeys;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
            return;
        }
        else
        {
            instance = this;
        }

        leftMonkeys = maxMonkeys;
        minToWin = (int)Math.Round(maxMonkeys * percentToSucceed);
        currentScene = SceneManager.GetActiveScene().buildIndex;
    }

    void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (leftMonkeys == 0 && spawnMonkeys == 0)
            LevelFinished();
    }

    public void MonkeyFinish()
    {
        spawnMonkeys--;
        passedMonkeys++;
    }

    public void SpawnMonkey()
    {
        if(leftMonkeys != 0)
        {
            leftMonkeys--;
            spawnMonkeys++;
        }
    }

    private void LevelFinished()
    {
        if(passedMonkeys >= minToWin)
        {
            SceneManager.LoadScene(currentScene++);
        }
        else
        {
            SceneManager.LoadScene(currentScene);
        }
    }
}
