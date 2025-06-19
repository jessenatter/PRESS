using UnityEngine;
using System.IO;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public GameObject settingsMenu,buttons,postProcessObject;
    public float musicVolume = 1f, soundEffectVolume = 1f;
    bool usePostProcessing = true;
    Volume postProcessVolume;
    Slider musicSlider, soundEffectSlider;

    private void Awake()
    {
        GameDataManager.LoadFromDisk();
        postProcessVolume = gameObject.transform.GetChild(2).GetComponent<Volume>();
        postProcessVolume.enabled = GameDataManager.usePostProcessing;
        musicVolume = GameDataManager.musicVolume;
        soundEffectVolume = GameDataManager.soundEffectVolume;
        musicSlider = settingsMenu.transform.GetChild(1).transform.GetChild(1).GetComponentInChildren<Slider>();
        soundEffectSlider = settingsMenu.transform.GetChild(1).transform.GetChild(2).GetComponentInChildren<Slider>();
        postProcessObject = settingsMenu.transform.GetChild(1).transform.GetChild(3).transform.GetChild(0).transform.GetChild(0).gameObject;
    }

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
        soundEffectSlider.value = soundEffectVolume;
        musicSlider.value = musicVolume;
        settingsMenu.SetActive(true);
        buttons.SetActive(false);
    }

    void LoadNextScene()
    {
        int index = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex + 1;
        UnityEngine.SceneManagement.SceneManager.LoadScene(index);
    }

    public void ReturnToMenu()
    {
        soundEffectVolume = soundEffectSlider.value;
        musicVolume = musicSlider.value;
        GameDataManager.usePostProcessing = usePostProcessing;
        GameDataManager.soundEffectVolume = soundEffectVolume;
        GameDataManager.musicVolume = musicVolume;
        postProcessVolume.enabled = GameDataManager.usePostProcessing;
        GameDataManager.SaveToDisk();
        settingsMenu.SetActive(false);
        buttons.SetActive(true);
    }

    public void UsePostProcessing()
    {
        usePostProcessing = !usePostProcessing;
        postProcessVolume.enabled = usePostProcessing;
        postProcessObject.SetActive(usePostProcessing);
    }
}

public static class GameDataManager
{
    private static string path = Application.persistentDataPath + "/save.json";
    public static int savedWave = 0;
    public static int savedScore = 0;
    public static int highscore = 0;

    public static float musicVolume = 1,soundEffectVolume = 1;
    public static bool usePostProcessing = true;

    public static void Reset()
    {
        savedWave = -1;//bc we add one onto it later
        savedScore = 0;
        musicVolume = 1;
        soundEffectVolume = 1;
        usePostProcessing = true;
        highscore = 0;
    }

    public static void SaveToDisk()
    {
        SaveData data = new SaveData
        {
            savedWave = savedWave,
            savedScore = savedScore,
            musicVolume = musicVolume,
            soundEffectVolume = soundEffectVolume,
            usePostProcessing = usePostProcessing,
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
            musicVolume = data.musicVolume;
            soundEffectVolume = data.soundEffectVolume;
            usePostProcessing = data.usePostProcessing;
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
    public float musicVolume;
    public float soundEffectVolume;
    public bool usePostProcessing;
}
