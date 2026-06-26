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
    
    [Header("Preloading Settings")]
    public bool preloadOnStart = true;
    public int[] scenesToPreload = new int[] { 1 }; // Preload Level 1 by default
    
    string[] sceneNames = new string[] { 
        "MainMenu",
        "Game" 
    };

    private AsyncOperation[] preloadedScenes;

    private void Start()
    {
        if (preloadOnStart && scenesToPreload.Length > 0)
        {
            StartCoroutine(C_PreloadScenes());
        }
    }

    private IEnumerator C_PreloadScenes()
    {
        preloadedScenes = new AsyncOperation[scenesToPreload.Length];
        
        for (int i = 0; i < scenesToPreload.Length; i++)
        {
            int sceneIndex = scenesToPreload[i];
            Debug.Log($"Preloading scene: {sceneNames[sceneIndex]}");
            
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneNames[sceneIndex]);
            asyncLoad.allowSceneActivation = false; // Prevent automatic activation
            preloadedScenes[i] = asyncLoad;
            
            while (asyncLoad.progress < 0.9f)
            {
                yield return null;
            }
            
            Debug.Log($"Scene {sceneNames[sceneIndex]} preloaded and ready");
        }
    }

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
        AudioManager.Instance.Play2D(AudioSoundIds.SoundDesign.UI.PlayGame);
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
            Debug.Log($"Found animator: {animator.gameObject.name} ");
            animator.gameObject.SetActive(true);
            animator.SetTrigger("StartFadeOut");
            yield return new WaitForSeconds(1f);
        }
        else
        {
            Debug.Log("No animator found");
        }
        
        Debug.Log($"Loading scene: {sceneNames[sceneIndex]}");
        if (sceneIndex == 0)//Skip preload for main menu
        {
            SceneManager.LoadScene(sceneIndex);
        }
        else
        {
            AsyncOperation preloadedScene = GetPreloadedScene(sceneIndex);
            if (preloadedScene != null)
            {
                Debug.Log($"Activating preloaded scene: {sceneNames[sceneIndex]}");
                preloadedScene.allowSceneActivation = true;
            }
            else
            {
                SceneManager.LoadScene(sceneIndex);
            }
        }
    }
    
    private AsyncOperation GetPreloadedScene(int sceneIndex)
    {
        if (preloadedScenes == null) return null;
        
        for (int i = 0; i < scenesToPreload.Length; i++)
        {
            if (scenesToPreload[i] == sceneIndex)
            {
                return preloadedScenes[i];
            }
        }
        return null;
    }
    
    public void PlayGameNow()
    {
        SceneManager.LoadScene(1);
    }

}
