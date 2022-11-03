using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    [SerializeField] float changeDelay = 1f;
    void OnTriggerEnter2D(Collider2D collision)
    {
        StartCoroutine(ChangeCurrentScene());
    }
    
    IEnumerator ChangeCurrentScene()
    {
        yield return new WaitForSecondsRealtime(changeDelay);
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        if(currentSceneIndex == SceneManager.sceneCountInBuildSettings)
        {
            nextSceneIndex = 0;
        }
        FindObjectOfType<ScenePersist>().ResetScenePersist();
        SceneManager.LoadScene(nextSceneIndex);

    }
    
}
