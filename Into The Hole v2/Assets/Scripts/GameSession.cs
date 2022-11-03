using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameSession : MonoBehaviour
{
    Player player;
    Coin coin;
    [SerializeField] int playerLives = 3;
    [SerializeField] TextMeshProUGUI livesText;
    [SerializeField] TextMeshProUGUI scoreText;

    int coinScore = 0;

    //int finalScore;
    void Awake()
    {   
        player = FindObjectOfType<Player>();
        coin = FindObjectOfType<Coin>();
        int numGameSessions = FindObjectsOfType<GameSession>().Length;
        if(numGameSessions > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
        
    }

    void Start()
    {
        livesText.text = playerLives.ToString();
        scoreText.text = coinScore.ToString();
    }

  

    public void ProcessPlayerDeath()
    {
        if (playerLives > 0)
        {
            TakeLife();
        }
        else
        {
            player.SetAlive();
            ResetGameSession();
        }
    }
    void TakeLife()
    {
        playerLives--;
        livesText.text = playerLives.ToString();
        //player.SendFlying();
    }
    void ResetGameSession()
    {
        //Debug.Log("game reset");
        FindObjectOfType<ScenePersist>().ResetScenePersist();
        SceneManager.LoadScene(0);
        Destroy(gameObject);
    }

    public void AddScore (int score)
    {
        coinScore += score;
        scoreText.text = coinScore.ToString();
    }
}
