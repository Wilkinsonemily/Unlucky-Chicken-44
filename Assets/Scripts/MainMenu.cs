using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Scenes")]
    public string gameSceneName;   

    [Header("Panels")]
    public GameObject optionsPanel; 

    public void PlayGame()
    {
        if (string.IsNullOrEmpty(gameSceneName))
        {
            Debug.LogError("gameSceneName kosong. Isi di Inspector!");
            return;
        }

        Debug.Log("PLAY CLICKED");

        SceneManager.sceneLoaded -= OnSceneLoadedPlayMusic;
        SceneManager.sceneLoaded += OnSceneLoadedPlayMusic;

        SceneManager.LoadScene(gameSceneName, LoadSceneMode.Single);
    }

    private void OnSceneLoadedPlayMusic(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != gameSceneName) return;

        SceneManager.sceneLoaded -= OnSceneLoadedPlayMusic;

        Debug.Log("SCENE LOADED: " + scene.name);

        if (AudioManager.Instance != null)
        {
            Debug.Log("Calling PlayGameMusic()");
            AudioManager.Instance.PlayGameMusic();
        }
        else
        {
            Debug.LogWarning("AudioManager.Instance is NULL after scene load");
        }
    }

    public void OpenOptions()
    {
        Debug.Log("OPTIONS CLICKED");
        if (optionsPanel != null)
            optionsPanel.SetActive(true);
    }

    public void CloseOptions()
    {
        Debug.Log("OPTIONS CLOSED");
        if (optionsPanel != null)
            optionsPanel.SetActive(false);
    }

    public void QuitGame()
    {
        Debug.Log("QUIT CLICKED");
        Application.Quit();
    }
}
