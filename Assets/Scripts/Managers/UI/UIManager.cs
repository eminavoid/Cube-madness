using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup loadingScreen;
    [SerializeField] private CanvasGroup ui1;
    [SerializeField] private CanvasGroup ui2;

    [SerializeField] private Slider loadingSlider;
    
    public void BackToMenu()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // Guardamos el �ndice de la escena actual
        PlayerPrefs.SetInt("SavedLevel", currentSceneIndex);
        PlayerPrefs.Save();

        SceneManager.LoadScene("Main Menu");
    }

    public void Play()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int nextIndex = currentIndex + 1;

        // Verificamos que exista una siguiente escena
        if (nextIndex < SceneManager.sceneCountInBuildSettings)
        {
            if (ui1 != null)
            {
                ui1.alpha = 0f;
                ui1.interactable = false;
                ui1.blocksRaycasts = false;
            }

            if (ui2 != null)
            {
                ui2.alpha = 0f;
                ui2.interactable = false;
                ui2.blocksRaycasts = false;
            }
            
            loadingScreen.alpha = 1f;
            loadingScreen.interactable = false;
            loadingScreen.blocksRaycasts = true;

            StartCoroutine(LoadLevelAsync(nextIndex));
            // SceneManager.LoadScene(nextIndex);
        }
        else
        {
            Debug.LogWarning("No hay m�s escenas en el build index.");
        }
    }

    public void Continue()
    {
        if (PlayerPrefs.HasKey("SavedLevel"))
        {
            int levelIndex = PlayerPrefs.GetInt("SavedLevel");
            
            if (ui1 != null)
            {
                ui1.alpha = 0f;
                ui1.interactable = false;
                ui1.blocksRaycasts = false;
            }

            if (ui2 != null)
            {
                ui2.alpha = 0f;
                ui2.interactable = false;
                ui2.blocksRaycasts = false;
            }
            
            loadingScreen.alpha = 1f;
            loadingScreen.interactable = false;
            loadingScreen.blocksRaycasts = true;

            StartCoroutine(LoadLevelAsync(levelIndex));
            
            // SceneManager.LoadScene(levelIndex);
        }
        else
        {
            Debug.Log("No hay partida guardada disponible");
        }
    }

    public void Exit()
    {
        Application.Quit();
    }

    IEnumerator LoadLevelAsync(int levelToLoad)
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(levelToLoad);

        while (!loadOperation.isDone)
        {
            float progressValue = Mathf.Clamp01(loadOperation.progress / 0.9f);
            loadingSlider.value = progressValue;
            yield return null;
        }
    }
}
