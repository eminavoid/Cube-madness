using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup loadingScreen;
    [SerializeField] private CanvasGroup ui1;
    [SerializeField] private CanvasGroup ui2;
    [SerializeField] private Slider loadingSlider;

    [SerializeField] private List<string> sceneAddresses;
    
    public void BackToMenu()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        PlayerPrefs.SetString("SavedLevelAddress", currentScene);
        PlayerPrefs.Save();
        ShowLoadingScreen();
        StartCoroutine(LoadAddressableSceneAsync("Main Menu"));
    }

    public void Play()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        int currentIndex = sceneAddresses.IndexOf(currentSceneName);

        if (currentIndex >= 0 && currentIndex + 1 < sceneAddresses.Count)
        {
            string nextScene = sceneAddresses[currentIndex + 1];
            Debug.Log(nextScene);
            ShowLoadingScreen();
            StartCoroutine(LoadAddressableSceneAsync(nextScene));
        }
        else
        {
            BackToMenu();
        }
    }

    public void Continue()
    {
        if (PlayerPrefs.HasKey("SavedLevelAddress"))
        {
            string savedScene = PlayerPrefs.GetString("SavedLevelAddress");
            ShowLoadingScreen();
            StartCoroutine(LoadAddressableSceneAsync(savedScene));
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

    IEnumerator LoadAddressableSceneAsync(string sceneAddress)
    {
        AsyncOperationHandle<SceneInstance> handle = Addressables.LoadSceneAsync(sceneAddress, LoadSceneMode.Single);

        while (!handle.IsDone)
        {
            float progressValue = Mathf.Clamp01(handle.PercentComplete);
            loadingSlider.value = progressValue;
            yield return null;
        }
    }
    
    private void ShowLoadingScreen()
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
    }
}
