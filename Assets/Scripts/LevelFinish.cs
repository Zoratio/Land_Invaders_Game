using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelFinish : MonoBehaviour
{
    private void Start()
    {
        Invoke("NextScene", 68);
    }

    private void NextScene()
    {
        GameObject go = GameObject.Find("Score Board");
        ScoreBoard sb = go.GetComponent<ScoreBoard>();
        PlayerPrefs.SetInt("FinalScore", sb.score);
        SceneManager.LoadScene(2);
    }
}
