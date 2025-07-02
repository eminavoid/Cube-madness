using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public void BackToMenu()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // Guardamos el índice de la escena actual
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
            SceneManager.LoadScene(nextIndex);
        }
        else
        {
            Debug.LogWarning("No hay más escenas en el build index.");
        }
    }

    public void Continue()
    {
        if (PlayerPrefs.HasKey("SavedLevel"))
        {
            int levelIndex = PlayerPrefs.GetInt("SavedLevel");
            SceneManager.LoadScene(levelIndex);
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
}
