using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Collider bCollider;
    [SerializeField] GameObject deathFX;
    [SerializeField] Transform parent;
    [SerializeField] int scorePerHit = 15;  //edit this number in inspector to be the correct score wanted for that particular enemy
    [SerializeField] int hits = 8;   //could change this later to be health instead which would allow me to have weapons that do different amounts of damage
        

    ScoreBoard scoreBoard;


    // Start is called before the first frame update
    void Start()
    {
        AddNonTriggerBoxCollider();
        scoreBoard = FindObjectOfType<ScoreBoard>();    //find the reference to the scoreboard in the game (its on the canvas text gameobject)
    }

    private void AddNonTriggerBoxCollider()
    {
        bCollider = gameObject.AddComponent<BoxCollider>();
        bCollider.isTrigger = false;
    }

    void OnParticleCollision(GameObject other)
    {
        scoreBoard.ScoreHit(scorePerHit);
        hits--;
        if (hits <= 0)
        {
            KillEnemy();
        }
    }

    private void KillEnemy()
    {
        GameObject fx = Instantiate(deathFX, transform.position, Quaternion.identity);
        fx.transform.parent = parent;
        Destroy(gameObject);
    }
}
