using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] int scorePoint = 100;

    GameSession gameSession;
    bool isTaken = false;
    //int finalScore = 0;
    // Start is called before the first frame update

    private void Awake()
    {
        gameSession = FindObjectOfType<GameSession>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && !isTaken)
        {
            //finalScore += scorePoint;
            gameSession.AddScore(scorePoint);
            gameObject.SetActive(false);
            Destroy(gameObject);
            isTaken = true;
        }
        
    }
}
