using UnityEngine;

public class MainMenuController : MonoBehaviour
{

        [SerializeField] private GameObject choosingPlayModePanel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void loadSceneByName(string sceneName){
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);

    }

    public void setDifficultyEasy(){
        GameSettings.SetDifficulty(GameSettings.GameDifficulty.Easy);
    }
    public void setDifficultyNormal(){
        GameSettings.SetDifficulty(GameSettings.GameDifficulty.Normal);
    }
    public void setDifficultyHard(){
        GameSettings.SetDifficulty(GameSettings.GameDifficulty.Hard);
    }
    public void setDifficultyExtreme(){
        GameSettings.SetDifficulty(GameSettings.GameDifficulty.Extreme);
    }

    public void showSingleplayerDifficultyPanel(){
        choosingPlayModePanel.SetActive(true);

    }

    public void hideSingleplayerDifficultyPanel(){
        choosingPlayModePanel.SetActive(false);
 
    }


    //TODO: start singleplayer game with the chosen difficulty
    public void startSingleplayerGame(){
        UnityEngine.SceneManagement.SceneManager.LoadScene("SingleplayerDifficultyScene");

    }

    public void exitGame(){
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
    
}
