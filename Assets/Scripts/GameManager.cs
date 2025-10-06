using UnityEngine;
using UnityEngine.SceneManagement;

public class Scenes : MonoBehaviour
{
    private BaseLab currentLab;
    public static int previousSceneIndex;
    public bool isPaused = false;

    void Start()
    {
        isPaused = true;
        FindCurrentLab(); 
    }

    private void FindCurrentLab()
    {
        currentLab = FindObjectOfType<BaseLab>();
        if (currentLab == null)
        {
            Debug.LogError("Лабораторная работа не найдена на сцене!");
        }
    }

    public void LoadScene(int sceneIndex)
    {
        previousSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(sceneIndex);
    }

    public void LoadPreviousScene()
    {
        SceneManager.LoadScene(previousSceneIndex);
    }

    public void Pause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0;
            if (currentLab != null)
            {
                currentLab.movingObject.transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            Debug.Log("Игра на паузе");
        }
        else
        {
            Time.timeScale = 1;
            Debug.Log("Игра продолжена");
            if (currentLab != null)
            {
                currentLab.movingObject.transform.rotation = Quaternion.Euler(0, -90, 0);

                ExecuteTask();
            }
        }
    }

    public void ExecuteTask()
    {
        if (currentLab != null)
        {
            currentLab.ExecuteTask();
        }
        else
        {
            Debug.Log("Лабораторная работа не найдена!");
        }
    }
}