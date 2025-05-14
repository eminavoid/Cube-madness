using UnityEngine;
using UnityEngine.SceneManagement;
public class RunningUI : MonoBehaviour
{
    public DotController PlayerScript;
    public TMPro.TextMeshProUGUI Text;
    bool IsPaused = false;
    public GameObject PauseMenu;

  
    private void Update()
    {
        Text.text = PlayerScript.numberOfMiniPlayers.ToString();

        //if (Input.GetKeyDown(KeyCode.Escape))
        //{
        //    Debug.Log("Pause!");
        //    IsPaused = !IsPaused;
        //    PauseMenu.SetActive(true);
        //    Time.timeScale = IsPaused ? 0 : 1;
        //    PauseMenu.SetActive(IsPaused);
        //}
       
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void ExitGame()
    {
        Debug.Log("Game exit!");
        Application.Quit();
    }

}
