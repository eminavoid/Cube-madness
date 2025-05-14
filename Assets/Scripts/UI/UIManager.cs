using UnityEngine;
using UnityEditor.SceneManagement;
public class UIManager : MonoBehaviour
{

    public GameObject MainScreen, OptionsScreen, CreditsScreen;

    public void PlayGame()
    {
        EditorSceneManager.LoadScene("SampleScene");
    }

    public void OpenCredits()
    {
        MainScreen.SetActive(false);
        CreditsScreen.SetActive(true);
    }

    public void CloseCredits()
    {
        CreditsScreen.SetActive(false);
        MainScreen.SetActive(true);
    }

    public void OpenOptions()
    {
        MainScreen.SetActive(false);
        OptionsScreen.SetActive(true);
    }

    public void CloseOptions()
    {
        OptionsScreen.SetActive(false);
        MainScreen.SetActive(true);
    }

    public void CloseGame()
    {
        Application.Quit();
    }
}
