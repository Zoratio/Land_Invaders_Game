using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CollisionHandler : MonoBehaviour
{
    [SerializeField] float levelLoadDelay = 1f;
    [SerializeField] GameObject deathFX;

    void OnTriggerEnter(Collider other)
    {
        StartDeathSequence();
        deathFX.SetActive(true);
    }

    private void StartDeathSequence()
    {
        SendMessage("OnPlayerDeath");
        Invoke("Respawn", levelLoadDelay);        
    }

    private void Respawn()  //PUT THIS METHOD IN SceneLoader AND JUST CALL IT FROM StartDeathSequence USING A COROUTINE - USE A SINGLETON FOR EASE OF FINDING
    {
        SceneManager.LoadScene(1);
    }
}
 