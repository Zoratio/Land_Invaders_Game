using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FinalScore : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Text txt = GetComponent<Text>();
        txt.text = PlayerPrefs.GetInt("FinalScore").ToString();
        PlayerPrefs.SetInt("FinalScore", 0);
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
