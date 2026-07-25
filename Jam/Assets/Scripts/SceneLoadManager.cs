using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadManager : MonoBehaviour
{
    public static SceneLoadManager Instance { get; private set; }
    
    [SerializeField] private bool transitionOnAwake = false;
    [SerializeField] private float transitionDuration = 0.5f;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject reloadHint;
    
    private AsyncOperation preloadOperation; 
    private bool isLoading = false;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        
        if (transitionOnAwake)
        {
            animator.gameObject.SetActive(true);
            animator.SetTrigger("Start");
            isLoading = true;
            StartCoroutine(OnTransitionStart());
        }
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }

        if (Input.GetKeyDown(KeyCode.Delete))
        {
            ClearAllSaves();
        }
    }

    public void OpenReloadHint()
    {
        reloadHint.SetActive(true);
    }

    public void CloseReloadHint()
    {
        reloadHint.SetActive(false);
    }
    
    public void OpenScene(string scene)
    {
        if (!Application.CanStreamedLevelBeLoaded(scene) && isLoading) return;
        isLoading = true;
        
        // MusicManager.instance.PlayRandomSound(MusicManager.SoundType.ButtonPress);
        StartCoroutine(PreloadAndTransition(scene));
    }

    public void OpenNextScene()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        
        if (nextSceneIndex >= SceneManager.sceneCountInBuildSettings || isLoading) return;
        isLoading = true;
        
        
        // MusicManager.instance.PlayRandomSound(MusicManager.SoundType.ButtonPress);
        
        string nextScene = SceneUtility.GetScenePathByBuildIndex(nextSceneIndex);
        StartCoroutine(PreloadAndTransition(nextScene));
    }

    public void RestartGame()
    {
        if (isLoading) return;
        isLoading = true;
        
        // MusicManager.instance.PlayRandomSound(MusicManager.SoundType.ButtonPress);
        
        string currentScene = SceneManager.GetActiveScene().name;
        StartCoroutine(PreloadAndTransition(currentScene));
    }
    
    private IEnumerator PreloadAndTransition(string scene)
    {
        if (preloadOperation != null) yield break;
        
        animator.gameObject.SetActive(true);
        animator.SetTrigger("End");
        
        yield return new WaitForSeconds(transitionDuration);
        
        preloadOperation = SceneManager.LoadSceneAsync(scene);
        preloadOperation.allowSceneActivation = false;
     
        while (preloadOperation.progress < 0.9f)
            yield return null;
        
        preloadOperation.allowSceneActivation = true;
    }
    
    private IEnumerator OnTransitionStart()
    {
        yield return new WaitForSeconds(transitionDuration);
        animator.gameObject.SetActive(false);
        isLoading = false;
    }

    public void ClearAllSaves()
    {
        // MusicManager.instance.PlayRandomSound(MusicManager.SoundType.ButtonPress);
        PlayerPrefs.DeleteAll();
    }

    public void QuitGame()
    {
        // MusicManager.instance.PlayRandomSound(MusicManager.SoundType.ButtonPress);
        Application.Quit();
    }
}
