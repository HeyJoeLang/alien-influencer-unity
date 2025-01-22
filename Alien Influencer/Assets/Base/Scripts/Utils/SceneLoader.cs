using System.Collections;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{

    /// <summary>
    ///     A simple scene loader that transitions between screens.
    /// </summary>
    
    [Header("References")]
    public Animator animator;
    public CoinManager coinManager;
    string[] sceneNames = new string[] { 
        "MainMenu",
        "Level 1" 
    };


    public void QuitApplication()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void PlayGame()
    {
        if(coinManager)
        {
            coinManager.RemoveCredit();
        }
        StartCoroutine(C_SwitchScene(1));
    }

    public void MainMenu()
    {
       StartCoroutine(C_SwitchScene(0));
    }


    private IEnumerator C_SwitchScene(int sceneIndex)
    {
        if (animator)
        {
            animator.gameObject.SetActive(true);
            animator.SetTrigger("StartFadeOut");
            yield return new WaitForSeconds(1f);
        }
        SceneManager.LoadScene(sceneIndex);
    }
}
