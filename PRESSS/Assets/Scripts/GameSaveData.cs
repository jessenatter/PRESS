using UnityEngine;
using System.IO;

public class Menu : MonoBehaviour
{
    public void NewGame()
    {
        GameDataManager.Reset();
        GameDataManager.SaveToDisk();
        LoadNextScene();
    }

    public void Continue()
    {
        if (GameDataManager.SaveExists())
        {
            GameDataManager.LoadFromDisk();
            LoadNextScene();
        }
    }

    public void Settings()
    {

    }

    void LoadNextScene()
    {
        int index = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex + 1;
        UnityEngine.SceneManagement.SceneManager.LoadScene(index);
    }
}

public static class GameDataManager
{
    private static string path = Application.persistentDataPath + "/save.json";
    public static int savedWave = 0;
    public static int savedScore = 0;

    public static void Reset()
    {
        savedWave = -1;//bc we add one onto it later
        savedScore = 0;
    }

    public static void SaveToDisk()
    {
        SaveData data = new SaveData
        {
            savedWave = savedWave,
            savedScore = savedScore
        };

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(path, json);
    }

    public static void LoadFromDisk()
    {
        if (!File.Exists(path)) return;

        string json = File.ReadAllText(path);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        if (data != null)
        {
            savedWave = data.savedWave;
            savedScore = data.savedScore;
        }
    }

    public static bool SaveExists()
    {
        return File.Exists(path);
    }

    public static void DeleteSave()
    {
        if (File.Exists(path))
            File.Delete(path);
    }
}

[System.Serializable]
public class SaveData
{
    public int savedWave;
    public int savedScore;
}
