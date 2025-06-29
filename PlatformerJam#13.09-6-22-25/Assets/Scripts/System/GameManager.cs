using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private SaveData saveData;
    RegionManager regionManager;

    private void Awake()
    {
        saveData = SaveManager.LoadGame();

        GameObject player = FindAnyObjectByType<PlayerScript>().gameObject;
        player.transform.position = saveData.GetPositionVector();
        regionManager = FindAnyObjectByType<RegionManager>();
        regionManager.CurrentRegionID = saveData.CurrentLevel;

        Debug.Log($"Loaded - Level: {saveData.CurrentLevel}, Position: {player.transform.position}");
    }

    public void SaveCurrentState(int currentLevel, Vector3 playerPosition)
    {
        saveData.CurrentLevel = currentLevel;
        saveData.SetPositionVector(playerPosition);
        SaveManager.SaveGame(saveData);
    }

    public void Pause()
    {
        Time.timeScale = 0.0f;
    }

    public void Resume()
    {
        Time.timeScale = 1.0f;
    }

    public void Restart()
    {
        GameObject player = FindAnyObjectByType<PlayerScript>().gameObject;
        player.transform.position = saveData.GetPositionVector();
        regionManager = FindAnyObjectByType<RegionManager>();
        regionManager.CurrentRegionID = saveData.CurrentLevel;
        LoadScene("FinalGame");
    }
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void CloseGame()
    {
        Application.Quit();
    }


    private void OnApplicationQuit()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        SaveCurrentState(regionManager.CurrentRegionID, player.transform.position);
    }
}