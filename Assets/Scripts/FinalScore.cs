using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FinalScore : MonoBehaviour
{
    Text txt;
    // Start is called before the first frame update
    void Start()
    {
        txt = GetComponent<Text>();
        txt.text = PlayerPrefs.GetInt("FinalScore").ToString();
        PlayerPrefs.SetInt("FinalScore", 0);
    }

}
